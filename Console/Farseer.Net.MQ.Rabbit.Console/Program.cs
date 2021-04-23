using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using FS;
using FS.DI;
using FS.MQ.Rabbit;
using FS.MQ.Rabbit.Attr;
using FS.Utils.Common;
using RabbitMQ.Client;

namespace Farseer.Net.MQ.Rabbit.Console
{
    [Rabbit]
    class Program
    {
        static void Main(string[] args)
        {
            // 项目启动时初始化
            FarseerApplication.Run<StartupModule>().Initialize();
            Thread.Sleep(-1);
        }
    }
}