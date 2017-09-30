using System;
using System.Runtime.Serialization;

namespace Farseer.Net
{
    /// <summary>
    ///     系统初始化异常
    /// </summary>
    [Serializable]
    public class FarseerInitException : FarseerException
    {
        /// <summary>
        ///     构造函数
        /// </summary>
        public FarseerInitException() { }
#if !CORE
        /// <summary>
        ///     构造函数
        /// </summary>
        public FarseerInitException(SerializationInfo serializationInfo, StreamingContext context) : base(serializationInfo, context) { }
#endif
        /// <summary>
        ///     构造函数
        /// </summary>
        /// <param name="message">Exception message</param>
        public FarseerInitException(string message) : base(message) { }

        /// <summary>
        ///     构造函数
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Inner exception</param>
        public FarseerInitException(string message, Exception innerException) : base(message, innerException) { }
    }
}