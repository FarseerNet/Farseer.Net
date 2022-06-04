using System;
using System.Threading;
using FS;
using FS.Core.AOP.Data;
using FS.Core.AOP.LinkTrack;
using FS.Core.LinkTrack;

namespace Farseer.Net.LinkTrackDemo
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            FarseerApplication.Run<StartupModule>().Initialize();
            new TrackDemo().Execute1();

            Thread.Sleep(millisecondsTimeout: -1);
        }
    }

    [Track] // 此处如果标记了，则Execute1、Execute2方法不需要再指定Track
    public class TrackDemo
    {
        [Track]
        public void Execute1() { }
        
        // 虽然我没有标记[Track]，但类中已标记，此处也会继承。
        public void Execute2() { }

        public void Execute3()
        {
            // 如果是记录某个代码片断，可以使用此种方式
            using (FsLinkTrack.Track($"一般这里传的是MethodName"))
            {
                // doSomething
            }
        }
    }
}