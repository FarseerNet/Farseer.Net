﻿// ********************************************
// 作者：何达贤（steden） QQ：11042427
// 时间：2016-07-13 16:35
// ********************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using FS.Utils.Component;

namespace FS.Utils.Common
{
    /// <summary>
    ///     Windows服务工具
    /// </summary>
    public class WinServer
    {
        /// <summary> 安装、卸载 服务 </summary>
        /// <param name="description"> 服务描述 </param>
        /// <param name="serviceName"> 服务名称 </param>
        /// <param name="displayName"> 显示名称 </param>
        public static void Install(string serviceName, string displayName, string description)
        {
            if (string.IsNullOrEmpty(value: serviceName)) throw new Exception(message: "未指定服务名！");
            serviceName = serviceName.Replace(oldValue: " ", newValue: "_");

            // win7及以上系统时才提示
            if (Environment.OSVersion.Version.Major >= 6) Console.WriteLine(value: "在win7/win2008及更高系统中，可能需要管理员权限执行才能安装/卸载服务。");
            Cmd.ServiceCmd(cmd: "create " + serviceName + " BinPath= \"" + Path.Combine(path1: AppDomain.CurrentDomain.BaseDirectory, path2: GetExeName()) + " -s\" start= auto DisplayName= \"" + displayName + "\"");
            if (!string.IsNullOrEmpty(value: description)) Cmd.ServiceCmd(cmd: "description " + serviceName + " \"" + description + "\"");
        }

        /// <summary> 安装、卸载 服务 </summary>
        /// <param name="serviceName"> 服务名称 </param>
        public static string UnInstall(string serviceName)
        {
            if (string.IsNullOrEmpty(value: serviceName)) throw new Exception(message: "未指定服务名！");

            // win7及以上系统时才提示
            if (Environment.OSVersion.Version.Major >= 6) Console.WriteLine(value: "在win7/win2008及更高系统中，可能需要管理员权限执行才能安装/卸载服务。");

            return ControlService(serviceName: serviceName, isStart: false) + Cmd.ServiceCmd(cmd: "Delete " + serviceName.Replace(oldValue: " ", newValue: "_"));
        }


        /// <summary> 是否已启动 </summary>
        public static bool? IsRunning(string name)
        {
            ServiceController control = null;
            try
            {
                control = GetService(serviceName: name);
                if (control == null) return false;
                try
                {
                    //尝试访问一下才知道是否已安装
                    var b = control.CanShutdown;
                }
                catch
                {
                    return false;
                }

                control.Refresh();
                if (control.Status == ServiceControllerStatus.Running) return true;
                if (control.Status == ServiceControllerStatus.Stopped) return false;
                return null;
            }
            catch
            {
                return null;
            }
            finally
            {
                control?.Dispose();
            }
        }

        /// <summary> 取得服务 </summary>
        public static ServiceController GetService(string serviceName)
        {
            var list = new List<ServiceController>(collection: ServiceController.GetServices());
            return list.Count < 1 ? null : list.FirstOrDefault(predicate: item => item.ServiceName == serviceName);
        }

        /// <summary> 启动、停止 服务 </summary>
        /// <param name="serviceName"> 服务对象 </param>
        /// <param name="isStart"> </param>
        public static string ControlService(string serviceName, bool isStart = true)
        {
            if (string.IsNullOrEmpty(value: serviceName)) throw new Exception(message: "未指定服务名！");
            return isStart ? Cmd.Run(cmd: "net start " + serviceName, showWindow: false, waitForExit: true) : Cmd.Run(cmd: "net stop " + serviceName, showWindow: false, waitForExit: true);
        }

        /// <summary> 是否已安装 </summary>
        public static bool? IsInstalled(string serviceName)
        {
            ServiceController control = null;
            try
            {
                // 取的时候就抛异常，是不知道是否安装的
                control = GetService(serviceName: serviceName);
                if (control == null) return false;
                try
                {
                    //尝试访问一下才知道是否已安装
                    var b = control.CanShutdown;
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            catch
            {
                return null;
            }
            finally
            {
                control?.Dispose();
            }
        }

        /// <summary> Exe程序名 </summary>
        public static string GetExeName()
        {
            var p        = Process.GetCurrentProcess();
            var filename = p.MainModule.FileName;
            filename = Path.GetFileName(path: filename);
            filename = filename.Replace(oldValue: ".vshost.", newValue: ".");
            return filename;
        }
    }
}