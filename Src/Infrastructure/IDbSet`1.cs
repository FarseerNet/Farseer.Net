namespace FS.Infrastructure
{
    /// <summary>
    ///     扩展使用，所有的Set类型的继承
    /// </summary>
    /// <typeparam name="TEntity">实体</typeparam>
    public interface IDbSet<TEntity> : IDbSet where TEntity : class, new()
    {
    }
}