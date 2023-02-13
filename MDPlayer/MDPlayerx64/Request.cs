using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer
{
    public class Request
    {
        public enmRequest request;
        public object[] args;
        public object[] results;
        public Action<object> callBack;

        private object objlock = new object();
        private bool _end = false;
        public bool end
        {
            get
            {
                lock (objlock) return _end;
            }
            set
            {
                lock (objlock) _end = value;
            }
        }

        public Request(enmRequest req, object[] args = null, Action<object> callBack = null)
        {
            request = req;
            this.args = args;
            this.callBack = callBack;
        }

    }

    public enum enmRequest
    {
        Die,
        GetStatus,
        Stop,
        Play,
        Pause,
        Fadeout
    }
}
