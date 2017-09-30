using System.Diagnostics;

namespace Farseer.Net.Utils.Common
{
    /// <summary>
    ///     获取版本信息
    /// </summary>
    public abstract class Assembly : System.Reflection.Assembly
    {
        /// <summary>
        ///     版本信息。
        /// </summary>
        private static readonly FileVersionInfo AssemblyFileVersion = FileVersionInfo.GetVersionInfo(GetExecutingAssembly().Location);

        /// <summary>
        ///     获得Assembly版本号
        /// </summary>
        public static string GetVersion()
        {
            return $"{AssemblyFileVersion.FileMajorPart}.{AssemblyFileVersion.FileMinorPart}.{AssemblyFileVersion.FileBuildPart}";
        }

        /// <summary>
        ///     获得Assembly产品名称
        /// </summary>
        public static string GetProductName()
        {
            return AssemblyFileVersion.ProductName;
        }

        /// <summary>
        ///     获得Assembly产品版权
        /// </summary>
        public static string GetCopyright()
        {
            return AssemblyFileVersion.LegalCopyright;
        }

        /// <summary>
        /// 行号
        /// </summary>
        public static int LineNumber
        {
            get
            {
                try
                {
                    //
                    // Get the trace information with file information by skipping
                    //   this function and then reading the top stack frame.
                    //
                    return (new StackTrace(1, true)).GetFrame(0).GetFileLineNumber();
                }
                catch {
                }

                return 0;
            }
        }
    }
}