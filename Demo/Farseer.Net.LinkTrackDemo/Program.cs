using System;
using System.Threading;
using FS;
using FS.Core.Attribute.Data;
using FS.Core.LinkTrack;

namespace Farseer.Net.LinkTrackDemo
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            FarseerApplication.Run<StartupModule>().Initialize();
            new TrackDemo().Execute();

            Thread.Sleep(millisecondsTimeout: -1);
        }
    }

    [Track]
    public class TrackDemo
    {
        public TrackDemo()
        {

        }
        
        public void Execute()
        {

        }
    }
}