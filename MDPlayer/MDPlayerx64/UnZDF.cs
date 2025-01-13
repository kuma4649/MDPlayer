
using MDPlayer.Driver.ZMS.nise68;
using MDSound;
using System.Collections.Generic;
using System.Formats.Tar;

namespace MDPlayer
{
    public class UnZDF
    {
        public List<Tuple<string, ulong>> GetFileList(string arcFile, string v)
        {
            FileMng fileMng = Unpack(arcFile);
            if (fileMng == null) return null;

            List<Tuple<string, ulong>> res = new List<Tuple<string, ulong>>();
            foreach (var ele in fileMng.vDrive)
            {
                if (ele.Value.Name.ToUpper() == "LZZ.R") continue;
                if (ele.Value.Name.ToUpper() == Path.GetFileName(arcFile).ToUpper()) continue;

                res.Add(new Tuple<string, ulong>(ele.Value.Name, (uint)ele.Value.Body.Length));
            }

            return res;
        }

        public byte[] GetFileByte(string arcFile, string dstFile)
        {
            FileMng fileMng = Unpack(arcFile);
            if(fileMng == null) return null;

            foreach (var ele in fileMng.vDrive)
            {
                if (ele.Value.Name.ToUpper() != Path.GetFileName(dstFile).ToUpper()) continue;
                return ele.Value.Body;
            }
            return null;
        }

        public FileMng Unpack(string arcFile)
        {
            List<Tuple<string, ulong>> res = new List<Tuple<string, ulong>>();
            string crntDir = Path.GetDirectoryName(Application.ExecutablePath);
            string lzz = Path.Combine(crntDir, "lzz.r");
            if (!File.Exists(lzz)) return null;

            string? dn = Path.GetDirectoryName(arcFile);
            FileMng fileMng = new FileMng(dn);
            fileMng.SetVFile(lzz);
            lzz = Path.GetFileName(lzz);

            MDPlayer.Driver.ZMS.nise68.Log.SetMsgWrite(MsgWrite);
            nise68 nise68;
            nise68 = new nise68();
            nise68.Init(null, false, fileMng);

            int rc;
            if ((rc = nise68.LoadRun(lzz, "-E " + Path.GetFileName(arcFile), 0x00033c00
            , true, true, true
            )) != 0) return null;

            return fileMng;
        }

        private void MsgWrite(string arg1, object[] arg2)
        {
            log.Write(LogLevel.Information, arg1, arg2);
        }

    }
}