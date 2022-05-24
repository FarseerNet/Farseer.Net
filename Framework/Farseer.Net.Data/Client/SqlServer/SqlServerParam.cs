using System.Data.Common;

namespace FS.Data.Client.SqlServer;

public class SqlServerParam : AbsDbParam
{
    public SqlServerParam(DbProviderFactory dbProviderFactory) : base(dbProviderFactory)
    {
    }

}