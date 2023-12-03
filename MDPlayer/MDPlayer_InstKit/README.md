# インストーラ作成キット v1.0

## 手順

1. bin.zipの内容をmdplayerディレクトリとして展開
2. personal.pfxを作成
3. 署名機能は環境によって動作しないので、バッチの中のパスを調整
4. NSISで_inst_mui2.nsiをコンパイル

## ディレクトリの説明
* `mdplayer`
binファイルの中身と同様です。

例としてMDPlayerの位置:
`mdplayer\MDPlayerx64.exe`

* `images`
画像リソースです。

## ファイルリストの作成

`MakeFileList.ps1`
で次のスクリプトを作成できます。

`00inst.nsh`
インストール用のファイルリスト

`01uninst.nsh`
アンインストール用のファイルリスト


## 署名用ファイルの作成

personal.pfxを作成します。
opensslなどを利用して作成します。
pfxファイルは秘密鍵を含むので取り扱いには注意してください。

以下は一例です。

```sh
# 秘密鍵
openssl genrsa -out private.key

# 署名リクエスト
openssl req -new -sha256 -key private.key -out request.csr

# 証明書
openssl x509 -req -in request.csr -signkey private.key -out cert.crt -days 3650

# pfxファイル
openssl pkcs12 -export -inkey private.key -in cert.crt -out personal.pfx
```

## 署名機能に関して

署名機能はWindows SDKにあるsigntool.exeを使用しています。
SignExe.bat内のパスはWindows SDKのバージョンによって調整してください。

## ファイル署名機能

`SignMDPlayer.ps1`でmdplayer以下のファイルを署名可能です。
ファイルリストはスクリプト内に書かれていて、`MakeExeList.ps1`で
その原型は取得できます。

`SignMDPlayer.ps1`はWindows SDKが入ったVisual StudioのPowerShellで実行します。
例: Developer PowerShell for VS 2022

## インストーラ署名機能

NSISコンパイル時に`SignExe.bat`で自動的に署名します。
SignExe.bat内の`signtool.exe`のパスは調整してください。

## コンパイル

NSIS v3.08で`_inst_mui2.nsi`をコンパイルします。




