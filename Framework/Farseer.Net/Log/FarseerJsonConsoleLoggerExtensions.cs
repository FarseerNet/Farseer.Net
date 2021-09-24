using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace FS.Log
{
    public static class FarseerJsonConsoleLoggerExtensions
    {
        /// <summary>
        ///     添加Json输出的日志（方便于容器日志采集）
        /// </summary>
        /// <param name="services"> </param>
        public static IServiceCollection AddFarseerLogging(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(paramName: nameof(services));
            services.AddLogging(configure: logging =>
            {
                logging.AddFarseerJsonConsole();

                // 非生产环境下添加调试输出
                if (Environment.GetEnvironmentVariable(variable: "ASPNETCORE_ENVIRONMENT") != "" && Environment.GetEnvironmentVariable(variable: "ASPNETCORE_ENVIRONMENT") != "Production") logging.AddDebug();
            });

            return services;
        }

        /// <summary>
        ///     添加Json输出的日志（方便于容器日志采集）
        /// </summary>
        /// <param name="builder"> </param>
        /// <param name="options"> </param>
        /// <returns> </returns>
        public static ILoggingBuilder AddFarseerJsonConsole(this ILoggingBuilder builder, Action<JsonConsoleFormatterOptions> options = null)
        {
            options ??= _ =>
            {
                _.UseUtcTimestamp = false;
                _.TimestampFormat = "yyyy-MM-dd HH:mm:ss";
            };
            //添加控制台输出
            builder.AddConsoleFormatter<FarseerJsonConsole, JsonConsoleFormatterOptions>(configure: _ => options(obj: _));

            builder.AddConsole(configure: o =>
            {
                o.FormatterName = "json";
            });
            return builder;
        }
    }
}