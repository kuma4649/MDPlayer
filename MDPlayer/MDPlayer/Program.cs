using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using MDPlayer.form;

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

            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new frmMain());
            }
            catch (ObjectDisposedException )
            {
                ;//無視する
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("不明なエラーが発生しました。\nException Message:\n{0}", e.Message), "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        static string checkFiles()
        {
            string fn;

            fn = "MDSound.dll";
            if (!System.IO.File.Exists(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Application.ExecutablePath),fn))) return fn;
            fn = "NAudio.dll";
            if (!System.IO.File.Exists(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Application.ExecutablePath), fn))) return fn;
            fn = "NScci.dll";
            if (!System.IO.File.Exists(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Application.ExecutablePath), fn))) return fn;
            fn = "scci.dll";
            if (!System.IO.File.Exists(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Application.ExecutablePath), fn))) return fn;
            fn = "Jacobi.Vst.Core.dll";
            if (!System.IO.File.Exists(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Application.ExecutablePath), fn))) return fn;
            fn = "Jacobi.Vst.Interop.dll";
            if (!System.IO.File.Exists(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Application.ExecutablePath), fn))) return fn;

            return null;
        }
    }
}
