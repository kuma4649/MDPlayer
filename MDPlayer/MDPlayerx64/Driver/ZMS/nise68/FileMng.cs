using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer.Driver.ZMS.nise68
{
    public class FileMng
    {
        public string VCurrentPath;
        private string pDir;
        private string vDir;
        //private string crntDir;

        public Dictionary<string, vFileInfo> vDrive = new Dictionary<string, vFileInfo>();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="phisicalPath">物理のパス</param>
        /// <param name="virtualPath">仮想のパス</param>
        public FileMng(string phisicalPath, string virtualPath = "C:")
        {
            
            string p = phisicalPath.ToUpper();
            string v = virtualPath.ToUpper();
            if (p[p.Length - 1] == '\\') p = p.Substring(0, p.Length - 1);
            if (v[v.Length - 1] == '\\') v = v.Substring(0, v.Length - 1);

            this.pDir = p;//.Split('\\', StringSplitOptions.RemoveEmptyEntries);
            this.vDir = v;//.Split('\\', StringSplitOptions.RemoveEmptyEntries);
            this.VCurrentPath = v;
            vDrive.Clear();
        }

        public bool ExistsFile(string vFile)
        {
            //仮想ドライブにファイルがあるか確認する
            string vFull = Path.Combine(VCurrentPath, vFile).ToUpper();
            if (vDrive.ContainsKey(vFull)) return true;

            try
            {
                //仮想ドライブに存在しない場合は物理ドライブをチェックする
                string pFull = ConvertPhisicalFileName(vFull);
                return File.Exists(pFull);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 仮想ドライブにファイルを作ります
        /// </summary>
        /// <param name="vFilename">仮想ドライブ向けファイル名</param>
        /// <param name="body">ファイルの本体</param>
        public void SetVFile(string vFilename, byte[] body)
        {
            string vFull = Path.Combine(VCurrentPath, vFilename).ToUpper();
            if (vDrive.ContainsKey(vFull))
            {
                vDrive[vFull].Body = body;
            }
            else
            {
                vFileInfo fileInfo = new vFileInfo();
                fileInfo.Name = Path.GetFileName(vFull);
                fileInfo.Body = body;
                vDrive.Add(vFull, fileInfo);
            }
        }

        /// <summary>
        /// 仮想ドライブにファイルを作ります
        /// </summary>
        /// <param name="pFilename">物理ドライブからファイルを読み込んだ上で仮想ドライブのカレントにファイルをセットする</param>
        public void SetVFile(string pFilename)
        {
            byte[] body = File.ReadAllBytes(pFilename);
            SetVFile(Path.GetFileName(pFilename), body);
        }

        /// <summary>
        /// 仮想ドライブからファイルを丸ごと読みこむ
        /// 仮想ドライブにファイルが無い場合は物理ドライブから読み込む。
        /// このとき仮想ドライブにファイルを作る。
        /// </summary>
        /// <param name="vFilename"></param>
        /// <returns></returns>
        public byte[] VReadAllBytes(string vFilename)
        {
            //仮想ドライブにファイルがあるか確認する
            string vFull = Path.Combine(VCurrentPath, vFilename).ToUpper();
            if (vDrive.ContainsKey(vFull)) return vDrive[vFull].Body;

            try
            {
                //仮想ドライブに存在しない場合は物理ドライブをチェックする
                string pFull = ConvertPhisicalFileName(vFull);
                byte[] body;
                try
                {
                    if (File.Exists(pFull)) body = File.ReadAllBytes(pFull);
                    else body = null;
                }
                catch { body = null; }
                SetVFile(vFilename, body);
                return body;
            }
            catch
            {
                return null;
            }
        }

        public string VGetFullFilename(string vFilename)
        {
            string vFull = Path.Combine(VCurrentPath, vFilename).ToUpper();
            return vFull;
        }

        private string ConvertPhisicalFileName(string vFull)
        {
            string vPath = Path.GetDirectoryName(vFull);
            if (vPath.IndexOf(vDir) != 0)
            {
                if (vPath != "\\")
                {
                    throw new ArgumentOutOfRangeException("範囲外のパスを参照しています");
                }
            }
            string pFull;
            if (vPath != "\\")
                pFull = Path.Combine(vPath.Replace(vDir, pDir),Path.GetFileName(vFull));
            else
            {
                pFull = Path.Combine(pDir, Path.GetFileName(vFull));

            }

            return pFull;
        }
    }

    public class vFileInfo
    {
        public string Name;
        public byte[] Body;
    }
}
