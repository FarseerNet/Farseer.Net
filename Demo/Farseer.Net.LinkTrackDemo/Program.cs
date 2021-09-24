using System;
using System.Threading;
using FS;
using FS.Core.LinkTrack;

namespace Farseer.Net.LinkTrackDemo
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            FarseerApplication.Run<StartupModule>().Initialize();
            using (var trackEnd = FsLinkTrack.Track(message: ""))
            {
                throw new Exception(message: "aaaaa");
            }

            Thread.Sleep(millisecondsTimeout: -1);
        }
    }
}