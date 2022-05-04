using System;
using StackExchange.Redis;

namespace FS.Cache.Redis;

public class RedisLock : IDisposable
{
    private readonly IDatabase _db;
    private readonly string    _key;
    private readonly TimeSpan  _lockTime;
    private          bool      _canLock;

    internal RedisLock(IDatabase db, string key, TimeSpan lockTime)
    {
        _db            = db;
        this._key      = key;
        this._lockTime = lockTime;
    }

    /// <summary>
    /// 尝试加锁
    /// </summary>
    public bool TryLock()
    {
        return _canLock = _db.StringSet(_key, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), _lockTime, when: When.NotExists);
    }

    public void Dispose()
    {
        if (_canLock) _db.KeyDelete(_key);
    }
}