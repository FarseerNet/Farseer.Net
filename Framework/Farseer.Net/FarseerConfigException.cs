// ********************************************
// 作者：何达贤（steden） QQ：11042427
// 时间：2017-01-17 10:37
// ********************************************

using System;
using System.Runtime.Serialization;

namespace FS
{
	/// <summary>
	/// 配置文件不正确
	/// </summary>
	public class FarseerConfigException : FarseerException
    {
		/// <summary>
		///     构造函数
		/// </summary>
		public FarseerConfigException() { }
		/// <summary>
		///     构造函数
		/// </summary>
		/// <param name="message">Exception message</param>
		public FarseerConfigException(string message) : base(message) { }

		/// <summary>
		///     构造函数
		/// </summary>
		/// <param name="message">Exception message</param>
		/// <param name="innerException">Inner exception</param>
		public FarseerConfigException(string message, Exception innerException) : base(message, innerException) { }
	}
}