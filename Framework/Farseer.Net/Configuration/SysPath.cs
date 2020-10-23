// ********************************************
// 作者：何达贤（steden） QQ：11042427
// 时间：2017-02-27 21:09
// ********************************************

using System;

namespace FS.Configuration
{
	/// <summary>
	///     获取系统路径
	/// </summary>
	public class SysPath
	{
		/// <summary>
		///     获取项目的App_Data的路径
		/// </summary>
		public static string AppData { get; set; } = AppContext.BaseDirectory + "/App_Data/";
		/// <summary>
		/// 配置文件名（可自己根据当前环境，设置不同的配置名称）
		/// </summary>
		public static string ConfigurationName { get; set; } = "Farseer.Net";

		/// <summary>
		/// 日志的路径保存位置
		/// </summary>
		public static string LogPath => AppData + "log/";

		/// <summary>
		/// Debug的路径保存位置
		/// </summary>
		public static string DebugPath => AppData + "log/Debug/";

		/// <summary>
		/// Error的路径保存位置
		/// </summary>
		public static string ErrorPath => AppData + "log/Error/";

		/// <summary>
		/// Fatal的路径保存位置
		/// </summary>
		public static string FatalPath => AppData + "log/Fatal/";

		/// <summary>
		/// Info的路径保存位置
		/// </summary>
		public static string InfoPath => AppData + "log/Info/";

		/// <summary>
		/// Warn的路径保存位置
		/// </summary>
		public static string WarnPath => AppData + "log/Warn/";

		/// <summary>
		/// Sql错误的路径保存位置
		/// </summary>
		public static string SqlErrorPath => AppData + "log/SqlError/";

		/// <summary>
		/// Sql运行的路径保存位置
		/// </summary>
		public static string SqlRunPath => AppData + "log/SqlRun/";
	}
}