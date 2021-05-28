@echo off

rem 注意                                                 2> nul
rem フォルダ名に漢字とかある場合は                       2> nul
rem 文字コードがUTF8のファイルを作る必要がありますので、 2> nul
rem 下のコマンドを入力してUTF8モードに切り替えてください 2> nul
rem chcp 65001                                           2> nul
rem S-JISに戻すときは                                    2> nul
rem chcp 932                                             2> nul

rem 操作ファイル作成                                     2> nul
echo %1 %2 > %appdata%\KumaApp\MDPlayer\operation\ope.txt
rem キャッシュをフラッシュ                               2> nul
dir %appdata%\KumaApp\MDPlayer\operation\ope.txt   > nul 2> nul
echo on
