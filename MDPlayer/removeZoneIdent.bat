@echo off
echo zip,dll,exeファイルのZone識別子を削除します。

cd /d "%~dp0"
if exist ".\MDPlayerx64.exe" ( 

	echo on

	DIR /B /S > dir.txt
	FOR /f %%a in (dir.txt) do (
	echo . > %%a:Zone.Identifier
	)
	DEL dir.txt

	@echo off
	echo 完了しました。
	echo on
	exit
) else (
	echo MDPlayerx64.exeが存在しないフォルダで実行されています。
	echo 実行を中止します。
	pause
	echo on
	exit
)
