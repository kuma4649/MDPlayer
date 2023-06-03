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

xcopy   .\mdc\bin\Release\*.*          .\output\ /E /R /Y /I /K
xcopy   .\mdpc\bin\Release\*.*         .\output\ /E /R /Y /I /K
xcopy   .\MDPlayerx64\bin\x64\Release\net6.0-windows\*.* .\output\ /E /R /Y /I /K
xcopy   ..\licenses                    .\output\licenses\ /E /R /Y /I /K
copy /Y .\MDPlayerx64\lib\scci*.*         .\output
copy /Y .\MDPlayerx64\lib\c86ctl.*        .\output
copy /Y .\MDPlayerx64\lib\MGSDRV.COM      .\output
copy /Y .\MDPlayerx64\lib\KINROU*.*       .\output
copy /Y .\MDPlayerx64\plugin\*.*          .\output\plugin
copy /Y .\MDPlayerx64\plugin\driver\*.*   .\output\plugin\driver
copy /Y .\..\CHANGE.txt                .\output
copy /Y .\..\LICENSE.txt               .\output
copy /Y .\..\README.md                 .\output
cd

del /Q .\output\*.pdb
rem del /Q .\output\*.config
del /Q .\output\*.bat
rem del /Q .\output\MGSDRV.COM
copy /Y .\removeZoneIdent.bat   .\output
copy /Y .\mdp.bat   .\output
rmdir /S /Q .\output\deploy
rmdir /S /Q .\output\net6.0-windows
rmdir /S /Q .\output\ref
rmdir /S /Q .\output\runtimes
rem -- withoutVST向け
Xcopy   .\MDPlayer\*.*                                                         ..\..\MDPlayerWithoutVST\MDPlayer\MDPlayer\ /E /R /Y /I /K
copy /Y ..\..\MDPlayerWithoutVST\MDPlayer\MDPlayerBackUp\VST\*.*               ..\..\MDPlayerWithoutVST\MDPlayer\MDPlayer\VST
copy /Y ..\..\MDPlayerWithoutVST\MDPlayer\MDPlayerBackUp\App.config            ..\..\MDPlayerWithoutVST\MDPlayer\MDPlayer
copy /Y ..\..\MDPlayerWithoutVST\MDPlayer\MDPlayerBackUp\MDPlayer.csproj       ..\..\MDPlayerWithoutVST\MDPlayer\MDPlayer
copy /Y ..\..\MDPlayerWithoutVST\MDPlayer\MDPlayerBackUp\MDPlayer.csproj.user  ..\..\MDPlayerWithoutVST\MDPlayer\MDPlayer

echo ----------------------------------------
echo 注意
echo withoutVSTのリビルドを行ってください
echo ソリューションの構成が変わった場合はcsprojなどの構成ファイルを修正する必要があります
echo 更に修正した構成ファイルをMDPlayerBackUpにコピーしておく必要があります
echo ----------------------------------------

pause
echo on
