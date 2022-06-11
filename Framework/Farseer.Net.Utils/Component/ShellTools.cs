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
        ///     执行shell 命令
        /// </summary>
        /// <param name="cmd"> </param>
        /// <param name="arguments"> </param>
        /// <param name="receiveOutput"> 外部第一时间，处理拿到的消息 </param>
        /// <param name="workingDirectory"> 设定Shell的工作目录 </param>
        /// <param name="environment">注入环境变量</param>
        /// <param name="cancellationToken">取消令牌</param>
        public static async Task<int> Run(string cmd, string arguments, IProgress<string> receiveOutput, Dictionary<string, string> environment, string workingDirectory = null, CancellationToken cancellationToken = default)
        {
            var exitCode = 0;
            try
            {
                // 打印当前执行的命令
                if (receiveOutput != null) receiveOutput.Report($"{cmd} {arguments}");
                var psi = new ProcessStartInfo(fileName: cmd, arguments: arguments)
                {
                    RedirectStandardOutput = true,
                    RedirectStandardError  = true,
                    UseShellExecute        = false,
                    WorkingDirectory       = workingDirectory
                };
                
                // 添加环境变量
                if (environment != null)
                    foreach (var env in environment)
                        psi.Environment.Add(item: env);

                // 开始执行
                using (var proc = Process.Start(startInfo: psi))
                {
                    proc.EnableRaisingEvents = true;

                    // 收到回显后的处理
                    void ProcOnOutputDataReceived(object sender, DataReceivedEventArgs args)
                    {
                        if (string.IsNullOrWhiteSpace(value: args.Data)) return;
                        // 外部第一时间，处理拿到的消息
                        if (receiveOutput != null) receiveOutput.Report(args.Data);
                    }

                    proc.OutputDataReceived += ProcOnOutputDataReceived;
                    proc.ErrorDataReceived  += ProcOnOutputDataReceived;
                    proc.BeginErrorReadLine();
                    proc.BeginOutputReadLine();

                    // 增加取消令牌
                    var tcs = new TaskCompletionSource<int>(creationOptions: TaskCreationOptions.RunContinuationsAsynchronously);

                    void ProcessExited(object sender, EventArgs e)
                    {
                        exitCode = proc.ExitCode;
                        tcs.TrySetResult(proc.ExitCode);
                    }

                    proc.Exited += ProcessExited;

                    try
                    {
                        if (proc.HasExited)
                        {
                            return proc.ExitCode;
                        }

                        using (cancellationToken.Register(callback: () => tcs.TrySetCanceled()))
                        {
                            await tcs.Task.ConfigureAwait(continueOnCapturedContext: false);
                        }
                    }
                    finally
                    {
                        proc.Exited -= ProcessExited;
                    }

                    // 等待退出
                    //proc.WaitForExit();
                }

                return exitCode;
            }
            catch (Exception e)
            {
                if (receiveOutput != null) receiveOutput.Report(e.Message);
                if (exitCode      == 0) exitCode = -1;
                return exitCode;
            }
        }
    }
}