using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Driver
{
    public class Ptr<T>
    {
        public T[] buf;
        public int ptr;

        public Ptr(int size)
        {
            buf = new T[size];
            ptr = 0;
        }

        public Ptr(T[] buf, int ptr)
        {
            this.buf = buf;
            this.ptr = ptr;
        }

        public T this[int i]
        {
            set {
                buf[ptr + i] = value;
            }
            get {
                return buf[ptr + i];
            }
        }

        public void AddPtr(int a)
        {
            ptr += a;
        }

        public string ToString(int len)
        {
            string ret = "";
            for(int i = 0; i < len; i++)
            {
                if (ptr + i < buf.Length) ret += buf[ptr + i];
            }

            return ret;
        }

        public static Ptr<byte> strchr(Ptr<byte> src, byte c)
        {
            int ptr = 0;
            for (int i = 0; i < src.buf.Length; i++)
            {
                if (src.buf[i] == c)
                {
                    ptr = i;
                    return new Ptr<byte>(src.buf, ptr);
                }
            }
            return null;
        }

        public static Ptr<byte> strrchr(Ptr<byte> src, byte c)
        {
            int ptr = 0;
            for (int i = src.buf.Length - 1; i >= src.ptr; i--)
            {
                if (src.buf[i] == c)
                {
                    ptr = i;
                    return new Ptr<byte>(src.buf, ptr);
                }
            }
            return null;
        }

        public static Ptr<byte> strstr(Ptr<byte> src, string s)
        {
            string str = Encoding.ASCII.GetString(src.buf, src.ptr, src.buf.Length - src.ptr);
            int ind = str.IndexOf(s);
            if (ind == -1) return null;

            return new Ptr<byte>(src.buf, ind);
        }

        public Int32 Count
        {
            get
            {
                return buf.Length - ptr;
            }
        }
    }
}
