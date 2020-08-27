@echo off
echo zip,dll,exeファイルのZone識別子を削除します。
pause

echo on
FOR %%a in (*.zip *.dll *.exe) do (echo . > %%a:Zone.Identifier)
FOR %%a in (ja\*.zip ja\*.dll ja\*.exe) do (echo . > %%a:Zone.Identifier)
FOR %%a in (plugin\driver\*.zip plugin\driver\*.dll plugin\driver\*.exe) do (echo . > %%a:Zone.Identifier)
@echo off

echo 完了しました。
pause
echo on
