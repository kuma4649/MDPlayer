@REM 環境に依存
SET SIGNTOOL="C:\Program Files (x86)\Windows Kits\10\bin\10.0.19041.0\x86\signtool.exe"

%SIGNTOOL% sign /f personal.pfx /p kuma /fd SHA256 %1

