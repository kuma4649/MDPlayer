@echo off
echo MDPlayer

mkdir  .\output
del /Q .\output\*.*
mkdir  .\output\licenses
del /Q .\output\licenses\*.*
mkdir  .\output\plugin
del /Q .\output\plugin\*.*
mkdir  .\output\plugin\driver
del /Q .\output\plugin\driver\*.*
mkdir  .\output\FileAssociationTool
del /Q .\output\FileAssociationTool\*.*
mkdir  .\output\FileAssociationTool\ico
del /Q .\output\FileAssociationTool\ico\*.*

xcopy   .\mdc\bin\Release\*.*          .\output\ /E /R /Y /I /K
xcopy   .\mdpc\bin\Release\net8.0-windows7.0\*.*         .\output\ /E /R /Y /I /K
xcopy   .\MDPlayerx64\bin\x64\Release\net8.0-windows7.0\*.*  .\output\ /E /R /Y /I /K
xcopy   .\FileAssociationTool\bin\Release\net8.0-windows\*.* .\output\FileAssociationTool\ /E /R /Y /I /K
xcopy   ..\licenses                       .\output\licenses\ /E /R /Y /I /K
copy /Y .\MDPlayerx64\lib\scci*.*         .\output
copy /Y .\MDPlayerx64\lib\c86ctl.*        .\output
copy /Y .\MDPlayerx64\lib\MGSDRV.COM      .\output
copy /Y .\MDPlayerx64\lib\KINROU*.*       .\output
copy /Y .\MDPlayerx64\plugin\*.*          .\output\plugin
copy /Y .\MDPlayerx64\plugin\driver\*.*   .\output\plugin\driver
copy /Y .\..\CHANGE.txt                   .\output
copy /Y .\..\LICENSE.txt                  .\output
copy /Y .\..\README.md                    .\output
copy /Y .\README_EN.md                    .\output
copy /Y .\..\README_AST.md                .\output
copy /Y .\..\README_AST_EN.md             .\output
cd

del /Q .\output\2608_bd.wav
del /Q .\output\2608_hh.wav
del /Q .\output\2608_rim.wav
del /Q .\output\2608_sd.wav
del /Q .\output\2608_tom.wav
del /Q .\output\2608_top.wav
del /Q .\output\FMC.EXE
del /Q .\output\FMP.COM
del /Q .\output\PPZ8.COM
del /Q .\output\KINROU4.COM
del /Q .\output\KINROU5.DRV
del /Q .\output\ZMC.X
del /Q .\output\ZMSC3.X
del /Q .\output\ZMUSIC.X
del /Q .\output\Z*.X
del /Q .\output\LZZ.R

del /Q .\output\*.pdb
rem del /Q .\output\*.config
del /Q .\output\*.bat
del /Q .\output\FileAssociationTool\*.pdb

del /Q .\output\scci.ini
del /Q .\output\scci.dll
del /Q .\output\scciconfig.exe
del /Q .\output\lib\scci.ini
del /Q .\output\lib\scci.dll
del /Q .\output\lib\scciconfig.exe

rem del /Q .\output\scci2.ini
rem del /Q .\output\scci2.dll
rem del /Q .\output\scci2config.exe
rem del /Q .\output\lib\scci2.ini
rem del /Q .\output\lib\scci2.dll
rem del /Q .\output\lib\scci2config.exe

rem del /Q .\output\MGSDRV.COM
copy /Y .\removeZoneIdent.bat   .\output
copy /Y .\mdp.bat   .\output
rmdir /S /Q .\output\deploy
rmdir /S /Q .\output\net8.0-windows7.0
rmdir /S /Q .\output\ref
rmdir /S /Q .\output\runtimes
rem -- withoutVST    
Xcopy   .\MDPlayer\*.*                                                         ..\..\MDPlayerWithoutVST\MDPlayer\MDPlayer\ /E /R /Y /I /K
copy /Y ..\..\MDPlayerWithoutVST\MDPlayer\MDPlayerBackUp\VST\*.*               ..\..\MDPlayerWithoutVST\MDPlayer\MDPlayer\VST
copy /Y ..\..\MDPlayerWithoutVST\MDPlayer\MDPlayerBackUp\App.config            ..\..\MDPlayerWithoutVST\MDPlayer\MDPlayer
copy /Y ..\..\MDPlayerWithoutVST\MDPlayer\MDPlayerBackUp\MDPlayer.csproj       ..\..\MDPlayerWithoutVST\MDPlayer\MDPlayer
copy /Y ..\..\MDPlayerWithoutVST\MDPlayer\MDPlayerBackUp\MDPlayer.csproj.user  ..\..\MDPlayerWithoutVST\MDPlayer\MDPlayer

del /Q .\output\scci.ini
del /Q .\output\scci.dll
del /Q .\output\scciconfig.exe
del /Q .\output\lib\scci.ini
del /Q .\output\lib\scci.dll
del /Q .\output\lib\scciconfig.exe
del /Q .\output\scci2.ini
del /Q .\output\lib\scci2.ini

echo ----------------------------------------
echo     
echo withoutVST ̃  r   h  s   Ă       
echo  \     [ V     ̍\     ς     ꍇ  csproj Ȃǂ̍\   t @ C    C      K v      ܂ 
echo  X ɏC       \   t @ C    MDPlayerBackUp ɃR s [   Ă    K v      ܂ 
echo ----------------------------------------

pause
echo on
