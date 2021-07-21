using System;
using System.Threading;
using FS;
using FS.Core.LinkTrack;

namespace Farseer.Net.LinkTrackDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            FarseerApplication.Run<StartupModule>().Initialize();
            using (var trackEnd = FsLinkTrack.Track(""))
            {
                throw new Exception("aaaaa");
            }
            
            Thread.Sleep(-1);
        }
    }
}