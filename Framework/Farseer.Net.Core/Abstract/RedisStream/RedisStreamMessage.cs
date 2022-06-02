namespace FS.Core.Abstract.RedisStream;

public class RedisStreamMessage
{
    /// <summary>
    /// 消息Id
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 消息内容
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// 单独提交指定消息
    /// </summary>
    public bool IsAck { get; set; }

    /// <summary>
    ///     单独提交指定消息，即使整批失败后，该条消息也会被Ack
    /// </summary>
    public void Ack() => IsAck = true;
}