using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using FS.Core.Entity;

namespace FS.Utils.Component
{
    public class ShellTools
    {
        /// <summary>
        /// 执行shell 命令
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="arguments"></param>
        /// <param name="actReceiveOutput">外部第一时间，处理拿到的消息 </param>
        /// <param name="workingDirectory">设定Shell的工作目录 </param>
        public static async Task<RunShellResult> Run(string cmd, string arguments, Action<string> actReceiveOutput, Dictionary<string, string> environment, string workingDirectory = null)
        {
            try
            {
                if (actReceiveOutput != null) actReceiveOutput($"{cmd} {arguments}");
                var psi = new ProcessStartInfo(cmd, arguments)
                {
                    RedirectStandardOutput = true,
                    RedirectStandardError  = true,
                    UseShellExecute        = false,
                    WorkingDirectory       = workingDirectory,
                };

                // 添加环境变量
                if (environment != null)
                {
                    foreach (var env in environment)
                    {
                        psi.Environment.Add(env);
                    }
                }
                var runShellResult = new RunShellResult {IsError = false, Output = new List<string>()};

                using (var proc = Process.Start(psi))
                {
                    proc.EnableRaisingEvents = true;

                    //开始读取
                    while (!proc.StandardOutput.EndOfStream)
                    {
                        var output = await proc.StandardOutput.ReadLineAsync();
                        if (string.IsNullOrWhiteSpace(output)) continue;
                        runShellResult.Output.Add(output);

                        // 外部第一时间，处理拿到的消息
                        if (actReceiveOutput != null) actReceiveOutput(output);
                    }

                    while (!proc.StandardError.EndOfStream)
                    {
                        var output = await proc.StandardError.ReadLineAsync();
                        if (string.IsNullOrWhiteSpace(output)) continue;
                        runShellResult.Output.Add(output);

                        // 外部第一时间，处理拿到的消息
                        if (actReceiveOutput != null) actReceiveOutput(output);
                    }

                    // 等待退出
                    proc.WaitForExit();
                    runShellResult.IsError = proc.ExitCode != 0;
                }

                return runShellResult;
            }
            catch (Exception e)
            {
                if (actReceiveOutput != null) actReceiveOutput(e.Message);
                return new RunShellResult() {IsError = true, Output = new List<string>() {e.Message}};
            }
        }
    }
}