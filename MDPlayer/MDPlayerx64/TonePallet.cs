using System.Text;

namespace MDPlayer
{
    [Serializable]
    public class TonePallet
    {

        private List<Tone> _lstTone = new(256);
        public List<Tone> LstTone
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

        public static TonePallet Copy()
        {
            TonePallet TonePallet = new();

            return TonePallet;
        }

        public void Save(string fileName)
        {
            string fullPath;
            if (fileName == null || fileName == "")
            {
                fullPath = Common.settingFilePath;
                fullPath = Path.Combine(fullPath, "DefaultTonePallet.xml");
            }
            else
            {
                fullPath = fileName;
            }

            System.Xml.Serialization.XmlSerializer serializer = new(typeof(TonePallet), typeof(TonePallet).GetNestedTypes());
            using StreamWriter sw = new(fullPath, false, new UTF8Encoding(false));
            serializer.Serialize(sw, this);
        }

        public static TonePallet Load(string fileName)
        {
            try
            {
                string fullPath = "";
                if (fileName == null || fileName == "")
                {
                    fullPath = Common.settingFilePath;
                    fullPath = Path.Combine(fullPath, "DefaultTonePallet.xml");
                }
                else
                {
                    fullPath = fileName;
                }

                System.Xml.Serialization.XmlSerializer serializer = new(typeof(TonePallet), typeof(TonePallet).GetNestedTypes());
                using System.IO.StreamReader sr = new(fullPath, new UTF8Encoding(false));
                TonePallet pl = (TonePallet)serializer.Deserialize(sr);
                return pl;
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
                return new TonePallet();
            }
        }


    }
}
