## Farseer.net是什么?
针对`.net core` 平台下的一套开发标准制定，提供一系列优雅的组件供使用。

通过该标准，我们会为您选型出目前最为流行的常用组件，并按我们的标准（模块化）来提供如何使用这些组件。

我们选用`Castle.Windsor`作为我们的`IOC`框架，并制定使用该框架的一些规则。

结合[FOPS](https://github.com/FarseerNet/FOPS) 项目（自动构建、链路追踪控制台）支持代码无侵入的全链路实时监控。

传送门：
1、[文档](https://github.com/FarseerNet/Farseer.Net/tree/main/Doc)
2、[demo](https://github.com/FarseerNet/Farseer.Net/tree/main/Demo)

### Farseer.Net.Data 数据库ORM组件：
```c#
  // 获取所有数据
  MetaInfoContext.Data.Task.ToList();
  // 写入实体对象
  MetaInfoContext.Data.Task.InsertAsync(po);
  // 获取单个对象
  MetaInfoContext.Data.Task.Where(o => o.Id == id).ToEntity();
  // 修改对象
  MetaInfoContext.Data.Task.UpdateAsync(po);
```

### Farseer.Net.Cache.Redis Redis组件：
```c#
  // 取出Redis实例
  var redisCacheManager = IocManager.GetService<IRedisCacheManager>();
  // hashSet
  await redisCacheManager.Db.HashSetAsync("test_sync", "init", "value");
  // keyDelete
  redisCacheManager.Db.KeyDelete("test_async");
```

### Farseer.Net.Cache 二级缓存组件
```c#
// 定义key，并设置为redis与本地缓存双写
public static CacheKey<TaskGroupVO, int> TaskGroupKey() => new($"FSS_TaskGroup", o => o.Id, EumCacheStoreType.MemoryAndRedis);

// 读取数据规则：优先读取本地缓存，不存在则读取Redis缓存，还是不存在，则读取数据库，并同步到Redis、本地缓存中
var key = CacheKeys.TaskGroupKey();
return RedisContext.Instance.CacheManager.GetListAsync(key, () => TaskGroupAgent.ToListAsync().MapAsync<TaskGroupVO, TaskGroupPO>());
```

### Farseer.Net.ElasticSearch es组件：
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

### Farseer.Net.MQ.Rabbit Rabbit组件：
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


### Farseer.Net.EventBus 事件总线组件：
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
### Farseer.Net.MQ.Queue 进程级别的消息队列组件：
`使用场景：多次发送数据后，集中批量写入ES、或数据库。`

`2.6 GHz 六核Intel Core i7`
`16 GB 2400 MHz DDR4`
`每秒发送：2,041,833条数据`
#### 发送
```c#
  // 取出实例
  var queueProduct = IocManager.GetService<IQueueManager>(name: "test").Product;
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

### Farseer.Net.MQ.RedisStream RedisStream组件：
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
      public Task<bool> Consumer(StreamEntry[] messages, ConsumeContext ea)
      {
          foreach (var message in messages)
          {
              Console.WriteLine(value: "接收到信息为:" + message.Values[0]);
              ea.Ack(message: message);
          }
  
          return Task.FromResult(result: true);
      }
  }
```

### Farseer.Net.Fss 分布式任务调度组件：
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

### Farseer.Net.Tasks 本地任务调度组件：
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

### Farseer.Net.Mapper Mapper组件：
`实体类`
```c#
  /// <summary>
  ///     会员账号实体
  /// </summary>
  [Map(typeof(UserVO),typeof(UserLoginVO))] // 告知Mapper组件，我可以Mapper的对象
  public class UserPO
  {
      /// <summary> </summary>
      [Field(Name = "ID", IsPrimaryKey = true, IsDbGenerated = true)]
      public int? Id { get; set; }
  }
```

`转换`
```c#
    UserVO vo = new UserPO().Map<UserVO>();
```

### Farseer.net有哪些功能？
* `Farseer.Net.Data`
  *  ORM组件支持：MySql/ClickHouse/Sqlserver/Sqlite/Oracle/Oledb（Access/Execl） 数据库。
* `Farseer.Net.AspNetCore`
  *  基于asp.net core的一些封装，如异常中间件、CORS、链路追踪入口、ioc注入web api
* `Farseer.Net.Cache`
  *  基于本地缓存MemoryCache的模块化封装
* `Farseer.Net.Cache.Redis`
  *  基于StackExchange.Redis的模块化封装
* `Farseer.Net.ElasticSearch`
  *  基于ES的ORM封装，于`Farseer.Net.Data`组件使用相似
* `Farseer.Net.EventBus`
  *  事件总线，实现轻量级的进程内发布与订阅。
* `Farseer.Net.Fss`
  *  基于`FSS`分布式调度平台的客户端，实现高可用的分布式的任务调度
* `Farseer.Net.LinkTrack`
  *  全链路追踪监控
* `Farseer.Net.Mapper`
  *  对象类型转换组件（基于AutoMapper)
* `Farseer.Net.MongoDB`
  *  MongoDB 组件
* `Farseer.Net.MQ.Queue`
  *  RabbitMQ 消息队列组件
* `Farseer.Net.MQ.Queue`
  *  进程级别的消息队列（批量消费场景）
* `Farseer.Net.MQ.Rabbit`
  *  RabbitMQ 消息队列组件
* `Farseer.Net.MQ.RocketMQ`
  *  RocketMQ 消息队列组件
* `Farseer.Net.MQ.RedisStream`
  * Redis5 消息队列组件
* `Farseer.Net.Utils`
  *  常用工具扩展封装