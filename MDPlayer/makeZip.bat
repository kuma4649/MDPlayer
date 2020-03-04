echo MDPlayer

mkdir .\output
del /Q .\output\*.*
mkdir .\output\licenses
del /Q .\output\licenses\*.*

xcopy .\mdpc\bin\Release\*.* .\output\ /E /R /Y /I /K
xcopy .\MDPlayer\bin\x86\Release\*.* .\output\ /E /R /Y /I /K
xcopy ..\licenses .\output\licenses\ /E /R /Y /I /K
copy /Y .\..\CHANGE.txt .\output
copy /Y .\..\LICENSE.txt .\output
copy /Y .\..\README.md .\output

del /Q .\output\*.pdb
del /Q .\output\*.config
del /Q .\output\*.bat
rem del /Q .\output\bin.zip

pause
