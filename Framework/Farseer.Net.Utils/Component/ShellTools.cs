using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
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
        public static async Task<RunShellResult> Run(string cmd, string arguments, Action<string> actReceiveOutput, Dictionary<string, string> environment, string workingDirectory = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                // 打印当前执行的命令
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

                // 结果
                var runShellResult = new RunShellResult {IsError = false, Output = new List<string>()};

                // 开始执行
                using (var proc = Process.Start(psi))
                {
                    proc.EnableRaisingEvents = true;

                    // 收到回显后的处理
                    void ProcOnOutputDataReceived(object sender, DataReceivedEventArgs args)
                    {
                        if (string.IsNullOrWhiteSpace(args.Data)) return;
                        runShellResult.Output.Add(args.Data);
                        // 外部第一时间，处理拿到的消息
                        if (actReceiveOutput != null) actReceiveOutput(args.Data);
                    }

                    proc.OutputDataReceived += ProcOnOutputDataReceived;
                    proc.ErrorDataReceived  += ProcOnOutputDataReceived;
                    proc.BeginOutputReadLine();
                    proc.BeginErrorReadLine();

                    // 增加取消令牌
                    var tcs = new TaskCompletionSource<RunShellResult>(TaskCreationOptions.RunContinuationsAsynchronously);
                    void ProcessExited(object sender, EventArgs e)
                    {
                        runShellResult.IsError = proc.ExitCode != 0;
                        tcs.TrySetResult(runShellResult);
                    }

                    proc.Exited += ProcessExited;

                    try
                    {
                        if (proc.HasExited)
                        {
                            runShellResult.IsError = proc.ExitCode != 0;
                            return runShellResult;
                        }

                        using (cancellationToken.Register(() => tcs.TrySetCanceled()))
                        {
                            await tcs.Task.ConfigureAwait(false);
                        }
                    }
                    finally
                    {
                        proc.Exited -= ProcessExited;
                    }

                    // 等待退出
                    //proc.WaitForExit();
                    //runShellResult.IsError = proc.ExitCode != 0;
                }

                return runShellResult;
            }
            catch (Exception e)
            {
                if (actReceiveOutput != null) actReceiveOutput(e.Message);
                return new RunShellResult(true, e.Message);
            }
        }
    }
}