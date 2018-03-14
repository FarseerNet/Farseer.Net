// ********************************************
// 作者：何达贤（steden） QQ：11042427
// 时间：2016-07-13 15:23
// ********************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Farseer.Net.Extends;
using FS.Extends;

namespace FS.Utils.Component
{
    /// <summary>
    /// 命令行
    /// </summary>
    public class Cmd
    {
        /// <summary>执行SC命令</summary>
        /// <param name="cmd"></param>
        public static string ServiceCmd(string cmd)
        {
            var path = Environment.SystemDirectory;
            path = Path.Combine(path, @"sc.exe");
            if (!File.Exists(path)) path = "sc.exe";
            if (!File.Exists(path)) return "无法获取系统路径";
            return Run(path + " " + cmd, false, true);
        }

        /// <summary>执行一个命令</summary>
        /// <param name="cmd"></param>
        /// <param name="showWindow"></param>
        /// <param name="waitForExit"></param>
        public static string Run(string cmd, bool showWindow, bool waitForExit)
        {
            if (!cmd.StartsWith(@"/")) { cmd = @"/c " + cmd; }
            var path = Path.Combine(Environment.SystemDirectory, @"cmd.exe");
            var si = new ProcessStartInfo { FileName = path, Arguments = cmd, UseShellExecute = false, CreateNoWindow = !showWindow, RedirectStandardOutput = true, RedirectStandardError = true };
            var p = new Process { StartInfo = si };
            p.Start();

            if (!waitForExit) { return string.Empty; }
            p.WaitForExit();

            var lstResult = new List<string>();
            var str = p.StandardOutput.ReadToEnd();
            if (!string.IsNullOrEmpty(str)) { lstResult.Add(str.Trim('\r', '\n', '\t').Trim()); }
            str = p.StandardError.ReadToEnd();
            if (!string.IsNullOrEmpty(str)) { lstResult.Add(str.Trim('\r', '\n', '\t').Trim()); }
            return lstResult.ToString(",");
        }
    }
}