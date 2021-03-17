using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer
{
    public static class OpeManager
    {

        private static List<Request> reqToAudio = new List<Request>();
        private static object reqLock = new object();

        public static enmAudioStatus GetAudioStatus()
        {
            return enmAudioStatus.Unknown;
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

                Request req = reqToAudio[reqToAudio.Count - 1];
                reqToAudio.Clear();

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
                trdCallback cb = new trdCallback(req);
                System.Threading.Thread trd = new System.Threading.Thread(cb.callBack);
                trd.Priority = System.Threading.ThreadPriority.BelowNormal;
                trd.Start();
            }
        }

    }

    public class trdCallback
    {
        private Request request;

        public trdCallback(Request req)
        {
            request = req;
        }

        public void callBack()
        {
            request.callBack?.Invoke(request.results);
        }
    }

    public enum enmAudioStatus
    {
        Unknown,
        Stop,
        FadeOut,
        Play,
        Pause
    }
}
