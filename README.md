## Farseer.net是什么?
针对 **.net core** 平台下的一套标准制定的框架。

我们为您选型出目前最为流行的组件，并按模块化来提供使用这些组件。

框架完美支持 **DDD领域驱动** 的技术实现，如`仓储资源库`、`应用层事务`、`领域事件`、`应用层动态WebAPI`。

使用了本框架后，真正实现了只关注您的业务，不用为技术实现而烦恼。

1. [文档](https://github.com/FarseerNet/Farseer.Net/tree/dev/Doc)
2. [demo](https://github.com/FarseerNet/Farseer.Net/tree/dev/Demo)

**优雅**

我们使用`IOC`技术，遍布整个框架及您的业务系统。

**简单**

我们使用`AOP`技术，让您无需额外编写非业务功能代码，如事务、缓存、异常捕获、日志、链路Track

**轻量**

框架内大量使用`集合池化`技术，使您的应用占用内存更小。

**链路追踪**

如果您使用我们提供的Orm、Redis、Http、Grpc、Elasticsearch、MQ(Rabbit、RedisStream、Rocker、本地Queue)、EventBus、Task、FSS等等，您什么都不需要做，系统将隐式为您实现链路追踪，并提供API请求日志、慢查询（前面提到的都会记录）。

结合[FOPS](https://github.com/FarseerNet/FOPS) 项目（自动构建、链路追踪控制台、K8S集群日志收集）支持代码无侵入的全链路实时监控。


### Farseer.net有哪些功能？
| 组件名称 | 描述  |
|------|-----|
| Farseer.Net.Data  | ORM组件支持：MySql/ClickHouse/Sqlserver/Sqlite/Oracle/Oledb（Access/Execl） 数据库 |
| Farseer.Net.AspNetCore  | 基于asp.net core的一些封装，如异常中间件、CORS、链路追踪入口、ioc注入web api |
| Farseer.Net.Cache  | 基于本地缓存MemoryCache的模块化封装 |
| Farseer.Net.Cache.Redis  | 基于StackExchange.Redis的模块化封装 |
| Farseer.Net.ElasticSearch  | 基于ES的ORM封装 |
| Farseer.Net.EventBus  | 事件总线，实现轻量级的进程内发布与订阅 |
| Farseer.Net.Fss  | 基于`FSS`分布式调度平台的客户端，实现高可用的分布式的任务调度 |
| Farseer.Net.LinkTrack  | 全链路追踪监控 |
| Farseer.Net.Mapper  | 对象类型转换组件 |
| Farseer.Net.MongoDB  | MongoDB 组件 |
| Farseer.Net.MQ.Queue  | RabbitMQ 消息队列组件 |
| Farseer.Net.MQ.Queue  | 进程级别的消息队列（批量消费场景） |
| Farseer.Net.MQ.Rabbit  | RabbitMQ 消息队列组件 |
| Farseer.Net.MQ.RocketMQ  | RocketMQ 消息队列组件 |
| Farseer.Net.MQ.RedisStream  | Redis5 消息队列组件 |
| Farseer.Net.Utils  | 常用工具扩展封装 |

### 链路追踪手动埋点：
说明：标记后，这些方法被调用时，将会被链路追踪记录
```c#
    [Track] // 此处如果标记了，则Execute1、Execute2方法不需要再指定Track
    public class TrackDemo
    {
        [Track]
        public void Execute1() { }
        
        // 虽然我没有标记[Track]，但类中已标记，此处也会继承。
        public void Execute2() { }

        public void Execute3()
        {
            // 如果是记录某个代码片断，可以使用此种方式
            using (FsLinkTrack.Track($"一般这里传的是MethodName"))
            {
                // doSomething
            }
        }
    }
```

### 数据库ORM组件：Farseer.Net.Data
特点：与手写SQL并填充到List集合的性能几乎一样
```c#
/// <summary>
/// 开启事务（指定数据库配置名称）
/// </summary>
[TransactionName("test")] // 开启事务标记
public void AopTransactionByName()
{
    // 获取所有数据
    MetaInfoContext.Data.Task.ToList();
    // 写入实体对象
    MetaInfoContext.Data.Task.InsertAsync(po);
    // 获取单个对象
    MetaInfoContext.Data.Task.Where(o => o.Id == id).ToEntity();
    // 修改对象
    MetaInfoContext.Data.Task.UpdateAsync(po);
}
```

### Redis组件：Farseer.Net.Cache.Redis：
特点：额外增加事务批量读写操作。
```c#
  // 取出Redis实例
  var redisCacheManager = IocManager.GetService<IRedisCacheManager>();
  // hashSet
  await redisCacheManager.Db.HashSetAsync("test_sync", "init", "value");
  // keyDelete
  redisCacheManager.Db.KeyDelete("test_async");
```

### 二级缓存组件：Farseer.Net.Cache
特点：真正实现了数据库与缓存的完美解耦
```c#
// 定义key，并设置为redis与本地缓存双写
var cacheServices = IocManager.Resolve<ICacheServices>();
cacheServices.SetProfilesInMemoryAndRedis<UserPO, int>("user", "default", o => o.Id, TimeSpan.FromSeconds(10));

// 缓存与数据库操作完全解耦，缓存命中时，不会执行方法内的代码。将被拦截处理
public class UserService
{
    /// <summary> 获取数据集合 </summary>
    [Cache("user")]
    public IEnumerable<UserPO> ToList() => new DatabaseContext().ToList();
    /// <summary> 获取数据集合 </summary>
    [Cache("user")]
    public UserPO ToEntity() => new DatabaseContext().ToList().FirstOrDefault();

    /// <summary> 模拟数据库添加操作（缓存必须有一个唯一标识） </summary>
    [CacheUpdate("user")]
    public UserPO Add(UserPO user)
    {
        new DatabaseContext().Add(user);
        return user;
    }

    /// <summary> 模拟数据库更新数据 </summary>
    [CacheUpdate("user")]
    public UserPO Update(int id, UserPO user)
    {
        new DatabaseContext().Update(id, user);
        return user;
    }

    /// <summary> 模拟数据库删除 </summary>
    [CacheRemove("user")]
    public void Delete([CacheId] int id) => new DatabaseContext().Delete(id);
}
```

### es组件：Farseer.Net.ElasticSearch：
特别：提供与数据库ORM一样的操作方式
```c#
  var time = "30";
  // 判断时间（并带有复杂的本地函数方法）
  TestContext.Data.User.Where(o => o.CreateAt >= DateTime.Now.AddMinutes(-time.ConvertType(0)).ToTimestamps()).ToList();
  // NETS原生的条件 + 自解析的条件
  TestContext.Data.User
             .Where(q => q.Term(t => t.Age, 33))
             .Where(o => o.UserName.Contains("ste")).ToList();
  // 模糊搜索 + 正序排序
  TestContext.Data.User.Where(o => o.Desc.Contains("我今年")).Asc(o => o.Age).ToList();
  // 前缀搜索 + 倒序排序
  TestContext.Data.User.Where(o => o.Desc.StartsWith("大家好")).Desc(o => o.Age).ToList();
  // 后缀搜索（只支持Keyword类型）
  TestContext.Data.User.Where(o => o.UserName.EndsWith("en")).ToList();
  // 不等于某个值
  TestContext.Data.User.Where(o => o.UserName != "aaa").ToList();
  // and 运算，如果两个Where方法调用，也相当于使用and
  TestContext.Data.User.Where(o => o.UserName == "steden" && o.Age == 18).ToList();
  // or 运行
  TestContext.Data.User.Where(o => o.UserName == "steden" || o.Age >= 10).ToList();
```

### Rabbit组件：Farseer.Net.MQ.Rabbit
#### 发送
```c#
// 取出实例
var rabbitProduct = IocManager.GetService<IRabbitManager>("test").Product;
// 发送
rabbitProduct.Send(message: "测试发送消息内容");
```
#### 消费
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
}
```


### 事件总线组件：Farseer.Net.EventBus
#### 发送
```c#
IocManager.GetService<IEventProduct>(name: "test").Send(null, message: "测试发送消息内容");
```
#### 消费
```c#
/// <summary>
/// 测试事件
/// </summary>
[Consumer(EventName = "test")]
public class TestEvent : IListenerMessage
{
    public async Task<bool> Consumer(string message, object sender, DomainEventArgs ea)
    {
        Console.WriteLine($"{ea.Id} 我订阅了test的消息：消息发送时间：{ea.CreateAt} 内容：{message}");
        return true;
    }
}
```
### 进程级别的消息队列组件：Farseer.Net.MQ.Queue
`使用场景：多次发送数据后，集中批量写入ES、或数据库。`

`2.6 GHz 六核Intel Core i7`
`16 GB 2400 MHz DDR4`
`每秒发送：2,041,833条数据`
#### 发送
```c#
  // 取出实例
  var queueProduct = IocManager.GetService<IQueueProduct>(name: "test");
  // 发送
  queueProduct.Send("测试发送消息内容");
```
#### 消费
```c#
  /// <summary>
  ///     消费客户端
  /// </summary>
  [Consumer(Enable = true, Name = "test")]
  public class TestConsumer : IListenerMessage
  {
      public Task<bool> Consumer(List<object> queueList)
      {
          Console.WriteLine(value: $"消费到{queueList.Count}条");
          return Task.FromResult(result: true);
      }
      public Task<bool> FailureHandling(List<object> messages) => throw new NotImplementedException();
  }
```

### RedisStream组件：Farseer.Net.MQ.RedisStream
#### 发送
```c#
  // 取出实例
  var redisStreamProduct = IocManager.GetService<IRedisStreamProduct>("test2");
  // 发送
  redisStreamProduct.Send(message: "测试发送消息内容");
```
#### 消费
```c#
  /// <summary>
  ///     消费客户端
  /// </summary>
  [Consumer(Enable = true, RedisName = "default", GroupName = "", QueueName = "test2", PullCount = 2, ConsumeThreadNums = 1)]
  public class TestConsumer : IListenerMessage
  {
      public Task<bool> Consumer(ConsumeContext context)
      {
          foreach (var redisStreamMessage in context.RedisStreamMessages)
          {
              Console.WriteLine(value: "接收到信息为:" + redisStreamMessage.Message);
              redisStreamMessage.Ack();
          }
  
          return Task.FromResult(result: true);
      }
  }
```

### 分布式任务调度组件：Farseer.Net.Fss
```c#
  [Fss] // 开启后，才能注册到FSS平台
  public class Program
  {
      public static void Main()
      {
          // 初始化模块
          FarseerApplication.Run<StartupModule>().Initialize();
          Thread.Sleep(millisecondsTimeout: -1);
      }
  }
    
  [FssJob(Name = "testJob")] // Name与FSS平台配置的JobName保持一致
  public class HelloWorldJob : IFssJob
  {
      /// <summary>
      ///     执行任务
      /// </summary>
      public async Task<bool> Execute(IFssContext context)
      {
          // 告诉FSS平台，当前进度执行了 20%
          await context.SetProgressAsync(rate: 20);
  
          // 让FSS平台，记录日志
          await context.LoggerAsync(logLevel: LogLevel.Information, log: "你好，世界！");
  
          // 下一次执行时间为10秒后（如果不设置，则使用任务组设置的时间）
          //context.SetNextAt(TimeSpan.FromSeconds(10));
  
          // 任务执行成功
          return true;
      }
  }
```

### 本地任务调度组件：Farseer.Net.Tasks
```c#
  [Tasks] // 开启后，才能把JOB自动注册进来
  public class Program
  {
      public static void Main()
      {
          // 初始化模块
          FarseerApplication.Run<StartupModule>().Initialize();
          Thread.Sleep(millisecondsTimeout: -1);
      }
  }
    
  [Job(Interval = 200)] // 需要附加Job特性，并设置执行间隔
  public class HelloWorldJob : IJob
  {
      /// <summary>
      ///     执行任务
      /// </summary>
      public Task Execute(ITaskContext context)
      {
          // 让FSS平台，记录日志
          context.Logger(logLevel: LogLevel.Information, log: "你好，世界！");

          context.SetNext(TimeSpan.FromSeconds(5));
          // 任务执行成功
          return Task.FromResult(0);
      }
  }
```

### Mapper组件：Farseer.Net.Mapper

`转换`
```c#
    UserVO vo = new UserPO().Map<UserVO>();
```