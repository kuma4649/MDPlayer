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

xcopy   .\mdpc\bin\Release\*.*         .\output\ /E /R /Y /I /K
xcopy   .\MDPlayer\bin\x86\Release\*.* .\output\ /E /R /Y /I /K
xcopy   ..\licenses                    .\output\licenses\ /E /R /Y /I /K
copy /Y .\MDPlayer\lib\scci*.*         .\output
copy /Y .\MDPlayer\lib\c86ctl.*        .\output
copy /Y .\MDPlayer\plugin\*.*          .\output\plugin
copy /Y .\MDPlayer\plugin\driver\*.*   .\output\plugin\driver
copy /Y .\..\CHANGE.txt                .\output
copy /Y .\..\LICENSE.txt               .\output
copy /Y .\..\README.md                 .\output
cd

del /Q .\output\*.pdb
del /Q .\output\*.config
del /Q .\output\*.bat
copy /Y MDPlayer\removeZoneIdent.bat   .\output

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
echo ----------------------------------------

pause
echo on
