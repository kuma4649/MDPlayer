rem @echo off
echo MDPlayer

SET /P version="Version”Ô†‚ð“ü—Í‚µ‚Ä‚­‚¾‚³‚¢(STBLxxx):"
echo STBL%version% > .\output\VERSION.txt

rmdir /Q /S      .\MDPlayer_InstKit\mdplayer\
mkdir            .\MDPlayer_InstKit\mdplayer\
xcopy .\output\  .\MDPlayer_InstKit\mdplayer\ /E /R /Y /I /K
del              .\MDPlayer_InstKit\mdplayer\bin.zip
cd               .\MDPlayer_InstKit

rem "C:\Windows\SysWOW64\WindowsPowerShell\v1.0\powershell.exe" -noe -c "&{ Set-Location C:\Users\maryb\source\repos\MDPlayer\MDPlayer ; Compress-Archive -Path output -DestinationPath bin.zip -Force ; exit ;}"
"C:\Program Files\PowerShell\7\pwsh.exe" -WorkingDirectory ~ -command "& { Set-Location C:\Users\maryb\source\repos\MDPlayer\MDPlayer\MDPlayer_InstKit ; ./MakeFileList.ps1 ; exit ; }"

"C:\Windows\SysWOW64\WindowsPowerShell\v1.0\powershell.exe" -noe -c "&{Import-Module """C:\Program Files\Microsoft Visual Studio\2022\Community\Common7\Tools\Microsoft.VisualStudio.DevShell.dll"""; Enter-VsDevShell c4d75d87;Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass; Set-Location C:\Users\maryb\source\repos\MDPlayer\MDPlayer\MDPlayer_InstKit ; ./SignMDPlayer.ps1 ; exit ;}"


rem "C:\Windows\SysWOW64\WindowsPowerShell\v1.0\powershell.exe" -noe -c "Import-Module 'C:\Program Files\Microsoft Visual Studio\18\Community\Common7\Tools\Microsoft.VisualStudio.DevShell.dll'; Enter-VsDevShell -SkipInstallationCheck; Set-Location C:\Users\maryb\source\repos\MDPlayer\MDPlayer_InstKit; ./SignMDPlayer.ps1; exit;"

del 02info.nsh
setlocal enabledelayedexpansion
for /f "delims=" %%X in (02info.nsh.T) do (
  SET lin0=%%X
  SET lin1=!lin0:VVEE=%version%!
  echo !lin1!>>02info.nsh
)
endlocal

"C:\Program Files (x86)\NSIS\makensis" ./_inst_mui2.nsi

copy MDPlayer-STBL*.exe .\..\
del  /Q MDPlayer-STBL*.exe

cd               .\..\
pause
echo on
