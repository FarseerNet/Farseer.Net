using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using FS.MQ.Rocket.SDK.Http.Model.exp;

namespace FS.MQ.Rocket.SDK.Http.Util
{
    public static class AliyunSDKUtils
    {
        #region Internal Constants

        internal const  string SDKVersionNumber   = "1.0.1";
        internal static string _userAgentBaseName = "mq-csharp-sdk";

        private const int DefaultConnectionLimit = 50;
        private const int DefaultMaxIdleTime     = 50 * 1000; // 50 seconds

        internal static readonly DateTime EPOCH_START = new DateTime(year: 1970, month: 1, day: 1, hour: 0, minute: 0, second: 0, millisecond: 0, kind: DateTimeKind.Utc);

        internal const int DefaultBufferSize = 8192;

        internal static Dictionary<int, string> RFCEncodingSchemes = new Dictionary<int, string>
        {
            { 3986, ValidUrlCharacters },
            { 1738, ValidUrlCharactersRFC1738 }
        };

        #endregion

        #region Public Constants

        public const string ValidUrlCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";

        public const string ValidUrlCharactersRFC1738 = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.";

        private static readonly string ValidPathCharacters = DetermineValidPathCharacters();

        private static string DetermineValidPathCharacters()
        {
            const string basePathCharacters = "/:'()!*[]";

            var sb = new StringBuilder();
            foreach (var c in basePathCharacters)
            {
                var escaped = Uri.EscapeUriString(stringToEscape: c.ToString());
                if (escaped.Length == 1 && escaped[index: 0] == c) sb.Append(value: c);
            }

            return sb.ToString();
        }

        public const string ISO8601DateFormat = "yyyy-MM-dd\\THH:mm:ss.fff\\Z";

        public const string RFC822DateFormat = "ddd, dd MMM yyyy HH:mm:ss \\G\\M\\T";

        #endregion

        #region UserAgent

        private static string _versionNumber;

        public static string SDKUserAgent { get; private set; }

        static AliyunSDKUtils()
        {
            BuildUserAgentString();
        }

        private static void BuildUserAgentString()
        {
            if (_versionNumber == null) _versionNumber = SDKVersionNumber;

            try
            {
                SDKUserAgent = string.Format(provider: CultureInfo.InvariantCulture, format: "{0}/{1}(.NET/{2} {3}/{4})",
                                             _userAgentBaseName,
                                             _versionNumber,
                                             DetermineRuntime(),
                                             Environment.OSVersion.Platform,
                                             DetermineOSVersion());
            }
            catch (Exception)
            {
                SDKUserAgent = string.Format(provider: CultureInfo.InvariantCulture, format: "{0}/{1}", arg0: _userAgentBaseName, arg1: _versionNumber);
            }
        }

        private static string DetermineRuntime() => string.Format(provider: CultureInfo.InvariantCulture, format: "{0}.{1}", arg0: Environment.Version.Major, arg1: Environment.Version.MajorRevision);

        private static string DetermineOSVersion() => Environment.OSVersion.Version.ToString();

        #endregion

        #region HTTP Connection Configurations

        private const int _defaultConnectionLimit = 2;

        internal static int GetConnectionLimit(int? clientConfigValue)
        {
            // Connection limit has been explicitly set on the client.
            if (clientConfigValue.HasValue) return clientConfigValue.Value;

            // If default has been left at the system default return the SDK default.
            if (ServicePointManager.DefaultConnectionLimit == _defaultConnectionLimit) return DefaultConnectionLimit;

            // The system default has been explicitly changed so we will honor that value.
            return ServicePointManager.DefaultConnectionLimit;
        }

        private const int _defaultMaxIdleTime = 100 * 1000;

        internal static int GetMaxIdleTime(int? clientConfigValue)
        {
            // MaxIdleTime has been explicitly set on the client.
            if (clientConfigValue.HasValue) return clientConfigValue.Value;

            // If default has been left at the system default return the SDK default.
            if (ServicePointManager.MaxServicePointIdleTime == _defaultMaxIdleTime) return DefaultMaxIdleTime;

            // The system default has been explicitly changed so we will honor that value.
            return ServicePointManager.MaxServicePointIdleTime;
        }

        #endregion

        #region Internal Methods

        internal static string GetParametersAsString(IDictionary<string, string> parameters)
        {
            var keys = new string[parameters.Keys.Count];
            parameters.Keys.CopyTo(array: keys, arrayIndex: 0);
            Array.Sort(array: keys);

            var data = new StringBuilder(capacity: 512);
            foreach (var key in keys)
            {
                var value = parameters[key: key];
                if (value != null)
                {
                    data.Append(value: key);
                    data.Append(value: '=');
                    data.Append(value: value);
                    data.Append(value: '&');
                }
            }

            var result = data.ToString();
            if (result.Length == 0) return string.Empty;

            return result.Remove(startIndex: result.Length - 1);
        }

        public static void CopyTo(this Stream input, Stream output)
        {
            var buffer = new byte[16 * 1024];
            int bytesRead;

            while ((bytesRead = input.Read(buffer: buffer, offset: 0, count: buffer.Length)) > 0) output.Write(buffer: buffer, offset: 0, count: bytesRead);
        }

        /// <summary>
        ///     Utility method for converting Unix epoch seconds to DateTime structure.
        /// </summary>
        /// <param name="milliSeconds"> The number of milliSeconds since January 1, 1970. </param>
        /// <returns> Converted DateTime structure </returns>
        public static DateTime ConvertFromUnixEpochSeconds(long milliSeconds) => new DateTime(ticks: milliSeconds * 10000L + EPOCH_START.Ticks, kind: DateTimeKind.Utc).ToLocalTime();

        public static long GetUnixTimeStamp(DateTime dateTime)
        {
            var ts = dateTime - EPOCH_START;
            return (long)ts.TotalMilliseconds;
        }

        public static long GetNowTimeStamp()
        {
            var ts = DateTime.UtcNow - new DateTime(year: 1970, month: 1, day: 1, hour: 0, minute: 0, second: 0, millisecond: 0);
            return Convert.ToInt64(value: ts.TotalMilliseconds);
        }

        #endregion

        #region Public Methods and Properties

        /// <summary>
        ///     Formats the current date as ISO 8601 timestamp
        /// </summary>
        /// <returns>
        ///     An ISO 8601 formatted string representation
        ///     of the current date and time
        /// </returns>
        public static string FormattedCurrentTimestampRFC822 => GetFormattedTimestampRFC822(minutesFromNow: 0);

        /// <summary>
        ///     Gets the RFC822 formatted timestamp that is minutesFromNow
        ///     in the future.
        /// </summary>
        /// <param name="minutesFromNow">
        ///     The number of minutes from the current instant
        ///     for which the timestamp is needed.
        /// </param>
        /// <returns> The ISO8601 formatted future timestamp. </returns>
        public static string GetFormattedTimestampRFC822(int minutesFromNow)
        {
            var dateTime = DateTime.UtcNow.AddMinutes(value: minutesFromNow);
            var formatted = new DateTime(
                                         year: dateTime.Year,
                                         month: dateTime.Month,
                                         day: dateTime.Day,
                                         hour: dateTime.Hour,
                                         minute: dateTime.Minute,
                                         second: dateTime.Second,
                                         millisecond: dateTime.Millisecond,
                                         kind: DateTimeKind.Local
                                        );
            return formatted.ToString(
                                      format: RFC822DateFormat,
                                      provider: CultureInfo.InvariantCulture
                                     );
        }


        /// <summary>
        ///     URL encodes a string per RFC3986. If the path property is specified,
        ///     the accepted path characters {/+:} are not encoded.
        /// </summary>
        /// <param name="data"> The string to encode </param>
        /// <param name="path"> Whether the string is a URL path or not </param>
        /// <returns> The encoded string </returns>
        public static string UrlEncode(string data, bool path) => UrlEncode(rfcNumber: 3986, data: data, path: path);

        /// <summary>
        ///     URL encodes a string per the specified RFC. If the path property is specified,
        ///     the accepted path characters {/+:} are not encoded.
        /// </summary>
        /// <param name="rfcNumber"> RFC number determing safe characters </param>
        /// <param name="data"> The string to encode </param>
        /// <param name="path"> Whether the string is a URL path or not </param>
        /// <returns> The encoded string </returns>
        /// <remarks>
        ///     Currently recognised RFC versions are 1738 (Dec '94) and 3986 (Jan '05).
        ///     If the specified RFC is not recognised, 3986 is used by default.
        /// </remarks>
        internal static string UrlEncode(int rfcNumber, string data, bool path)
        {
            var    encoded = new StringBuilder(capacity: data.Length * 2);
            string validUrlCharacters;
            if (!RFCEncodingSchemes.TryGetValue(key: rfcNumber, value: out validUrlCharacters)) validUrlCharacters = ValidUrlCharacters;

            var unreservedChars = string.Concat(str0: validUrlCharacters, str1: path ? ValidPathCharacters : "");

            foreach (char symbol in Encoding.UTF8.GetBytes(s: data))
                if (unreservedChars.IndexOf(value: symbol) != -1)
                    encoded.Append(value: symbol);
                else
                    encoded.Append(value: "%").Append(value: string.Format(provider: CultureInfo.InvariantCulture, format: "{0:X2}", arg0: (int)symbol));

            return encoded.ToString();
        }

        public static void Sleep(int ms)
        {
            Thread.Sleep(millisecondsTimeout: ms);
        }

        private static readonly object     _preserveStackTraceLookupLock = new object();
        private static          bool       _preserveStackTraceLookup;
        private static          MethodInfo _preserveStackTrace;

        /// <summary>
        ///     This method is used preserve the stacktrace used from clients that support async calls.  This
        ///     make sure that exceptions thrown during EndXXX methods has the orignal stacktrace that happen
        ///     in the background thread.
        /// </summary>
        /// <param name="exception"> </param>
        internal static void PreserveStackTrace(Exception exception)
        {
            if (!_preserveStackTraceLookup)
            {
                lock (_preserveStackTraceLookupLock)
                {
                    _preserveStackTraceLookup = true;
                    try
                    {
                        _preserveStackTrace = typeof(Exception).GetMethod(name: "InternalPreserveStackTrace",
                                                                          bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic);
                    }
                    catch
                    {
                    }
                }
            }

            if (_preserveStackTrace != null) _preserveStackTrace.Invoke(obj: exception, parameters: null);
        }

        /// <summary>
        ///     IDictionary to string.
        /// </summary>
        /// <returns> The to string. </returns>
        /// <param name="dict"> Dict. </param>
        public static string DictToString(IDictionary<string, string> dict)
        {
            if (dict == null || dict.Count == 0) return string.Empty;
            var data = new StringBuilder(capacity: 64);
            foreach (var key in dict.Keys)
            {
                var value = dict[key: key];
                CheckPropValid(key: key, value: value);
                data.Append(value: key).Append(value: ":").Append(value: value).Append(value: "|");
            }

            return data.ToString();
        }

        /// <summary>
        ///     Strings to IDictionary.
        /// </summary>
        /// <param name="str"> String. </param>
        /// <param name="dict"> Dict. </param>
        public static void StringToDict(string str, IDictionary<string, string> dict)
        {
            if (string.IsNullOrEmpty(value: str) || dict == null) return;

            var str_array = str.Split('|');
            foreach (var kv in str_array)
            {
                var kv_array = kv.Split(':');
                if (kv_array.Length != 2) continue;
                dict.Add(key: kv_array[0], value: kv_array[1]);
            }
        }

        internal static void CheckPropValid(string key, string value)
        {
            if (string.IsNullOrEmpty(value: key) || string.IsNullOrEmpty(value: value)) throw new MQException(message: "Message's property can't be null or empty");

            if (IsContainSpecialChar(str: key) || IsContainSpecialChar(str: value))
            {
                throw new MQException(
                                      message: string.Format(
                                                             format: "Message's property[{0}:{1}] can't contains: & \" ' < > : |",
                                                             arg0: key, arg1: value
                                                            )
                                     );
            }
        }

        internal static bool IsContainSpecialChar(string str) => str.Contains(value: "&")     || str.Contains(value: "<") || str.Contains(value: ">")
                                                                 || str.Contains(value: "\"") || str.Contains(value: "\'")
                                                                 || str.Contains(value: "|")  || str.Contains(value: ":");

        #endregion
    }
}