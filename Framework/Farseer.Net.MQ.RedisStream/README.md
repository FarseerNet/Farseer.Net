## Farseer.Net.MQ.RedisStream是什么?
是Redis 5.0后新增的Stream数据结构，用于支持完整的消息队列MessageQueue

通过本组件，可以让你更加简单的实现消息发送（生产）、消费

你也可以参考我另外写的两处代码：

https://github.com/FarseerNet/Farseer.Net/tree/master/Demo/Farseer.Net.MQ.RedisStreamDemo

https://github.com/FarseerNet/FarseerSchedulerService/tree/main/04_Component%EF%BC%88%E4%B8%9A%E5%8A%A1%E7%BB%84%E4%BB%B6%EF%BC%89/FSS.Com.MetaInfoServer/RunLog


### 使用组件的示例：
`Program.cs`
```c#
[RedisStream]
    class Program
    {
        static void Main(string[] args)
        {
            // 项目启动时初始化
            FarseerApplication.Run<StartupModule>().Initialize();

            // ******************** 以下演示消息发送 *********************
            var redisStreamProduct = IocManager.Instance.Resolve<IRedisStreamProduct>("test");
            redisStreamProduct.Send(DateTime.Now);
            redisStreamProduct.Send(DateTime.Now);
            redisStreamProduct.Send(DateTime.Now);
            var startNew = Stopwatch.StartNew();
            startNew.Start();
            Thread.Sleep(1000);
            startNew.Stop();
            // 以上也是JIT
            
            // ******************** 测试1秒内，能发送多少条消息 *********************
            var count = 0;
            startNew.Reset();
            startNew.Restart();
            while (startNew.ElapsedMilliseconds<1000)
            {
                redisStreamProduct.Send(DateTime.Now.ToString());
                //Thread.Sleep(10);
                count++;
            }

            Console.WriteLine(count);
            
            Thread.Sleep(-1);
        }
    }
```
`StartupModule.cs`
```c#
    /// <summary>
    /// 启动模块
    /// </summary>
    [DependsOn(typeof(RedisStreamModule))]
    public class StartupModule : FarseerModule
    {
    }
```
`TestConsumer.cs`
```c#
    /// <summary>
    /// 消费客户端
    /// </summary>
    [Consumer(Enable = true, RedisName = "default", GroupName = "test", QueueName = "test", PullCount = 2, ConsumeThreadNums = 1)]
    public class TestConsumer : IListenerMessage
    {
        public Task<bool> Consumer(string[] messages, ConsumeContext ea)
        {
            foreach (var message in messages)
            {
                System.Console.WriteLine("接收到信息为:" + message);
            }

            return Task.FromResult(true);
        }

        public Task<bool> FailureHandling(string[] messages, ConsumeContext ea) => throw new NotImplementedException();
    }
```
以上三个class，就实现了`redis`的消费。
`Program.cs` 有个特性`[RedisStream]`,用来告诉`RedisStream模块`，是否启用消费端，开启后，会扫描所有消费端进行初始化并异步消费。

`StartupModule.cs` 是启动模块。`[DependsOn]`特性会告诉`Farseer.Net`，在项目启动时需要依赖哪些组件（比如上面的：`RedisStreamModule模块`，它也是继承`FarseerModule`）。
同时在模块中，会有初始化的方法。可以写入你希望初始化时执行的配置

`TestConsumer.cs` 是实际的消费执行端，通过`[Consumer]`特性，会知道当前要连接哪一个`Redis`，属于哪个队列、消费组。

并且在启动消费的时候，如果消费组不存在时，会执行创建操作。


### 有问题反馈
在使用中有任何问题，欢迎反馈给我，可以用以下联系方式跟我交流

* QQ群: 116228666
* 教程：http://www.cnblogs.com/steden/