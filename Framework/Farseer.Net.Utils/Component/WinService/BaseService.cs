// ********************************************
// 作者：何达贤（steden） QQ：11042427
// 时间：2016-07-13 16:14
// ********************************************

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using Farseer.Net.Extends;
using Farseer.Net.Utils.Common;
using FS.Extends;
using FS.Utils.Common;
using FS.Utils.Component;

namespace FS.Utils.Component.WinService
{
    /// <summary>服务程序基类</summary>
    public abstract class BaseService<TService> : ServiceBase where TService : BaseService<TService>, new()
    {
        /// <summary>显示名</summary>
        protected string DisplayName { get; set; }
        /// <summary>描述</summary>
        protected string Description { get; set; }
        /// <summary>服务实例。每个应用程序域只有一个服务实例</summary>
        protected static TService Instance { get; set; } = new TService();
        /// <summary> 是否来自BaseTimingService派生的类 </summary>
        private readonly bool _isTimingService = typeof(TService).GetBaseTypes().Any(o => o == typeof(BaseTimingService<TService>));
        /// <summary>初始化</summary>
        protected BaseService(string serviceName, string displayName, string description)
        {
            ServiceName = string.IsNullOrEmpty(serviceName) ? GetType().Name : serviceName;
            DisplayName = string.IsNullOrEmpty(displayName) ? ServiceName : displayName;
            Description = string.IsNullOrEmpty(description) ? ServiceName + "服务" : description;
        }

        #region 静态辅助函数
        private static bool _isAutoStartServer;

        /// <summary>服务主函数</summary>
        public static void ServerMain()
        {
            //提升进程优先级
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;

            //每10秒钟检测一下服务状态。
            TimingTasks.Interval(o => ChecState(_isAutoStartServer), 10, 0);
            // 当前进程运行时的参数
            var args = Environment.GetCommandLineArgs();

            if (args.Length > 1) { Instance.Receive(args[1]); }
            else
            {
                Console.Title = Instance.DisplayName;
                Instance.ShowStatus();
                Instance.Receive();
            }
        }

        /// <summary>
        ///     守护主服务进程。
        /// </summary>
        private static void ChecState(object status)
        {
            var autoStart = status.ConvertType(false);

            if (_isAutoStartServer || autoStart)
            {
                if (WinServer.IsRunning(Instance.ServiceName) != false) { return; }
                Console.WriteLine($"*************************************************\r\n发现服务{Instance.ServiceName}被关闭，准备启动！\r\n*************************************************");
                Cmd.Run("net start " + Instance.ServiceName, false, true);
            }
            else
            { if (WinServer.IsRunning(Instance.ServiceName) == true) { _isAutoStartServer = true; } }
        }

        /// <summary>
        ///     输出菜单，并接受命令输入
        /// </summary>
        private void Receive()
        {
            while (true)
            {
                //输出菜单
                ShowMenu();
                Console.Write("请选择操作（-x是命令行参数）：");

                //读取命令
                var key = Console.ReadKey();
                if (key.KeyChar == '0') { break; }
                Console.WriteLine();

                switch (key.KeyChar - '0')
                {
                    case 1: ShowStatus(); break;    // 输出状态
                    case 2: Receive(WinServer.IsInstalled(ServiceName) == true ? "-u" : "-i"); break;   // 安装卸载
                    case 3: Receive(WinServer.IsRunning(ServiceName) == true ? "-stop" : "-start"); break;  // 开始停止
                    case 4: ReceiveSetp(); break; // 单步调试
                    case 5: ReceiveRun(); break; // 循环调试
                    case 6: ReceiveShowTaskList(); break;//  任务列表
                }
            }
        }

        /// <summary>
        ///     接收命令，并执行
        /// </summary>
        /// <param name="args"></param>
        private void Receive(string args)
        {
            switch (args.ToLower())
            {
                case "-s": // 服务安装后，开始运行
                    {
                        try { Run(new ServiceBase[] { Instance }); }
                        catch (Exception ex)
                        {
                            //LogManger.Log.Error(ex);
                            Console.WriteLine(ex.ToString());
                        }
                        ShowStatus();
                    }
                    break;
                case "-i": //安装服务
                    {
                        WinServer.Install(ServiceName, DisplayName, Description);
                        ShowStatus();
                        break;
                    }
                case "-u": // 卸载服务
                    {
                        _isAutoStartServer = false;
                        WinServer.ControlService(ServiceName, false);
                        Console.WriteLine(WinServer.UnInstall(ServiceName));
                        ShowStatus();
                    }
                    break;
                case "-start": //启动服务
                    {
                        Console.WriteLine(WinServer.ControlService(ServiceName, true));
                        ShowStatus();
                    }
                    break;
                case "-stop": //停止服务
                    {
                        _isAutoStartServer = false;
                        WinServer.ControlService(ServiceName, false);
                        ShowStatus();
                        break;
                    }
            }
        }

        /// <summary> 单步调试 </summary>
        protected virtual void ReceiveSetp() { }

        /// <summary> 循环调试 </summary>
        protected virtual void ReceiveRun() { Start(new string[] { }); }

        /// <summary> 显示所有任务列表 </summary>
        protected virtual void ReceiveShowTaskList() { }

        /// <summary>显示状态</summary>
        protected void ShowStatus()
        {
            Consoles.WriteLine("*************************************************", ConsoleColor.Red);
            var service = Instance;
            var name = service.ServiceName;
            Console.Write("服务：");
            Consoles.WriteLine(name != service.DisplayName ? $"{service.DisplayName}({name})" : $"{name}", ConsoleColor.Red);

            Console.Write("描述：");
            Consoles.WriteLine(service.Description, ConsoleColor.Red);
            Console.Write("状态：");

            string status;
            switch (WinServer.IsInstalled(service.ServiceName))
            {
                case null: status = "未知"; break;
                case false: status = "未安装"; break;
                default:
                    switch (WinServer.IsRunning(service.ServiceName))
                    {
                        case null: status = "未知"; break;
                        case false: status = "未启动"; break;
                        default: status = "运行中"; break;
                    }
                    break;
            }
            Consoles.WriteLine(status, ConsoleColor.Green);
            Consoles.WriteLine("*************************************************", ConsoleColor.Red);
        }

        /// <summary>显示菜单</summary>
        protected void ShowMenu()
        {
            var sp = new StrPlus();
            sp.AppendLine(new string('*', 50));
            sp.AppendLine("1 显示状态");

            if (WinServer.IsInstalled(ServiceName) == true)
            {
                if (WinServer.IsRunning(ServiceName) == true) { sp.AppendLine("3 停止服务 -stop"); }
                else
                {
                    sp.AppendLine("2 卸载服务 -u");
                    sp.AppendLine("3 启动服务 -start");
                }
            }
            else
            { sp.AppendLine("2 安装服务 -i"); }

            if (_isTimingService) { sp.AppendLine("4 单步调试 -step"); }
            sp.AppendLine("5 循环调试 -run");
            if (_isTimingService) { sp.AppendLine("6 任务列表 -show"); }
            sp.AppendLine("0 退出");
            sp.AppendLine(new string('*', 50));
            Consoles.WriteLine(sp.ToString(), ConsoleColor.Yellow);
        }
        #endregion

        #region 服务控制
        /// <summary>服务启动事件</summary>
        protected override void OnStart(string[] args)
        {
            _isAutoStartServer = true;
            Consoles.WriteLine("服务启动", ConsoleColor.Red);
            Start(args);
        }

        /// <summary>服务启动事件</summary>
        protected abstract void Start(string[] args);

        /// <summary>重启服务</summary>
        protected void RestartService()
        {
            Console.WriteLine("重启服务！");

            //在临时目录生成重启服务的批处理文件
            var filename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "重启.bat");
            if (File.Exists(filename)) { File.Delete(filename); }

            File.AppendAllText(filename, "net stop " + ServiceName);
            File.AppendAllText(filename, Environment.NewLine);
            File.AppendAllText(filename, "ping 127.0.0.1 -n 5 > nul ");
            File.AppendAllText(filename, Environment.NewLine);
            File.AppendAllText(filename, "net start " + ServiceName);

            //执行重启服务的批处理
            var p = new Process();
            var si = new ProcessStartInfo { FileName = filename, UseShellExecute = true, CreateNoWindow = true };
            p.StartInfo = si;
            p.Start();
        }
        #endregion
    }
}