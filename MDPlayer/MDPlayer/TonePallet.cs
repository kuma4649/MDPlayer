using System;
using System.Collections.Generic;
using System.Text;

namespace MDPlayer
{
    [Serializable]
    public class TonePallet
    {

        private List<Tone> _lstTone = new List<Tone>(256);
        public List<Tone> lstTone
        {
            get
            {
                return _lstTone;
            }

            set
            {
                _lstTone = value;
            }
        }

        public TonePallet Copy()
        {
            TonePallet TonePallet = new TonePallet();

            return TonePallet;
        }

        public void Save(string fileName)
        {
            string fullPath = "";

            if (fileName == null || fileName == "")
            {
                fullPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                fullPath = System.IO.Path.Combine(fullPath, "KumaApp", common.AssemblyTitle);
                if (!System.IO.Directory.Exists(fullPath)) System.IO.Directory.CreateDirectory(fullPath);
                fullPath = System.IO.Path.Combine(fullPath, "DefaultTonePallet.xml");
            }
            else
            {
                fullPath = fileName;
            }

            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(TonePallet));
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(fullPath, false, new UTF8Encoding(false)))
            {
                serializer.Serialize(sw, this);
            }
        }

        public static TonePallet Load(string fileName)
        {
            try
            {
                string fullPath = "";
                if (fileName == null || fileName == "")
                {
                    fullPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                    fullPath = System.IO.Path.Combine(fullPath, "KumaApp", common.AssemblyTitle);
                    if (!System.IO.Directory.Exists(fullPath)) System.IO.Directory.CreateDirectory(fullPath);
                    fullPath = System.IO.Path.Combine(fullPath, "DefaultTonePallet.xml");
                }
                else
                {
                    fullPath = fileName;
                }

                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(TonePallet));
                using (System.IO.StreamReader sr = new System.IO.StreamReader(fullPath, new UTF8Encoding(false)))
                {
                    TonePallet pl = (TonePallet)serializer.Deserialize(sr);
                    return pl;
                }
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
                return new TonePallet();
            }
        }


    }
}
