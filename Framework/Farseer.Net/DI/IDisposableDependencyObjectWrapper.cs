namespace Farseer.Net.DI
{
    /// <summary>
    ///     包装一个从Ioc容器中获取的对象的泛型接口
    ///     此对象继承自IDisposable
    /// </summary>
    public interface IDisposableDependencyObjectWrapper : IDisposableDependencyObjectWrapper<object> {}
}