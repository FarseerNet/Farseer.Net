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
    [Track] // Here, if marked, the Execute1 and Execute2 methods do not need to specify Track again.
    public class TrackDemo
    {
        [Track]
        public void Execute1() { }
        
        // Although I did not mark [Track], it is marked in the class and will be inherited here.
        public void Execute2() { }

        public void Execute3()
        {
            // If you are recording a code snippet, you can use this method
            using (FsLinkTrack.Track($"Generally, the MethodName is passed here"))
            {
                // doSomething
            }
        }
    }
```

### Database ORM components：Farseer.Net.Data

---
Features: Almost the same performance as writing SQL by hand and populating it into a List collection
```c#
/// <summary>
/// Open transaction (specify database configuration name)
/// </summary>
[TransactionName("test")] // Turn on transaction tagging
public void AopTransactionByName()
{
    // Get all data
    MetaInfoContext.Data.Task.ToList();
    // Write to entity objects
    MetaInfoContext.Data.Task.InsertAsync(po);
    // Get a single object
    MetaInfoContext.Data.Task.Where(o => o.Id == id).ToEntity();
    // Modify an object
    MetaInfoContext.Data.Task.UpdateAsync(po);
}
```

### Redis components：Farseer.Net.Cache.Redis：

---
Features: Additional transaction bulk read and write operations.
```c#
  // Get a Redis Instance
  var redisCacheManager = IocManager.GetService<IRedisCacheManager>();
  // hashSet
  await redisCacheManager.Db.HashSetAsync("test_sync", "init", "value");
  // keyDelete
  redisCacheManager.Db.KeyDelete("test_async");
```

### Secondary cache component：Farseer.Net.Cache

---
Features: truly perfect decoupling of database and cache
```c#
// Define the key and set it to double-write for redis and local cache
var cacheServices = IocManager.Resolve<ICacheServices>();
cacheServices.SetProfilesInMemoryAndRedis<UserPO, int>("user", "default", o => o.Id, TimeSpan.FromSeconds(10));

// The cache is completely decoupled from database operations, and no code within the method is executed when the cache hits. It will be intercepted for processing
public class UserService
{
    /// <summary> Get the data set </summary>
    [Cache("user")]
    public IEnumerable<UserPO> ToList() => new DatabaseContext().ToList();
    /// <summary> Get the data set </summary>
    [Cache("user")]
    public UserPO ToEntity() => new DatabaseContext().ToList().FirstOrDefault();

    /// <summary> Simulate database add operation (cache must have a unique identifier) </summary>
    [CacheUpdate("user")]
    public UserPO Add(UserPO user)
    {
        new DatabaseContext().Add(user);
        return user;
    }

    /// <summary> Simulate database update data </summary>
    [CacheUpdate("user")]
    public UserPO Update(int id, UserPO user)
    {
        new DatabaseContext().Update(id, user);
        return user;
    }

    /// <summary> Simulate database deletion </summary>
    [CacheRemove("user")]
    public void Delete([CacheId] int id) => new DatabaseContext().Delete(id);
}
```

### es component：Farseer.Net.ElasticSearch：

---
Special: provide the same operation as database ORM
```c#
  var time = "30";
  // Judgment time (with complex local function methods)
  TestContext.Data.User.Where(o => o.CreateAt >= DateTime.Now.AddMinutes(-time.ConvertType(0)).ToTimestamps()).ToList();
  // NETS native conditionals and self-resolving conditionals
  TestContext.Data.User
             .Where(q => q.Term(t => t.Age, 33))
             .Where(o => o.UserName.Contains("ste")).ToList();
  // Fuzzy Search and Positive Order Sorting
  TestContext.Data.User.Where(o => o.Desc.Contains("hello")).Asc(o => o.Age).ToList();
  // Prefix search and reverse sorting
  TestContext.Data.User.Where(o => o.Desc.StartsWith("hello")).Desc(o => o.Age).ToList();
  // Suffix search (only Keyword type is supported)
  TestContext.Data.User.Where(o => o.UserName.EndsWith("en")).ToList();
  // Not equal to a value
  TestContext.Data.User.Where(o => o.UserName != "aaa").ToList();
  // and operation, if two Where methods are called, is also equivalent to using and
  TestContext.Data.User.Where(o => o.UserName == "steden" && o.Age == 18).ToList();
  // or operation
  TestContext.Data.User.Where(o => o.UserName == "steden" || o.Age >= 10).ToList();
```

### Rabbit component：Farseer.Net.MQ.Rabbit

---
#### Product
```c#
// get instance
var rabbitProduct = IocManager.GetService<IRabbitManager>("test").Product;
// send
rabbitProduct.Send(message: "test send message");
```
#### Consumer
```c#
/// <summary> Consumer Client </summary>
[Consumer(Enable = false, Name = "default", ExchangeName = "test", QueueName = "test", ExchangeType = eumExchangeType.fanout, DlxExchangeName = "DeadLetter")]
public class TestConsumer : IListenerMessage
{
    public bool Consumer(string message, object sender, BasicDeliverEventArgs ea)
    {
        System.Console.WriteLine(ea.ConsumerTag + "this message is:" + message);
        return true;
    }
}
```

### Event Bus component：Farseer.Net.EventBus

---
#### Product
```c#
IocManager.GetService<IEventProduct>(name: "test").Send(null, message: "hello event");
```
#### Consumer
```c#
/// <summary>
/// Test events
/// </summary>
[Consumer(EventName = "test")]
public class TestEvent : IListenerMessage
{
    public async Task<bool> Consumer(string message, object sender, DomainEventArgs ea)
    {
        Console.WriteLine($"{ea.Id} I subscribe to test's messages：time：{ea.CreateAt} message：{message}");
        return true;
    }
}
```

### Process-level message queues component：Farseer.Net.MQ.Queue

---
`Usage scenario: After sending data multiple times, centralized batch writing to ES, or database。`

`2.6 GHz Six cores Intel Core i7`
`16 GB 2400 MHz DDR4`
`Sending per second: 2,041,833 pieces of data`
#### Product
```c#
  // instance
  var queueProduct = IocManager.GetService<IQueueProduct>(name: "test");
  // send
  queueProduct.Send("test send message");
```
#### Consumer
```c#
  /// <summary>
  ///     Consumer Client
  /// </summary>
  [Consumer(Enable = true, Name = "test")]
  public class TestConsumer : IListenerMessage
  {
      public Task<bool> Consumer(List<object> queueList)
      {
          Console.WriteLine(value: $"get {queueList.Count} count");
          return Task.FromResult(result: true);
      }
      public Task<bool> FailureHandling(List<object> messages) => throw new NotImplementedException();
  }
```

### RedisStream component：Farseer.Net.MQ.RedisStream

---
#### Product
```c#
  // instance
  var redisStreamProduct = IocManager.GetService<IRedisStreamProduct>("test2");
  // send
  redisStreamProduct.Send(message: "test send message");
```
#### Consumer
```c#
  /// <summary>
  ///     Consumer Client
  /// </summary>
  [Consumer(Enable = true, RedisName = "default", GroupName = "", QueueName = "test2", PullCount = 2, ConsumeThreadNums = 1)]
  public class TestConsumer : IListenerMessage
  {
      public Task<bool> Consumer(ConsumeContext context)
      {
          foreach (var redisStreamMessage in context.RedisStreamMessages)
          {
              Console.WriteLine(value: "this message is:" + redisStreamMessage.Message);
              redisStreamMessage.Ack();
          }
  
          return Task.FromResult(result: true);
      }
  }
```

### Distributed Task Scheduling component：Farseer.Net.Fss

---
```c#
  [Fss] // Open before you can register to the FSS platform
  public class Program
  {
      public static void Main()
      {
          // Initialize
          FarseerApplication.Run<StartupModule>().Initialize();
          Thread.Sleep(millisecondsTimeout: -1);
      }
  }
    
  [FssJob(Name = "testJob")] // Name is consistent with the JobName configured in the FSS platform
  public class HelloWorldJob : IFssJob
  {
      /// <summary>
      ///     Execution of tasks
      /// </summary>
      public async Task<bool> Execute(IFssContext context)
      {
          // Tell the FSS platform that 20% of the current progress has been executed
          await context.SetProgressAsync(rate: 20);
  
          // Let the FSS platform, logging logs
          await context.LoggerAsync(logLevel: LogLevel.Information, log: "hello world！");
  
          // Next execution time is after 10 seconds (if not set, the time set by the task group is used)
          //context.SetNextAt(TimeSpan.FromSeconds(10));
  
          // success
          return true;
      }
  }
```

### Local task scheduling component：Farseer.Net.Tasks

---
```c#
  [Tasks] // Turn it on to automatically register JOBs in
  public class Program
  {
      public static void Main()
      {
          // Initialize
          FarseerApplication.Run<StartupModule>().Initialize();
          Thread.Sleep(millisecondsTimeout: -1);
      }
  }
    
  [Job(Interval = 200)] // Need to attach Job feature and set execution interval
  public class HelloWorldJob : IJob
  {
      /// <summary>
      ///     Execution of tasks
      /// </summary>
      public Task Execute(ITaskContext context)
      {
          // Let the job platform, logging logs
          context.Logger(logLevel: LogLevel.Information, log: "hello world！");

          context.SetNext(TimeSpan.FromSeconds(5));
          // success
          return Task.FromResult(0);
      }
  }
```

### Mapper component：Farseer.Net.Mapper

---

```c#
    UserVO vo = new UserPO().Map<UserVO>();
```