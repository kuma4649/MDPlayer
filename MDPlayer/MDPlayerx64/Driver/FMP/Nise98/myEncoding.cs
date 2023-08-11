using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer.Driver.FMP.Nise98
{
    public class myEncoding : iEncoding
    {
        private static Lazy<myEncoding> defaultEncoding;
        private Encoding sjis;

        static myEncoding()
        {
            defaultEncoding = new Lazy<myEncoding>(() => new myEncoding(), true);
        }

        public myEncoding()
        {
            try
            {
                sjis = Encoding.GetEncoding("shift_jis");
            }
            catch
            {
                sjis = Encoding.UTF8;
            }
        }

        public static iEncoding Default => defaultEncoding.Value;

        public byte[] GetSjisArrayFromString(string utfString) => sjis.GetBytes(utfString);
        public string GetStringFromSjisArray(byte[] sjisArray) => sjis.GetString(sjisArray);
        public string GetStringFromSjisArray(byte[] sjisArray, int index, int count) => sjis.GetString(sjisArray, index, count);
        public string GetStringFromUtfArray(byte[] utfArray) => Encoding.UTF8.GetString(utfArray);
        public byte[] GetUtfArrayFromString(string utfString) => Encoding.UTF8.GetBytes(utfString);
    }
}
