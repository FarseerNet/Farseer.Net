using FS.Data.Infrastructure;

namespace FS.Data.Client.SqlServer
{
    public class SqlServerFunctionProvider : AbsFunctionProvider
    {
        public override string GetColumns(string tableName) => $"sp_columns [{tableName}]";
    }
}