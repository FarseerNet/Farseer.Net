using System;
using System.Threading.Tasks;
using FS;
using FS.DI;

namespace Farseer.Net.DataDemo
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            FarseerApplication.Run<StartupModule>().Initialize();

            var lst = await IocManager.GetService<TaskGroupAgent>().ToListAsync();

            foreach (var taskGroupPO in lst)
            {
                Console.WriteLine(taskGroupPO.Caption);
            }
        }
    }
}