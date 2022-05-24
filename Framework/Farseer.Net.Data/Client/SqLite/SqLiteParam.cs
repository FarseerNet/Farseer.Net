using System.Data.Common;

namespace FS.Data.Client.SqLite;

public class SqLiteParam : AbsDbParam
{
    public SqLiteParam(DbProviderFactory dbProviderFactory) : base(dbProviderFactory) { }
}