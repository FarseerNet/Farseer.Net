namespace FS.Infrastructure
{
    /// <summary> 通过实体类的继承后，后续Set、扩展方法提供针对主键的Where条件 </summary>
    /// <typeparam name="T"></typeparam>
    public interface IEntity<T>
    {
        /// <summary> 主键ID </summary>
        T ID { get; set; }
    }

    /// <summary> 通过实体类的继承后，后续Set、扩展方法提供针对主键的Where条件（默认为int?） </summary>
    public interface IEntity : IEntity<int?>
    {
    }
}