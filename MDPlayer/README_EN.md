# MDPlayer
Player for VGM files, etc. (a performance tool that emulates Mega Drive sound chips, etc.)  
  
[Overview]  
  This tool plays a VGM file while displaying the keyboard.  
  (NRD,XGM,S98,MID,RCP,NSF,HES,SID,MGS,MDR,MDX,MND,MUC,MUB,M,M2,MZ,WAV,MP3,AIFF files are also supported.)  
  
[Caution.]  
  Please pay attention to the volume when playing. Noise caused by bugs may be played back at high volume.  
  (Especially when you try a file you have never played before, or when you update the program.)  
  
  If you find any problems while using the program, please contact us.  
    Twitter(@kumakumakumaT_T)  
    Github Issues(https://github.com/kuma4649/MDPlayer/issues)  
  Important! Important!  
  Please contact the authors of VGMPlay, NRTDRV, and other great software  
  Please do not contact the authors of VGMPlay, NRTDRV and other great software directly about MDPlayer.  
  We will do our best to help you, but we may not always be able to meet your needs. Please understand.  
  
[Supported formats].  
  VGM (so-called vgm files)  
  .VGZ (gziped vgm file)  
  .NRD (Performance file of the driver that plays 2 OPMs and AY8910 on NRTDRV X1)  
  .XGM (file for MegaDrive)  
  .S98 (mainly for Japanese retro PCs)  
  .MID (Standard MIDI file. Supports format 0/1)  
  .RCP (Recompon file, CM6, GSD can be sent)  
  .NSF (NES Sound Format)  
  .HES (HES file)  
  .SID (Commodore file)  
  .MGS (MGSDRV file. MGSDRV.COM is required to play this file)  
  .MDR (performance file for MoonDriver MSX driver that plays MoonSound (OPL4))  
  .MDX (File for MXDRV)  
  .MND (Performance file of the driver which uses MNDRV X68000(OPM,OKIM6258) & Markyuryunit(OPNAx2))  
  .MUC (file for MUCOM88Windows)  
  .MUB (file for MUCOM88Windows)  
  .M (file for PMD)  
  .M2 (files for PMD)  
  .MZ (files for PMD)  
  .WAV (TBD audio files)  
  .MP3 (TBD audio files)  
  .AIF (TBD audio files)  
  .M3U (playlists)  
  
[M3U (playlist)  
  Currently, the following Mega Drive sound chips can be emulated for playback.  
     
      AY8910, YM2612, SN76489, RF5C164, PWM, C140(C219), OKIM6295, OKIM6258(including PCM8, MPCM), SEGAPCM, YM2CM  
      , SEGAPCM , YM2151 , YM2203 , YM2413 , YM2608 , YM2610/B , HuC6280 , C352  
      , K054539 , NES_APU , NES_DMC , NES_FDS , MMC5 , FME7 , N160 , VRC6  
      , VRC7 , MultiPCM , YMF262 , YMF271 , YMF278B , YMZ280B , DMG , QSound  
      , S5B , GA20 , X1-010 , SAA1099
      RF5C68, SID, Y8950, YM3526, YM3812, K053260, K051649(K052539)  
  
  Currently, the following keyboard displays are available.  
     
      YM2612 , SN76489 , RF5C164  
      YM2612 , SN76489 , RF5C164 , AY8910 , C140(C219) , C352 , SEGAPCM  
      , Y8950 , YM2151 , YM2203 , YM2413 , YM2608 , YM2610/B , YM3526 , YM3812  
      , YMF262 , YMF278B , YMZ280B , MultiPCM   
      , HuC6280 , MIDI       
      , NES_APU&DMC , NES_FDS , MMC5 , N106(N163) , VRC6 , VRC7 , PPZ8    
  
      Left-click on a channel (keyboard) to mask it.  
      Right-click to unmask all channels.  
      (Some of the various levels are not supported.)  
    　Clicking on 'ch' in each keyboard display window will toggle the mask in one go.  


      Automatically open the keyboard to be used from the information in the file to be played.  
      (Up to two identical keys can be displayed, but only one MIDI key will be opened.)  
  
  It is created in C#.  
  
  The source code of VGMPlay, MAME, and DOSBOX are referenced and ported.  
  
  The source code of FMGen is referenced and ported.  
  
  Reference and porting of NSFPlay source code.  
  
  Refer to and port the source of NEZ Plug++.  
  
  Reference and porting of the libsidplayfp source.  
  
  Referencing and porting the source of sidplayfp.  
  
  The source code of NRTDRV is referenced and ported.  
  
  Reference and porting of the source code of MoonDriver.  
  
  Reference and porting of the MXP source code.  
  
  Reference and porting of the MXDRV source code.  
  
  Reference and porting of the MNDRV source code.  
  
  The X68Sound source code has been ported for reference.  
  (Both m_puusan's and rururutan's versions)  
  
  The output of CVS.EXE is used as a reference and adjusted to output the same data.  
  
  It can be played from real YM2612,SN76489,YM2608,YM2151,YMF262 using SCCI.  
  SPPCM is also supported.  
  
  Playback from real YM2608, YM2151, YMF262 using GIMIC (C86ctl).  
  
  Z80dotNet is used.  
  
  The buttons are arranged in the following order.  
     
     Setting, Stop, Pause, Fade out, Previous song, 1/4 speed play, Play, 4x speed play, Next song, Play mode  
     Play mode, open file, playlist, playlist, play mode  
     Information panel display, Mixer panel display, Panel list display, VSTeffect settings, MIDI keyboard display, Change display magnification  
  
  Left-clicking on an OPN, OPM, or OPL tone parameter will copy the tone parameter to the clipboard as text.  
  The parameter format can be changed from the option settings.  
     
      FMP7 , MDX , MUCOM88(MUSIC LALF) , NRTDRV , HuSIC , MML2VGM , .TFI , MGSC , .DMP , .OPNI  
  
  DMP, and .OPNI, and when .TFI, .DMP, or .OPNI is selected, it will output to a file instead of the clipboard.  
  
  If you choose .  
  If you use VOPMex, it is possible to reflect FM sound source tone information.  
  (Not VOPM, but VOPMex. (It is VOPMex, not VOPM. ;-P )  
  
  It is possible to dump PCM data, and only in the case of SEGAPCM, it is output in WAV.  
  
  It is possible to export the performance as wav.  
  
  It is possible to specify VSTi as MIDI sound source.  
  
  Playback, stop, and other operations can be performed from a keyboard or MIDI keyboard.  
  
  It is possible to open files with the same name (Text, MML, Image) with different extensions from the playlist.  
  
  VGM/VGZ files have their own functions.  
      Dual performance of RF5C164  
      Lyrics display  

  MDPlayer can be operated from command line using mdp.bat.  
        The command of mdp.bat is as follows.  
            PLAY [file]  
            STOP [file  
            NEXT  
            PREV [file] FADEOUT  
            FADEOUT  
            FAST  
            SLOW  
            PAUSE  
            CLOSE  
            LOOP  
            MIXER  
            INFO  
  
[A little confusing operation]  
  Double-clicking (toggling) the title bar of each window will always bring it to the front.  
　-Added a function to initialize the window position by starting the application while holding down the Shift key.  

  
  
[G.I.M.I.C. related information]  
  About SSG volume  
    SSG volume can be adjusted using the "G.OPN" and "G.OPNA" faders at the bottom right of the mixer screen.  
    The faders are  
      G.OPN -> G.I.M.I.C. module set to YM2203 (Pri/Sec)  
      G.OPNA -> Module of G.I.M.I.C. set to YM2608(Pri/Sec)  
    The setting information will be sent to the module of G.I.M.I.C. set to YM2608(Pri/Sec).  
    Note that the settings are sent only when playback starts.  
    Therefore, even if you move the fader while playing, the value will not be reflected immediately.  
    The initial values are  
      .muc(mucom88) -> 63 (equivalent to PC-8801-11)  
      .mub(mucom88) -> 63 (PC-8801-11 equivalent)  
      .mnd(MNDRV) -> 31 (PC-9801-86 equivalent)  
      .s98 -> 31 (PC-9801-86 equivalent)  
      .vgm -> 31 (PC-9801-86 equivalent)  
    is set.  
    Adjust the balance on a per-driver or per-file basis if necessary, and save (right-click on the mixer screen).  
    (Right-click on the mixer screen to display the save menu.  
    
    In addition, the following performance files can be set automatically by identifying the tags described in the files (TBD).
    
    .S98 file  
    MDPlayer will set "63" if it finds the character "8801" in the "system" tag.  
    If it finds the letters "9801", MDPlayer will set it to "31".  
    If both are found, "8801" will be given priority.  
    If not found, the value will be the one you set in the mixer screen.  
    
    .vgm file  
    If "8801" is found in the "systemname" and "systemnamej" tags, MDPlayer will set the value to "63".  
    If you find the letters "9801", MDPlayer will set the value to "31".  
    If both are found, "8801" will be given priority.  
    If both are found, "8801" has priority.  
    
    
  About frequency  
    Set the frequency of the module (master clock of the chip) for each file format.  
    The setting values are as follows  
      .vgm -> Use the value set in the file.  
      .s98 -> Use the value set in the file  
      .mub(mucom88) -> OPNA:7987200Hz  
      .muc(mucom88) -> OPNA:7987200Hz  
      .nrd(NRTDRV) -> OPM:4000000Hz  
      .mdx(MXDRV) -> OPM:4000000Hz  
      .mnd(MNDRV) -> OPM:4000000Hz OPNA:8000000Hz  
      .mml(PMD) -> OPNA:7987200Hz  
      .m(PMD) -> OPNA:7987200Hz  
      .m2(PMD) -> OPNA:7987200Hz  
      .mz(PMD) -> OPNA:7987200Hz  
  
  
[Required Operating Environment]  
  I am using Windows10Home(x64).  
  NET Framework4 installed.  
  
  NET Framework4 must be installed.  
  
  NET Standard2 must be installed.  
  
  NET Standard2 must be installed. - Visual C++ Redistributable Package of Visual Studio 2012 Update 4 must be installed.  
  
  Microsoft Visual C++ 2015 Redistributable(x86) - 14.0.23026 must be installed.  
  
  If you want to use LZH file, you need to install UNLHA32.DLL(Ver3.0 or later).  
  
  An audio device that can play audio is required.  
  The UCA222 that came as an extra with the UMX250 is good enough. I used this one.  
  
  If you have one, use SPFM Light + YM2612 + YM2608 + YM2151 + SPPCM  
  
  GIMIC + YM2608 + YM2151 if available  
  
  When emulating the YM2608, the following audio files are needed to play the rhythm sound.  
  The following audio files are needed to play the rhythm sound when emulating the YM2608.  
      
      Bass drum 2608_BD.WAV  
      Hi-Hat 2608_HH.WAV  
      Rim Shot 2608_RIM.WAV  
      Snare drum 2608_SD.WAV  
      Tam-Tam 2608_TOM.WAV  
      Top cymbal 2608_TOP.WAV  
      (44.1KHz 16bitPCM monaural uncompressed Microsoft WAVE format file)  
  
  When emulating the YMF278B, the following ROM file is required to play the MoonSound tone.  
  The following ROM file is required to play the MoonSound sound when emulating the YMF278B. I'm sorry, but I'll leave the creation method to you.  
  	yrw801.rom  
  
  When emulating the C64, the following ROM file is required.  
  The creation method is left up to you.  
  	Kernal , Basic , Character  
  
  A reasonably fast CPU.  
  The amount of processing required will vary depending on the Chip used.  
  I am using an i7-9700K 3.6GHz.  
  
  To play MGSDRV files, you need the following files.  
  (I have included them in advance, but if you need them, please get them from the official website.)
    MGSDRV.COM  

  
[Synchronization]  
    
  Synchronizing SCCI/GIMIC (C86ctl) and emulation (EMU) sounds is a tricky thing.  
  It depends on your environment, so I don't know what the right answer is, but I'll show you the adjustment procedure in my environment. 1.  
      
    1. First, select the device you want to use for audio output from the [Output] tab.  
    The recommended pattern is to use WasapiOut and select Shared or ASIO. 2.  
      
    2. Select either 50ms or 100ms for the delay time. Press [OK] once and play a song that uses only EMU.  
    Make sure the sound is not rough or noisy.  
    (If it doesn't play back cleanly, set the delay time to a larger value.)  
      
    Select SCCI of YM2612 from the [Sound] tab and select the module you want to use.  
    SCCI only  
    Check "Send Wait Signal" and "Emulate PCM only" checkboxes.  
    If "Emulate PCM only" is checked, only PCM will be emulated.  
    If the "Emulate PCM only" checkbox is unchecked, PCM data will be sent to SCCI, but the sound quality and tempo will not be stable.  
    "Send Wait Signal" seems to stabilize the tempo at SCCI.  
    However, if "Double wait" is checked, the PCM sound quality is improved but the tempo tends to be unstable. 4.  
      
    For the delayed performance group, set SCCI/GIMIC and EMU to 0 ms for now, and check the "opportunistic mode" checkbox.  
    In the "opportunistic mode", for example, if a large load is applied during the performance and the SCCI/GIMIC playback and the EMU playback are far apart, the SCCI/GIMIC playback will be delayed.  
    This function adjusts the SCCI/GIMIC playback speed to reduce the gap. However, the gap set (or intended) in the delayed performance will be maintained. 5.  
      
    Play a song where both SCCI/GIMIC and EMU are used, and carefully check which one sounds first.  
    Increase the delay time of the SCCI/GIMIC or EMU that is playing first, and play the song to check. 6.  
      
    Repeat steps 6 and 5 until there is no misalignment, and the synchronization process is complete. Have fun!  
      
    7. About the performance gap between SCCI and GIMIC.  
    If SCCI is faster, adjust the delay setting of SCCI.  
    If GIMIC is faster, adjust GIMIC's delay setting.  
  
  
[How to use a MIDI keyboard]  
  If you have a MIDI keyboard, you can use it to make sounds from the YM2612 (EMU).  
  This function is mainly for supporting MML typing.  
  (This function is mainly for supporting MML input.)  
  
  How to use it for the time being  
      
    In the settings screen, select the MIDI keyboard you want to use. 2.  
       
    Send (CC:97) while the YM2612 data is playing.  
        The tone of 1Ch of the YM2612 will be set to all channels. 3.  
       
    All you have to do is play.  
    
  Main Functions 1.  
      
    Importing tone data  
      Click on the tone data section of the keyboard display of each OPN or OPM sound source.  
      The tone data will be copied to the selected channel. 2.  
      
    2. performance mode switching  
      MONO mode (playing using a single channel) and  
      You can switch between MONO mode (playing using a single channel) and POLY mode (playing using multiple channels (up to 6)).  
      MONO mode  
      MONO mode is intended for playing short phrases during recording and outputting them as MML.  
      POLY mode is used to check chords during recording. 3.  
      
    3. channel note log  
      Up to 100 notes can be recorded for each channel.  
      
    4. channel note log MML conversion function  
      Clicking on the log field will copy the pronunciation record to the clipboard as MML.  
      The note length will not be output. The corresponding command is c d e f g a b o < >.  
      Only the first note of the octave information is specified absolutely by the o command, and the rest are specified relative to the octave by the < command> command.  
      The octave information is specified absolutely by the o command for the first note, and then expanded by relative specification using the < command> command. 5.  
      
    5. tone storage and loading  
      It is possible to save or load 256 types of tones in memory.  
      The data can be output to or loaded from a file in a specified format.  
      It can be saved and loaded in the following formats for the following software  
        FMP7  
        MUCOM88(MUSIC LALF/mucomMD2vgm)  
        NRTDRV  
        MXDRV  
        mdl2vgm  
      
    6. simple tone editing (TBD)  
      After selecting the parameter you want to enter, you can edit it by entering a numerical value.  
      
  Screen  
      
    Keypad (TBD)  
      Shows the note you are playing. 1.  
      
    1. MONO  
      Click to switch to MONO mode. Click to switch to MONO mode, and the icon will appear. 2.  
      
    2．POLY  
      Click to switch to POLY mode. When the mode is switched, the icon will appear. 3.  
      
    3．PANIC  
      Sends key-off to all channels. (This is used when the sound keeps playing.) 4.  
      
    L.CLS  
      Clears the note log for all channels. 5.  
      
    TP.PUT  
      Saves the selected channel's tone to the TonePallet (tone storage area in memory). 6.  
      
    TP.GET  
      Loads a tone from TonePallet to the selected channel.  
      
    T.SAVE  
      Saves the TonePallet to a file.  
      
    8. t.load  
      Loads the TonePallet from the file. 9.  
      
    T.LOAD Loads the TonePallet from the file. 9. tone data (for 6 channels)  
      You can select and deselect channels by clicking "-" or "Â".  
      By right-clicking on a parameter, you can change that parameter. (TBD)  
      By left-clicking on a parameter, a context menu is displayed.  
        Copy : Copy the clicked tone to the clipboard.  
        Paste : Pastes the tone on the clipboard to the clicked tone.  
        The text format used for the above functions can be changed by clicking on the software name in the FORMAT field.  
        You can also copy and paste by keystroke. In this case, the selected channel will be the target.  
        In this case, the selected channel will be used.  
      Clicking the "Â" next to "LOG" will clear the note log for that channel.  
      Click "LOG" to set the MML data to the clipboard.  
      
  Operation from MIDI keyboard  
    The following are the default settings. (Can be customized in the settings. (Can be customized in the settings, and can be disabled by setting the setting value to blank.)  
      
    CC:97(DATA DEC)  
      Copies the tone of 1Ch of the YM2612 to all channels. (Ignores selection status)  
      
    CC:96(DATA INC)  
      Deletes only one of the most recent logs. (Deletes the most recent log.) (This function cancels misplaying, etc.)  
      
    CC:66(SOSTENUTO)  
      In MONO mode only, set the log of the selected line to the clipboard as MML.  
      Difference from the process when clicking on the screen  
        Sends the keystroke Ctrl+V (paste).  
        Clears the note log of the selected channel.  
        The first octave command is not output.  
      
      
[SpecialThanks]  
  Thanks to the following people for their help with this tool. The following software and web pages were referenced and used.  
  Thank you very much.  
     
    Mr. Rael  
    Mr. Tomokegao  
    HI-RO  
    Starvation 3  
    Oyaji Pippi  
    Mr. Osoumen  
    Nurito  
    Mr. Hex125  
    Mr. Kitao Nakamura  
    Mr. Kuroma  
    Katao Nakamura  
    Mr. Kakiuchi  
    dj.tuBIG/MaliceX  
    WING  
    WING  
    WING  
    Gou Ooba  
    sgq1205  
    Chigiri@Buchigiri P(but80)  

    Visual Studio Community 2015/2017  
    MinGW/msys  
    gcc  
    SGDK  
    VGM Player  
    VGM Player ・Git  
    SourceTree  
    Sakura Editor  
    VOPMex  
    NRTDRV  
    MoonDriver  
    MXP  
    MXDRV  
    MNDRV  
    MPCM  
    X68Sound  
    Shoot  
    XM6 TypeG  
    ASLPLAY  
    NAUDIO  
    VST.NET  
    NSFPlay  
    CVS.EXE  
    KeyboardHook3.cs  
    MUCOM88  
    MUCOM88windows  
    C86ctl source  
    MGSDRV  
    Z80dotNET  
    BlueMSX  
     
    SMS Power!  
    DOBON.NET  
    Wikipedia  
    GitHub  
    GitHub ・Nururi.  
    Gigamix Online  
    MSX Datapack wiki project  
    MSX Resource Center  
    msxnet  
    The link to Xyz's tweet (https://twitter.com/XyzGonGivItToYa/status/1216942514902634496?s=20)  
    Ganzu Work's Diary

[FAQ]  
  
  It doesn't start.  
  
    Case1  
      This is because a zone identifier has been added to the file, causing an error to occur during the boot process.  
    The zone identifier is one of the protection functions of the OS, and is automatically added to files downloaded from the Internet to prevent the execution of unintended files.  
    The zone identifier is automatically added to files downloaded from the Internet to prevent the execution of unintended files, but it can interfere with the intended download, as in this case.  
    →Double-click on the removeZoneIdent.bat file to run it.  
    This batch file will remove zone identifiers in bulk.  
    The following message will be displayed.  
        An unknown error has occurred.  
        Exception Message:  
        Could not load file or assembly  
        'file://.... .dll' or one of its dependencies,Operation is not supported.  
        (Exception from HRESULT:xxxx)  

    Case2  
      This error occurs mainly when the actual chip is used, because SCCI is in a state where it uses c86ctl.  
    Since MDPlayer also uses c86ctl, it will compete with SCCI and fail to start.  
    →Use scciconfig.exe to uncheck the "enable" option in the c86ctl configuration.  
  
    Case3  
      The version of .NETframework is different.  
    NETframework → Installing the latest version of .  
    NETframework.  
        An unknown error has occurred.  
        Exception Message:  
        Could not load file or assembly  
        'netstandard, Version=... , Culture=... , PublicKeyToken=...' or one of its dependencies. The specified file was not found.    

    CaseX  
      TBD  
  
  
  The tempo is not stable, the beginning of the song is not played at the beginning of the performance, or the song goes into fast forward.  
  
    Case1  
      This problem mainly occurs when using a real chip. The real chip takes a little time to process when the performance starts.  
    On the other hand, when the emulation starts playing, the process is completed immediately.  
    This is because the real chip tries to catch up with the emulation to close the time gap.  
    →In the "Options" window, click on the "Sound" tab, and uncheck the "opportunistic" checkbox in the lower left corner.  
  
    CaseX  
      TBD  
  
  The sound is choppy and choppy. Display is very heavy.  
  
    Case1  
    This problem occurs when all the necessary processing is not done within a limited time.  
    Please open the "Output" tab in the "Options" window and switch the device.  
    It depends on your environment, so we recommend you to try different devices.  
    Wasapi and ASIO tend to give good response.  
    Depending on the device, adjusting the "Latency (Rendering Buffer)" value may improve the results.  
  
    CaseX  
      TBD  
  
  
[Copyright and Disclaimer]  
  MDPlayer is licensed under the MIT License, see LICENSE.txt.  
  The copyright is owned by the author.  
  This software has no warranty, and the author will not be liable for any damage caused by using this software.  
  The author assumes no responsibility for any damages resulting from the use of this software.  
  The MIT license requires a copyright notice and this permission notice, which are not required for this software.  
  The source code of the following software has been ported and modified for C# or used as is.  
  These sources and software are copyrighted by their respective authors. As for the license, please refer to each document.  
  
  VGMPlay  
  MAME  
  DOSBOX  
  FMGen  
  NSFPlay  
  NEZ Plug  
  libsidplayfp  
  DOSBOX - FMGen - NSFPlay - NEZ Plug++ - libsidplayfp - sidplayfp  
  NRTDRV  
  MoonDriver  
  MXP  
  MXDRV  
  MNDRV  
  X68Sound  
  (Both m_puusan's/rurutan's version)  
  MUCOM88  
  MUCOM88windows(mucomDotNET)  
  M86(M86DotNET)  
  VST.NET  
  NAudio  
  SCCI  
  c86ctl  
  PMD(PMDDotNET)  
  MGSDRV  
  Z80dotNet  
  mucom88torym2612  

