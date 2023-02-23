@echo off
echo zip,dll,exeファイルのZone識別子を削除します。

echo on

DIR /B /S > dir.txt
FOR /f %%a in (dir.txt) do (
echo . > %%a:Zone.Identifier
)
DEL dir.txt

@echo off
echo 完了しました。
echo on
