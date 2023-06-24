namespace MDPlayer
{
    public static class OpeManager
    {

        private static readonly List<Request> reqToAudio = new();
        private static readonly object reqLock = new();

        public static EnmAudioStatus GetAudioStatus()
        {
            return EnmAudioStatus.Unknown;
        }

        public static void RequestToAudio(Request req)
        {
            lock (reqLock) reqToAudio.Add(req);
        }



        /// <summary>
        /// Audioがリクエストを受け取る
        /// </summary>
        /// <returns></returns>
        public static Request GetRequestToAudio()
        {
            lock (reqLock)
            {
                if (reqToAudio.Count < 1) return null;

                Request req = reqToAudio[^1];
                reqToAudio.Remove(req);//.Clear();

                return req;
            }
        }

        /// <summary>
        /// Audioがリクエストの処理を完了したらよばれる
        /// </summary>
        /// <param name="req"></param>
        public static void CompleteRequestToAudio(Request req)
        {
            lock (reqLock)
            {
                TrdCallback cb = new(req);
                System.Threading.Thread trd = new(cb.CallBack)
                {
                    Priority = System.Threading.ThreadPriority.BelowNormal
                };
                trd.Start();
            }
        }

    }

    public class TrdCallback
    {
        private readonly Request request;

        public TrdCallback(Request req)
        {
            request = req;
        }

        public void CallBack()
        {
            request.callBack?.Invoke(request.results);
        }
    }

    public enum EnmAudioStatus
    {
        Unknown,
        Stop,
        FadeOut,
        Play,
        Pause
    }
}
