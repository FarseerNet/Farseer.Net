using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
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
        ///     释放
        /// </summary>
        private readonly IDisposable _optionsReloadToken;

        /// <summary>
        ///     当配置改变时，自动重读
        /// </summary>
        public FarseerJsonConsole(IOptionsMonitor<JsonConsoleFormatterOptions> options) : base(name: "json")
        {
            ReloadLoggerOptions(options: options.CurrentValue);
            _optionsReloadToken = options.OnChange(listener: ReloadLoggerOptions);
        }

        /// <summary>
        ///     格式化设置，请通过
        /// </summary>
        internal JsonConsoleFormatterOptions FormatterOptions { get; set; }

        public void Dispose() => _optionsReloadToken?.Dispose();

        public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider scopeProvider, TextWriter textWriter)
        {
            var formatter = logEntry.Formatter(arg1: logEntry.State, arg2: logEntry.Exception);
            if (logEntry.Exception == null && formatter == null) return;
            var logLevel  = logEntry.LogLevel;
            var category  = logEntry.Category;
            var id        = logEntry.EventId.Id;
            var exception = logEntry.Exception;

            var byteBufferWriter = new ArrayBufferWriter<byte>(initialCapacity: 1024);
            using (Utf8JsonWriter writer = new(bufferWriter: byteBufferWriter, options: FormatterOptions.JsonWriterOptions))
            {
                writer.WriteStartObject();
                var timestampFormat = FormatterOptions.TimestampFormat;
                if (timestampFormat != null)
                {
                    var dateTimeOffset = FormatterOptions.UseUtcTimestamp ? DateTimeOffset.UtcNow : DateTimeOffset.Now;
                    writer.WriteString(propertyName: "Timestamp", value: dateTimeOffset.ToString(format: timestampFormat));
                }

                writer.WriteString(propertyName: "LogLevel", value: logLevel.ToString());
                if (id > 0) writer.WriteNumber(propertyName: "EventId", value: id);
                writer.WriteString(propertyName: "Category", value: category.Split('.').LastOrDefault());
                writer.WriteString(propertyName: "Message", value: formatter);

                if (exception != null)
                {
                    var str2                                               = exception.ToString();
                    if (!FormatterOptions.JsonWriterOptions.Indented) str2 = str2.Replace(oldValue: Environment.NewLine, newValue: " ");
                    writer.WriteString(propertyName: "Exception", value: str2);
                }

                WriteScopeInformation(writer: writer, scopeProvider: scopeProvider);
                writer.WriteEndObject();
                writer.Flush();
            }

            var content = Encoding.UTF8.GetString(bytes: byteBufferWriter.WrittenMemory.Span.ToArray());
            content = Unicode2String(source: content);
            textWriter.Write(value: content);
            textWriter.Write(value: Environment.NewLine);
        }

        /// <summary>
        ///     Unicode转字符串
        /// </summary>
        /// <param name="source"> 经过Unicode编码的字符串 </param>
        /// <returns> 正常字符串 </returns>
        public string Unicode2String(string source)
        {
            return new Regex(pattern: @"\\u([0-9A-F]{4})", options: RegexOptions.IgnoreCase | RegexOptions.Compiled).Replace(
                                                                                                                             input: source, evaluator: x => string.Empty + Convert.ToChar(value: Convert.ToUInt16(value: x.Result(replacement: "$1"), fromBase: 16)));
        }

        private string ToInvariantString(object obj) => Convert.ToString(value: obj, provider: CultureInfo.InvariantCulture);

        private void WriteItem(Utf8JsonWriter writer, KeyValuePair<string, object> item)
        {
            var key = item.Key;
            switch (item.Value)
            {
                case bool flag:
                    writer.WriteBoolean(propertyName: key, value: flag);
                    break;
                case byte num1:
                    writer.WriteNumber(propertyName: key, value: num1);
                    break;
                case sbyte num2:
                    writer.WriteNumber(propertyName: key, value: num2);
                    break;
                case char ch:
                    writer.WriteString(propertyName: key, value: ch.ToString());
                    break;
                case decimal num3:
                    writer.WriteNumber(propertyName: key, value: num3);
                    break;
                case double num4:
                    writer.WriteNumber(propertyName: key, value: num4);
                    break;
                case float num5:
                    writer.WriteNumber(propertyName: key, value: num5);
                    break;
                case int num6:
                    writer.WriteNumber(propertyName: key, value: num6);
                    break;
                case uint num7:
                    writer.WriteNumber(propertyName: key, value: num7);
                    break;
                case long num8:
                    writer.WriteNumber(propertyName: key, value: num8);
                    break;
                case ulong num9:
                    writer.WriteNumber(propertyName: key, value: num9);
                    break;
                case short num10:
                    writer.WriteNumber(propertyName: key, value: num10);
                    break;
                case ushort num11:
                    writer.WriteNumber(propertyName: key, value: num11);
                    break;
                case null:
                    writer.WriteNull(propertyName: key);
                    break;
                default:
                    writer.WriteString(propertyName: key, value: ToInvariantString(obj: item.Value));
                    break;
            }
        }

        private void WriteScopeInformation(Utf8JsonWriter writer, IExternalScopeProvider scopeProvider)
        {
            if (!FormatterOptions.IncludeScopes || scopeProvider == null) return;
            writer.WriteStartArray(propertyName: "Scopes");
            scopeProvider.ForEachScope(callback: (scope, state) =>
            {
                if (scope is IReadOnlyCollection<KeyValuePair<string, object>> keyValuePairs2)
                {
                    state.WriteStartObject();
                    state.WriteString(propertyName: "Message", value: scope.ToString());
                    foreach (var keyValuePair in keyValuePairs2) WriteItem(writer: state, item: keyValuePair);
                    state.WriteEndObject();
                }
                else
                    state.WriteStringValue(value: ToInvariantString(obj: scope));
            }, state: writer);
            writer.WriteEndArray();
        }

        private void ReloadLoggerOptions(JsonConsoleFormatterOptions options) => FormatterOptions = options;
    }
}