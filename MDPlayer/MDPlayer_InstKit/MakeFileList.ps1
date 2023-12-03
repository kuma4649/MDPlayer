
# インストール、アンインストールするファイルのリストを出力する
$BinDir = (Get-Item .\mdplayer).FullName

$dirs = @($BinDir)
$dirs += Get-ChildItem .\mdplayer -Directory -Recurse | ForEach-Object { $_.FullName }

function MakeInstallList() {
    # 現在のディレクトリ
    $BaseDir = (Get-Location).Path + "\"

    foreach ($dir in $dirs) {
        $DirName = if ($dir.Length -gt $BinDir.Length) { $dir.Substring($BinDir.Length) } else { "" }
        "SetOutPath `"`$INSTDIR${DirName}`""
    
        $files = Get-ChildItem $dir -File | ForEach-Object { $_.FullName }
        $files | ForEach-Object { ".\" + $_.Substring($BaseDir.Length) } | ForEach-Object { "File `"$_`"" }
    }
}

function MakeUninstallList() {
    foreach ($dir in $dirs) {
        $files = Get-ChildItem $dir -File | ForEach-Object { $_.FullName }
        $files | ForEach-Object { $_.Substring($BinDir.Length+1) } | ForEach-Object { "Delete `"`$INSTDIR\$_`"" }
    }
    MakeUninstallDirList
}

function GetDirList($Path) {
    $dirs = Get-ChildItem $Path -Directory  | ForEach-Object { $_.FullName }

    foreach ($dir in $dirs) {
        GetDirList($dir)
    }
    $Path.Substring($BinDir.Length)
}
function GetUninstallDir() {
    GetDirList $BinDir
}

function MakeUninstallDirList() {
    GetUninstallDir | ForEach-Object { "RMDir `"`$INSTDIR$_`"" } 
}


MakeInstallList | Out-File -Encoding utf8NoBOM ".\00inst.nsh"
MakeUninstallList | Out-File -Encoding utf8NoBOM ".\01uninst.nsh"
#MakeInstallList | Out-File  ".\00inst.nsh"
#MakeUninstallList | Out-File  ".\01uninst.nsh"
