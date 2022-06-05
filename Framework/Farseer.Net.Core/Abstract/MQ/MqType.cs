namespace FS.Core.Abstract.MQ;

/// <summary>
/// MQ类型
/// </summary>
public enum MqType
{
    Rabbit,
    RedisStream,
    Queue,
    Kafka,
    Rocker
}