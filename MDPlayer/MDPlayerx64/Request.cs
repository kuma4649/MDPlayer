namespace MDPlayer
{
    public class Request
    {
        public EnmRequest request;
        public object[] args;
        public object[] results;
        public Action<object> callBack;

        private readonly object objlock = new();
        private bool _end = false;
        public bool End
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

        public Request(EnmRequest req, object[] args = null, Action<object> callBack = null)
        {
            request = req;
            this.args = args;
            this.callBack = callBack;
        }

    }

    public enum EnmRequest
    {
        Die,
        GetStatus,
        Stop,
        Play,
        Pause,
        Fadeout
    }
}
