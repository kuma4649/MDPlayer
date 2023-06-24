using MDPlayer.form;

namespace MDPlayerx64
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            string fn = CheckFiles();
            if (fn != null)
            {
                MessageBox.Show(string.Format("動作に必要なファイル({0})がみつかりません。", fn), "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                frmMain frm = new();
                Application.Run(frm);
            }
            catch (InvalidOperationException)
            {
                ;
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("不明なエラーが発生しました。\nException Message:\n{0}", e.Message), "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        static string CheckFiles()
        {
            List<string> chkFn = new()
            {
                "MDSound.dll"
                , "NAudio.dll"
                , "RealChipCtlWrap64.dll"
                , "lib\\scci.dll"
                , "lib\\c86ctl.dll"
            };
            chkFn.AddRange(MDPlayer.vstMng.chkFn);

            foreach (string fn in chkFn)
            {
                if (!File.Exists(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), fn))) return fn;
            }

            return null;
        }
    }
}