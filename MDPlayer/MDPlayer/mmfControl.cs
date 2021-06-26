using System;
using System.IO.MemoryMappedFiles;
using System.Security.AccessControl;
using System.Text;


namespace MDPlayer
{
    public class mmfControl
    {
        private object lockobj = new object();
        private MemoryMappedFile _map;
        private byte[] mmfBuf;
        public string mmfName = "dummy";
        public int mmfSize = 1024;

        public mmfControl()
        {
        }

        public mmfControl(bool isClient, string mmfName, int mmfSize)
        {
            this.mmfName = mmfName;
            this.mmfSize = mmfSize;
            if (!isClient) Open(mmfName, mmfSize);
        }

        public void Open(string mmfName, int mmfSize)
        {
            mmfBuf = new byte[mmfSize];

            lock (lockobj)
            {
                _map = MemoryMappedFile.CreateNew(mmfName, mmfSize);
                MemoryMappedFileSecurity permission = _map.GetAccessControl();
                permission.AddAccessRule(
                  new AccessRule<MemoryMappedFileRights>("Everyone",
                    MemoryMappedFileRights.FullControl, AccessControlType.Allow));
                _map.SetAccessControl(permission);
            }
        }

        public void Close()
        {
            lock (lockobj)
            {
                if (_map == null) return;
                _map.Dispose();
            }
        }

        public string GetMessage()
        {
            string msg = "";

            lock (lockobj)
            {
                using (MemoryMappedViewAccessor view = _map.CreateViewAccessor())
                {
                    view.ReadArray(0, mmfBuf, 0, mmfBuf.Length);
                    msg = Encoding.Unicode.GetString(mmfBuf);
                    msg = msg.Substring(0, msg.IndexOf('\0'));
                    Array.Clear(mmfBuf, 0, mmfBuf.Length);
                    view.WriteArray(0, mmfBuf, 0, mmfBuf.Length);
                }
            }

            return msg;
        }

        public void SendMessage(string msg)
        {
            byte[] ary = Encoding.Unicode.GetBytes(msg);
            if (ary.Length > mmfSize) throw new ArgumentOutOfRangeException();

            using (var map = MemoryMappedFile.OpenExisting(mmfName))
            using (var view = map.CreateViewAccessor())
                view.WriteArray(0, ary, 0, ary.Length);
        }

    }
}
