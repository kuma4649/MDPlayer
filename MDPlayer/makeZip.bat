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

del /Q .\output\*.pdb
del /Q .\output\*.config
del /Q .\output\*.bat
rem del /Q .\output\bin.zip

pause
