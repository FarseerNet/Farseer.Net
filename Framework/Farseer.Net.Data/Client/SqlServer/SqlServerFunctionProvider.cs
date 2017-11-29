using Farseer.Net.Data.Infrastructure;

namespace Farseer.Net.Data.Client.SqlServer
{
    public class SqlServerFunctionProvider : AbsFunctionProvider
    {
        public override string GetColumns(string tableName) => $"sp_columns [{tableName}]";
    }
}