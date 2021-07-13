using System.Net;
using System.Net.Http.Headers;
using FS.Core.LinkTrack;

namespace FS.Core.Http
{
    public static class HttpWebRequestCatExt
    {
        public static void AddTraceInfoToHeader(this HttpWebRequest httpWebRequest)
        {
            httpWebRequest.Headers.Add("FsContextId", FsLinkTrack.Current.Get().ContextId);
        }

        public static void AddTraceInfoToHeader(this HttpContentHeaders httpWebRequest)
        {
            httpWebRequest.Add("FsContextId", FsLinkTrack.Current.Get().ContextId);
        }
    }

}
