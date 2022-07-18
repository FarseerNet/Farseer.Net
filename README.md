## What is Farseer.net?

---
[English](https://github.com/FarseerNet/Farseer.Net) | [中文](https://github.com/FarseerNet/Farseer.Net/blob/main/README.zh-cn.md)

A standard framework for the **.net core** platform.

We have selected the most popular components for you and provide the use of these components in a modular way.

The framework perfectly supports **DDD domain-driven** technical implementations such as `repository`, `application-layer transactions`, `domain events`, `application-layer dynamic WebAPI`.

After using this framework, really focus only on your business, do not have to worry about technical implementation.

1. [Document](https://github.com/FarseerNet/Farseer.Net/tree/main/Doc)
2. [demo](https://github.com/FarseerNet/Farseer.Net/tree/main/Demo)

### What are the features?

---
**Elegant**

We use `IOC` technology throughout the framework and your business systems.

**Simple**

We use `AOP` technology so that you don't have to write additional non-business functional code such as transactions, caching, exception catching, logging, linking Track

**Lightweight**

The framework makes extensive use of `collection pooling` technology to make your application take up less memory.

**Tracking**

If you use Orm, Redis, Http, Grpc, Elasticsearch, MQ (Rabbit, RedisStream, Rocker, local Queue), EventBus, Task, FSS, etc. that we provide, you don't need to do anything, the system will implicitly implement link tracking for you and provide API request logs, slow queries (all of the previously mentioned will be logged).

[FOPS](https://github.com/FarseerNet/FOPS) Project (automatic build, link trace console, K8S cluster log collection) supports code non-intrusive full link real-time monitoring.

### What are the functions?

---
| Component | Description  |
|------|-----|
| Farseer.Net.Data | ORM component support: MySql/ClickHouse/Sqlserver/Sqlite/Oracle/Oledb (Access/Execl) databases |
| Farseer.Net.AspNetCore | Some packages based on asp.net core, such as exception middleware, CORS, link tracking portal, ioc injection web api |
| Farseer.Net.Cache | Modular packaging based on local cache MemoryCache |
| Farseer.Net.Cache.Redis | Modular packaging based on StackExchange.Redis |
| Farseer.Net.ElasticSearch | ES-based ORM packaging |
| Farseer.Net.EventBus | Event bus for lightweight in-process publish and subscribe |
| Farseer.Net.Fss | A client based on the `FSS` distributed scheduling platform to achieve highly available distributed task scheduling |
| Farseer.Net.LinkTrack | Full-link tracking and monitoring |
| Farseer.Net.Mapper | Object type conversion component |
| Farseer.Net.MongoDB | MongoDB component |
| Farseer.Net.MQ.Queue | RabbitMQ Message Queue Components |
| Farseer.Net.MQ.Queue | Process-level message queues (bulk consumption scenarios) |
| Farseer.Net.MQ.Rabbit | RabbitMQ Message Queue Components |
| Farseer.Net.MQ.RocketMQ | RocketMQ Message Queue Components |
| Farseer.Net.MQ.RedisStream | RedisStream Message Queue Components |
| Farseer.Net.Utils | Extended package of common tools |

### Link tracking manual operation：

---
Note: After marking, these methods will be logged by the link trace when they are called
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

---
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

---
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

---
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

---
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

---
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

---
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

---
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

---
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

---
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

---
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

---

`转换`
```c#
    UserVO vo = new UserPO().Map<UserVO>();
```