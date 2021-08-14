## Farseer.net是什么?
针对`.net core` 平台下的一套开发标准制定。 

通过该标准，我们会为您选型出目前最为流行的常用组件，并按我们的标准（模块化）来提供如何使用这些组件。

我们选用`Castle.Windsor`作为我们的`IOC`框架，并制定使用该框架的一些规则。

`Farseer.Net`会告诉你以什么样的方式实现优雅的代码。

我们不造轮子，我们只是这些优秀的开源组件搬运工。

我们所有组件的使用标准，均无偿开源在`GitHub`上，并提供`Nuget`下载
    
### 使用Rabbit组件的示例：
`Program.cs`
```c#
[Rabbit]
class Program
{
    static void Main(string[] args)
    {
        // 项目启动时初始化
        FarseerApplication.Run<StartupModule>().Initialize();
        Thread.Sleep(-1);
    }
}
```
`StartupModule.cs`
```c#
/// <summary> 启动模块 </summary>
[DependsOn(typeof(RabbitModule))]
public class StartupModule : FarseerModule
{
    public override void PreInitialize() { }

    public override void PostInitialize() { }
}
```
`TestConsumer.cs`
```c#
/// <summary> 消费客户端 </summary>
[Consumer(Enable = false, Name = "default", ExchangeName = "test", QueueName = "test", ExchangeType = eumExchangeType.fanout, DlxExchangeName = "DeadLetter")]
public class TestConsumer : IListenerMessage
{
    public bool Consumer(string message, object sender, BasicDeliverEventArgs ea)
    {
        System.Console.WriteLine(ea.ConsumerTag + "接收到信息为:" + message);
        return true;
    }

    public bool FailureHandling(string message, object sender, BasicDeliverEventArgs ea) => throw new NotImplementedException();
}
```
以上三个class，就实现了`rabbit`的消费。
`Program.cs` 有个特性`[Rabbit]`,用来告诉`Rabbit模块`，是否启用消费端，开启后，会扫描所有消费端进行初始化并异步消费。

`StartupModule.cs` 是启动模块。`[DependsOn]`特性会告诉`Farseer.Net`，在项目启动时需要依赖哪些组件（比如上面的：`RabbitModule模块`，它也是继承`FarseerModule`）。
同时在模块中，会有初始化的方法。可以写入你希望初始化时执行的配置

`TestConsumer.cs` 是实际的消费执行端，通过`[Consumer]`特性，会知道当前要连接哪一个`Rabbit`，属于哪个队列、交换器。

并且在启动消费的时候，如果交换器、队列不存在时，会执行创建操作。

### 我们再来看另外的例子：数据库ORM操作：
`TaskPO.cs`
```c#
/// <summary> 数据库实体 </summary>
public class TaskPO
{
    /// <summary> 主键 </summary>
    [Field(Name = "id",IsPrimaryKey = true)]
    public int? Id { get; set; }
    
    /// <summary> 任务的标题 </summary>
    [Field(Name = "caption")]
    public string Caption { get; set; }
}
```
`MetaInfoContext.cs`
```c#
/// <summary> 数据库上下文 </summary>
public class MetaInfoContext : DbContext<MetaInfoContext>
{
    public MetaInfoContext() : base("default")
    {
    }
    
    public TableSet<TaskPO>   Task   { get; set; }

    protected override void CreateModelInit(Dictionary<string, SetDataMap> map)
    {
        // 设定数据库表名称
        map["Task"].SetName("task");
    }
}
```
`ITaskAgent.cs`
```c#
public interface ITaskAgent : ITransientDependency
{
    List<TaskPO> ToList();
    TaskPO ToEntity(int id);
}
```
`TaskAgent.cs`
```c#
/// <summary> 任务数据库层 </summary>
public class TaskAgent : ITaskAgent
{
    /// <summary> 获取所有任务列表 </summary>
    public List<TaskPO> ToList() => MetaInfoContext.Data.Task.ToList();

    /// <summary> 获取任务信息 </summary>
    public TaskPO ToEntity(int id) => MetaInfoContext.Data.Task.Where(o => o.Id == id).ToEntity();
}
```
`ITaskList.cs`
```c#
public interface ITaskList: ITransientDependency
{
    List<TaskVO> ToList();
    TaskVO ToInfo(int id);
}
```
`TaskServer.cs`
```c#
public class TaskServer : ITaskServer
{
    public ITaskAgent TaskAgent { get; set; }

    /// <summary> 获取任务信息 </summary>
    public TaskVO ToInfo(int id) => TaskAgent.ToEntity(id).Map<TaskVO>();
    
    /// <summary> 获取全部任务列表 </summary>
    public List<TaskVO> ToList() => TaskAgent.ToList().Map<TaskVO>();
}
```
可以看到，`TaskServer`使用了`Ioc`的`属性注入`，一切很自然。

在Mvc层调用`TaskServer`，同样是用`ITaskServer`接口通过`Ioc`来调用。

上面用到了数据库上下文的概念，并支持工作单元模式。在我们需要操作数据库方面是极为轻巧、简单的。

关与数据库操作、模块化理念的实际使用，可以参考我另外一个开源项目：分布式调度平台，地址：[GitHub](https://github.com/FarseerNet/FarseerSchedulerService)

当然，这里需要依赖`DataModule`模块：
`StartupModule.cs`
```c#
/// <summary> 启动模块 </summary>
[DependsOn(typeof(DataModule),typeof(RabbitModule))]
public class StartupModule : FarseerModule
{
    public override void PreInitialize() { }

    public override void PostInitialize() { }
}
```

### Farseer.net有哪些功能？
* `Farseer.Net.Data`：数据库ORM
  *  支持：MySql/ClickHouse/Sqlserver/Sqlite/Oracle/Oledb（Access/Execl） 数据库。 
* `Farseer.Net.AspNetCore`
  *  基于asp.net core的一些封装
* `Farseer.Net.Cache`
  *  基于本地缓存MemoryCache的模块化封装
* `Farseer.Net.Cache.Redis`
  *  基于StackExchange.Redis的模块化封装
* `Farseer.Net.Configuration`（计划废除）
  *  本地json配置解析
* `Farseer.Net.Core`
  *  基础依赖
* `Farseer.Net.ElasticSearch`
  *  基于NETS的模块化封装
* `Farseer.Net.Job`
  *  基于`FSS`分布式调度平台的客户端
* `Farseer.Net.LinkTrack`
  *  全链路追踪监控
* `Farseer.Net.Log`
  *  基于NLog的模块化封装
* `Farseer.Net.Mapper`
  *  基于AutoMapper的模块化封装
* `Farseer.Net.MongoDB`
  *  基于MongoDB.Driver的模块化封装
* `Farseer.Net.MQ`
  *  MQ的基类
* `Farseer.Net.MQ.Kafka`
  *  基于Confluent.Kafka的模块化封装
* `Farseer.Net.MQ.Rabbit`
  *  基于RabbitMQ.Client的模块化封装
* `Farseer.Net.MQ.RocketMQ`
  *  基于阿里云ONSClient4CPP的模块化封装
* `Farseer.Net.Utils`
  *  常用工具扩展封装
* `Farseer.Net.Web.Mvc`
### 有问题反馈
在使用中有任何问题，欢迎反馈给我，可以用以下联系方式跟我交流

* QQ群: 116228666
* 教程：http://www.cnblogs.com/steden/