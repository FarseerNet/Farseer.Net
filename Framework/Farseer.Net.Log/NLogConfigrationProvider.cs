using System;
using System.Text;
using FS.Log.Configuration;
using NLog;
using NLog.Config;
using NLog.LayoutRenderers;
using NLog.Targets;
using NLog.Targets.Wrappers;

namespace FS.Log
{
    internal class NLogConfigrationProvider
    {
        static NLogConfigrationProvider()
        {
            var traceId = "traceId"; //初始化TraceId
            LayoutRenderer.Register(name: "traceid", func: logEvent => traceId);
            LayoutRenderer.Register(name: "MyDateTime", func: logEvent => DateTime.Now.ToString(format: "yyyy-MM-dd HH:mm:ss.fff")); //初始化日期格式
        }

        public static LoggingConfiguration CreateConfigration(NLogConfig userConfig)
        {
            // Step 1. Create configuration object 
            var config = new LoggingConfiguration();

            // Step 2. Create targets and add them to the configuration 

            var fileTarget = new FileTarget();


            //%d{yyyy-MM-dd HH:mm:ss.SSS} ${LOG_LEVEL_PATTERN:-%5p} ${PID:- } [%traceId] [%t][%logger] : %m%n${LOG_EXCEPTION_CONVERSION_WORD:-%wEx
            //2017-07-24 18:42:28.699 ERROR 10997 [null][viceImpl-0-exe0] com.Farseer.dubbo.example.service.Handle: aaaaaaaaa
            //时间,日志类别,进程ID,TraceId,线程名称,命名空间.类名 日志内容

            // Step 3. Set target properties 
            //fileTarget.FileName = "c:/Farseerlog/应用名称/日志文件.log";
            //fileTarget.FileName = $"{itemConfig.FolderPath}{itemConfig.AppName}/日志文件.log";
            fileTarget.FileName = $"c:/Farseerlog/{userConfig.AppName}/日志文件.log";
            //fileTarget.Layout = @"${MyDateTime} ${level:uppercase=True} ${processid} [${traceid}] [${threadid}] ${callsite:className=True:includeNamespace=True:fileName=False:includeSourcePath=True:methodName = False:cleanNamesOfAnonymousDelegates = False:skipFrames = 0}: ${message} ${newline}";
            //fileTarget.Layout = @"[${MyDateTime}] ${level:uppercase=True} ${processid} [${traceid}] [${threadname}] ${callsite:className=True:includeNamespace=True:fileName=False:includeSourcePath=True:methodName = False:cleanNamesOfAnonymousDelegates = False:skipFrames = 0}: ${message}";
            fileTarget.Layout = @"[${date:format=yyyy-MM-dd HH\:mm\:ss.fff}] ${level} ${processid} [${traceid}] [${threadid}] [${callsite:className=True:includeNamespace=True:fileName=False:includeSourcePath=True:methodName = False:cleanNamesOfAnonymousDelegates = False:skipFrames = 0}] : ${message}${exception:innerFormat=StackTrace:maxInnerExceptionLevel=100:format=StackTrace}";
            //fileTarget.ArchiveFileName = "c:/Farseerlog/应用名称/日志文件{#}.log";
            //fileTarget.ArchiveFileName = itemConfig.FolderPath+itemConfig.AppName+"/日志文件{#}.log";
            fileTarget.ArchiveFileName      = "c:/Farseerlog/" + userConfig.AppName + "/日志文件{#}.log";
            fileTarget.ArchiveNumbering     = ArchiveNumberingMode.Rolling;
            fileTarget.MaxArchiveFiles      = 10;
            fileTarget.ArchiveAboveSize     = 1024 * 1024 * 512; //1024 * 1024 * 1024;
            fileTarget.ConcurrentWrites     = true;
            fileTarget.KeepFileOpen         = true;
            fileTarget.OpenFileCacheTimeout = 30;
            fileTarget.Encoding             = Encoding.UTF8;

            var asyncTargetWrapper = new AsyncTargetWrapper(wrappedTarget: fileTarget, queueLimit: 10000, overflowAction: AsyncTargetWrapperOverflowAction.Discard);

            config.AddTarget(name: "Farseer_log_file", target: asyncTargetWrapper);

            // Step 4. Define rules
            var rule2 = new LoggingRule(loggerNamePattern: "*", minLevel: LogLevel.Trace, target: asyncTargetWrapper);
            config.LoggingRules.Add(item: rule2);

            // Step 5. Activate the configuration
            return config;
        }
    }
}