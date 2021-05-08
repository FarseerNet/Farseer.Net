using System;
using System.Buffers;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;

namespace FS.Log
{
    public class FarseerJsonConsole : ConsoleFormatter, IDisposable
    {
        /// <summary>
        /// 释放
        /// </summary>
        private readonly IDisposable _optionsReloadToken;

        /// <summary>
        /// 格式化设置，请通过
        /// </summary>
        internal JsonConsoleFormatterOptions FormatterOptions { get; set; }

        /// <summary>
        /// 当配置改变时，自动重读
        /// </summary>
        public FarseerJsonConsole(IOptionsMonitor<JsonConsoleFormatterOptions> options) : base("json")
        {
            this.ReloadLoggerOptions(options.CurrentValue);
            this._optionsReloadToken = options.OnChange(this.ReloadLoggerOptions);
        }

        public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider scopeProvider, TextWriter textWriter)
        {
            var formatter = logEntry.Formatter(logEntry.State, logEntry.Exception);
            if (logEntry.Exception == null && formatter == null) return;
            var logLevel  = logEntry.LogLevel;
            var category  = logEntry.Category;
            var id        = logEntry.EventId.Id;
            var exception = logEntry.Exception;

            var byteBufferWriter = new ArrayBufferWriter<byte>(1024);
            using (Utf8JsonWriter writer = new(byteBufferWriter, this.FormatterOptions.JsonWriterOptions))
            {
                writer.WriteStartObject();
                string timestampFormat = this.FormatterOptions.TimestampFormat;
                if (timestampFormat != null)
                {
                    DateTimeOffset dateTimeOffset = this.FormatterOptions.UseUtcTimestamp ? DateTimeOffset.UtcNow : DateTimeOffset.Now;
                    writer.WriteString("Timestamp", dateTimeOffset.ToString(timestampFormat));
                }

                writer.WriteString("LogLevel", logLevel.ToString());
                if (id > 0) writer.WriteNumber("EventId", id);
                writer.WriteString("Category", category.Split('.').LastOrDefault());
                writer.WriteString("Message",  formatter);

                if (exception != null)
                {
                    string str2 = exception.ToString();
                    if (!this.FormatterOptions.JsonWriterOptions.Indented)
                        str2 = str2.Replace(Environment.NewLine, " ");
                    writer.WriteString("Exception", str2);
                }

                this.WriteScopeInformation(writer, scopeProvider);
                writer.WriteEndObject();
                writer.Flush();
            }

            var content = Encoding.UTF8.GetString(byteBufferWriter.WrittenMemory.Span.ToArray());
            content = Unicode2String(content);
            textWriter.Write(content);
            textWriter.Write(Environment.NewLine);
        }

        /// <summary>
        /// Unicode转字符串
        /// </summary>
        /// <param name="source">经过Unicode编码的字符串</param>
        /// <returns>正常字符串</returns>
        public string Unicode2String(string source)
        {
            return new Regex(@"\\u([0-9A-F]{4})", RegexOptions.IgnoreCase | RegexOptions.Compiled).Replace(
                source, x => string.Empty + Convert.ToChar(Convert.ToUInt16(x.Result("$1"), 16)));
        }

        private string ToInvariantString(object obj) => Convert.ToString(obj, CultureInfo.InvariantCulture);

        private void WriteItem(Utf8JsonWriter writer, KeyValuePair<string, object> item)
        {
            string key = item.Key;
            switch (item.Value)
            {
                case bool flag:
                    writer.WriteBoolean(key, flag);
                    break;
                case byte num1:
                    writer.WriteNumber(key, num1);
                    break;
                case sbyte num2:
                    writer.WriteNumber(key, num2);
                    break;
                case char ch:
                    writer.WriteString(key, ch.ToString());
                    break;
                case Decimal num3:
                    writer.WriteNumber(key, num3);
                    break;
                case double num4:
                    writer.WriteNumber(key, num4);
                    break;
                case float num5:
                    writer.WriteNumber(key, num5);
                    break;
                case int num6:
                    writer.WriteNumber(key, num6);
                    break;
                case uint num7:
                    writer.WriteNumber(key, num7);
                    break;
                case long num8:
                    writer.WriteNumber(key, num8);
                    break;
                case ulong num9:
                    writer.WriteNumber(key, num9);
                    break;
                case short num10:
                    writer.WriteNumber(key, num10);
                    break;
                case ushort num11:
                    writer.WriteNumber(key, num11);
                    break;
                case null:
                    writer.WriteNull(key);
                    break;
                default:
                    writer.WriteString(key, ToInvariantString(item.Value));
                    break;
            }
        }

        private void WriteScopeInformation(Utf8JsonWriter writer, IExternalScopeProvider scopeProvider)
        {
            if (!this.FormatterOptions.IncludeScopes || scopeProvider == null) return;
            writer.WriteStartArray("Scopes");
            scopeProvider.ForEachScope((scope, state) =>
            {
                if (scope is IReadOnlyCollection<KeyValuePair<string, object>> keyValuePairs2)
                {
                    state.WriteStartObject();
                    state.WriteString("Message", scope.ToString());
                    foreach (KeyValuePair<string, object> keyValuePair in keyValuePairs2)
                        this.WriteItem(state, keyValuePair);
                    state.WriteEndObject();
                }
                else
                    state.WriteStringValue(ToInvariantString(scope));
            }, writer);
            writer.WriteEndArray();
        }

        private void ReloadLoggerOptions(JsonConsoleFormatterOptions options) => this.FormatterOptions = options;
        public  void Dispose()                                                => this._optionsReloadToken?.Dispose();
    }
}