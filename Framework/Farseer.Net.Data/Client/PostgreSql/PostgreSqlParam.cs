using System.Data.Common;

namespace FS.Data.Client.PostgreSql;

public class PostgreSqlParam : AbsDbParam
{
    public PostgreSqlParam(DbProviderFactory dbProviderFactory) : base(dbProviderFactory)
    {
    }

}