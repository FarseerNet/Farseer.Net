using System.Data.Common;
using System.Text;
using FS.Data.Infrastructure;
using FS.Data.Internal;

namespace FS.Data.Client.OleDb
{
    /// <summary>
    ///     OleDb 数据库提供者（不同数据库的特性）
    /// </summary>
    public class OleDbProvider : AbsDbProvider
    {
        public override DbProviderFactory DbProviderFactory => System.Data.OleDb.OleDbFactory.Instance;
        public override AbsFunctionProvider FunctionProvider => new OleDbFunctionProvider();
        internal override AbsSqlBuilder CreateSqlBuilder(ExpressionBuilder expBuilder, string name)=> new OleDbBuilder(this, expBuilder, name);
        public override bool IsSupportTransaction => false;
        public override string CreateDbConnstring(string server, string port, string userID, string passWord = null, string catalog = null, string dataVer = null, string additional = null, int connectTimeout = 60, int poolMinSize = 16, int poolMaxSize = 100)
        {
            var sb = new StringBuilder($"Data Source={GetFilePath(server)};");
            switch (dataVer)
            {
                case "3.0":
                {
                    sb.Append("Provider=Microsoft.Jet.OLEDB.4.0;");
                    break;
                } //Extended Properties=Excel 3.0;
                case "4.0":
                {
                    sb.Append("Provider=Microsoft.Jet.OLEDB.4.0;");
                    break;
                } //Extended Properties=Excel 4.0;
                case "5.0":
                {
                    sb.Append("Provider=Microsoft.Jet.OLEDB.4.0;");
                    break;
                } //Extended Properties=Excel 5.0;
                case "95":
                {
                    sb.Append("Provider=Microsoft.Jet.OLEDB.4.0;");
                    break;
                } //Extended Properties=Excel 5.0;
                case "97":
                {
                    sb.Append("Provider=Microsoft.Jet.OLEDB.3.51;");
                    break;
                }
                case "2003":
                {
                    sb.Append("Provider=Microsoft.Jet.OLEDB.4.0;");
                    break;
                } //Extended Properties=Excel 8.0;
                //  2007+   DR=YES
                default:
                {
                    sb.Append("Provider=Microsoft.ACE.OLEDB.12.0;");
                    break;
                } //Extended Properties=Excel 12.0;
            }

            if (!string.IsNullOrWhiteSpace(userID)) { sb.Append($"User ID='{userID}';"); }
            if (!string.IsNullOrWhiteSpace(passWord)) { sb.Append($"Password='{passWord}';"); }

            if (poolMinSize > 0) { sb.Append($"Min Pool Size='{poolMinSize}';"); }
            if (poolMaxSize > 0) { sb.Append($"Max Pool Size='{poolMaxSize}';"); }
            if (connectTimeout > 0) { sb.Append($"Connect Timeout='{connectTimeout}';"); }

            sb.Append(additional);
            return sb.ToString();
        }
    }
}