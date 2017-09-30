using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Farseer.Net.Context;

namespace Farseer.Net.Utils.Common
{
    public static class HttpWebRequestCatExt
    {
        public static void AddHeaderCatInfo(this HttpWebRequest httpWebRequest)
        {
            if (TraceIdContext.Current == null) { return; }
            httpWebRequest.Headers.Add("_catRootMessageId", TraceIdContext.Current.RootId ?? "");
            httpWebRequest.Headers.Add("_catParentMessageId", TraceIdContext.Current.ParentId ?? "");
            httpWebRequest.Headers.Add("_catChildMessageId", TraceIdContext.Current.ChildId ?? "");
        }
    }

}
