
# MUI2を使用

!include "MUI2.nsh"
!include "LogicLib.nsh"

# 情報マクロ
!include "02info.nsh"

# 基本設定
Name "${PACKAGE}"
OutFile "${NAME}-${VERSION}.exe"

InstallDir "C:\MDPlayer"

# 変数
!define RegistryKey "Software\Microsoft\Windows\CurrentVersion\Uninstall\MDPlayer"
!define EXEFILE "$INSTDIR\MDPlayerx64.exe"

# 画像
# 164x314
!define MUI_WELCOMEFINISHPAGE_BITMAP "images\welcome.bmp"
!define MUI_WELCOMEFINISHPAGE_BITMAP_STRETCH AspectFitHeight
!define MUI_ICON "images/Feli.ico"

# ページ設定
!define MUI_ABORTWARNING

!insertmacro MUI_PAGE_WELCOME
!insertmacro MUI_PAGE_LICENSE "mdplayer\LICENSE.txt"

Page custom test
Function test
  SetRegView 64
  ReadRegStr $0 HKLM ${RegistryKey} "InstallDir"
#  MessageBox MB_OK  "Install directory at: $0"
  ${If} $0 != ""
    StrCpy $INSTDIR "$0"
  ${EndIf}
FunctionEnd

!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_PAGE_FINISH

!insertmacro MUI_UNPAGE_WELCOME
!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES

# 言語ファイル
!insertmacro MUI_LANGUAGE "English"
!insertmacro MUI_LANGUAGE "Japanese"

# インストール
Section "Install" SECTION_INSTALL

  !include 00inst.nsh

  WriteUninstaller "$INSTDIR\Uninstall.exe"

  # スタート メニューにショートカットを登録
  CreateDirectory "$SMPROGRAMS\MDPlayer"
  SetOutPath "$INSTDIR"
  CreateShortcut "$SMPROGRAMS\MDPlayer\MDPlayer.lnk" "${EXEFILE}" ""

  SectionGetSize "${SECTION_INSTALL}" $0


  # アンインストール情報をレジストリに登録
  WriteRegStr HKLM ${RegistryKey} "InstallDir" "$INSTDIR"
  WriteRegStr HKLM ${RegistryKey} "DisplayName" "${NAME}"
  WriteRegStr HKLM ${RegistryKey} "UninstallString" '"$INSTDIR\Uninstall.exe"'
  WriteRegStr HKLM ${RegistryKey} "Publisher" '"${PUBLISHER}"'
  WriteRegStr HKLM ${RegistryKey} "DisplayVersion" '"${VERSION}"'
  WriteRegStr HKLM ${RegistryKey} "DisplayIcon" '"${EXEFILE}"'
  WriteRegDWORD HKLM "${RegistryKey}" "EstimatedSize" "$0"
  WriteRegDWORD HKLM "${RegistryKey}" "NoModify" "1"
  WriteRegDWORD HKLM "${RegistryKey}" "NoRepair" "1"

  ExecWait '"$INSTDIR\removeZoneIdent.bat"'

SectionEnd

# アンインストーラ
Section "Uninstall"
  Delete "$INSTDIR\Uninstall.exe"
  !include 01uninst.nsh

  Delete "$SMPROGRAMS\MDPlayer\MDPlayer.lnk"
  RMDir "$SMPROGRAMS\MDPlayer"

  # レジストリ キーを削除
  DeleteRegKey HKLM ${RegistryKey}
SectionEnd

# 署名する
!finalize 'SignExe.bat "%1"'
!uninstfinalize 'SignExe.bat "%1"'
