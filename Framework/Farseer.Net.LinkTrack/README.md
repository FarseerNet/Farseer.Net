## 全链路实时监控框架
使用Farseer.Net框架提供的http、Redis、Orm、Elasticsearch、Mq(rabbit、rocket、redisStream)、http、grpc组件时，将支持代码无侵入、轻量级的全链路实时监控。
  
通过依赖本模块`LinkTrackModule`后，且配置了Elasticsearch（监控日志记录到ES），系统将会把所有上述提到的组件执行时间点一一记录到ES。

使用后，我们可以得知自己系统在一次api被请求后，我们系统在哪个时间点执行了哪些操作，且他们的耗时是多少。

本框架支持多系统间关联（需使用本框架提供的httpClient或GrpcClient），将会把所有操作关联到一次请求中，供我们查询所有操作耗时的时间分布图