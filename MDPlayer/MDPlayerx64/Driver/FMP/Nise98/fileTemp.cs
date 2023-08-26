using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer.Driver.FMP.Nise98
{
    public class fileTemp
    {
        private Dictionary<string, byte[]> temp = new Dictionary<string, byte[]>();
        private Setting setting;

        public fileTemp(Setting setting)
        {
            this.setting = setting;
        }

        public void WriteTemp(string filename, byte[] data)
        {
            if (temp.ContainsKey(filename.ToUpper()))
            {
                temp.Remove(filename.ToUpper());
            }
            temp.Add(filename.ToUpper(), data);

            if (!setting.other.SaveCompiledFile) return;

            try
            {
                File.WriteAllBytes(filename, data);
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
        }

        public byte[] ReadTemp(string filename) 
        {
            if(!temp.ContainsKey(filename.ToUpper()))
            {
                return null;
            }
            return temp[filename.ToUpper()]; 
        }

        public bool ExistTemp(string filename)
        {
            return temp.ContainsKey(filename.ToUpper());
        }

    }
}
