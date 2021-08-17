using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using MDPlayer.form;
using System.IO;

namespace MDPlayer
{
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            string fn = checkFiles();
            if (fn != null)
            {
                MessageBox.Show(string.Format("動作に必要なファイル({0})がみつかりません。", fn), "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            frmMain frm=null;
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                frm = new frmMain();
                Application.Run(frm);
            }
            catch(InvalidOperationException)
            {
                ;
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("不明なエラーが発生しました。\nException Message:\n{0}", e.Message), "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        static string checkFiles()
        {
            List<string> chkFn = new List<string>()
            {
                "MDSound.dll"
                , "NAudio.dll"
                , "RealChipCtlWrap.dll"
                , "scci.dll"
                , "c86ctl.dll"
            };
            chkFn.AddRange(vstMng.chkFn);

            foreach (string fn in chkFn)
            {
                if (!File.Exists(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), fn))) return fn;
            }

            return null;
        }




    }
}
