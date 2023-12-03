# 実行ファイルのリスト生成

$files = Get-ChildItem .\mdplayer -Recurse -Include *.exe,*.dll

$cwd = (Get-Item .).FullName

$list = $files | ForEach-Object { "." + $_.FullName.Substring($cwd.Length) } 

$list
