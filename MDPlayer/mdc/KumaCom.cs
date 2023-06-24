using System;

namespace mdc
{
    public class KumaCom
    {
        public KumaCom()
        {

        }

        public virtual bool Init(string a, string p)
        {
            return true;
        }
        public virtual bool Open(string mmfName, int mmfSize)
        {
            return false;
        }

        public virtual void Close()
        {
        }

        public virtual string GetMessage()
        {
            return "";
        }

        public virtual void SendMessage(string msg)
        {
            return;
        }

        public virtual void SetBytes(byte[] buf)
        {
            return;
        }

        public virtual byte[] GetBytes()
        {
            throw new NotImplementedException();
        }
    }
}
