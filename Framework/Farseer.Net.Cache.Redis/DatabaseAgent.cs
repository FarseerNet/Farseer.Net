using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FS.Core.LinkTrack;
using StackExchange.Redis;

namespace FS.Cache.Redis
{
    public class DatabaseAgent : IDatabase
    {
        private readonly IDatabase _db;

        public DatabaseAgent(IDatabase db)
        {
            _db = db;
        }

        public int            Database                                          => _db.Database;
        public Task<TimeSpan> PingAsync(CommandFlags flags = CommandFlags.None) => _db.PingAsync(flags);

        public bool                   TryWait(Task          task)                  => _db.TryWait(task);
        public void                   Wait(Task             task)                  => _db.Wait(task);
        public T                      Wait<T>(Task<T>       task)                  => _db.Wait<T>(task);
        public void                   WaitAll(params Task[] tasks)                 => _db.WaitAll(tasks);
        public IConnectionMultiplexer Multiplexer                                  => _db.Multiplexer;
        public TimeSpan               Ping(CommandFlags flags = CommandFlags.None) => _db.Ping(flags);

        public bool IsConnected(RedisKey key, CommandFlags flags = CommandFlags.None) => _db.IsConnected(key, flags);

        public Task KeyMigrateAsync(RedisKey key, EndPoint toServer, int toDatabase = 0, int timeoutMilliseconds = 0, MigrateOptions migrateOptions = MigrateOptions.None, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("KeyMigrateAsync", key))
            {
                return _db.KeyMigrateAsync(key, toServer, toDatabase, timeoutMilliseconds, migrateOptions, flags);
            }
        }

        public Task<RedisValue> DebugObjectAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("DebugObjectAsync", key))
            {
                return _db.DebugObjectAsync(key, flags);
            }
        }

        public Task<bool> GeoAddAsync(RedisKey key, double longitude, double latitude, RedisValue member, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("GeoAddAsync", key))
            {
                return _db.GeoAddAsync(key, longitude, latitude, member, flags);
            }
        }

        public Task<bool> GeoAddAsync(RedisKey key, GeoEntry value, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("GeoAddAsync", key))
            {
                return _db.GeoAddAsync(key, value, flags);
            }
        }

        public Task<long> GeoAddAsync(RedisKey key, GeoEntry[] values, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("GeoAddAsync", key))
            {
                return _db.GeoAddAsync(key, values, flags);
            }
        }

        public Task<bool> GeoRemoveAsync(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("GeoRemoveAsync", key))
            {
                return _db.GeoRemoveAsync(key, member, flags);
            }
        }

        public Task<double?> GeoDistanceAsync(RedisKey key, RedisValue member1, RedisValue member2, GeoUnit unit = GeoUnit.Meters, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("GeoDistanceAsync", key))
            {
                return _db.GeoDistanceAsync(key, member1, member2, unit, flags);
            }
        }

        public Task<string[]> GeoHashAsync(RedisKey key, RedisValue[] members, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("GeoHashAsync", key))
            {
                return _db.GeoHashAsync(key, members, flags);
            }
        }

        public Task<string> GeoHashAsync(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("GeoHashAsync", key))
            {
                return _db.GeoHashAsync(key, member, flags);
            }
        }

        public Task<GeoPosition?[]> GeoPositionAsync(RedisKey key, RedisValue[] members, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("GeoPositionAsync", key))
            {
                return _db.GeoPositionAsync(key, members, flags);
            }
        }

        public Task<GeoPosition?> GeoPositionAsync(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("GeoPositionAsync", key))
            {
                return _db.GeoPositionAsync(key, member, flags);
            }
        }

        public Task<GeoRadiusResult[]> GeoRadiusAsync(RedisKey key, RedisValue member, double radius, GeoUnit unit = GeoUnit.Meters, int count = -1, Order? order = null, GeoRadiusOptions options = GeoRadiusOptions.Default, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("GeoRadiusAsync", key))
            {
                return _db.GeoRadiusAsync(key, member, radius, unit, count, order, options, flags);
            }
        }

        public Task<GeoRadiusResult[]> GeoRadiusAsync(RedisKey key, double longitude, double latitude, double radius, GeoUnit unit = GeoUnit.Meters, int count = -1, Order? order = null, GeoRadiusOptions options = GeoRadiusOptions.Default, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("GeoRadiusAsync", key))
            {
                return _db.GeoRadiusAsync(key, longitude, latitude, radius, unit, count, order, options, flags);
            }
        }

        public Task<long> HashDecrementAsync(RedisKey key, RedisValue hashField, long value = 1, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("HashDecrementAsync", key, hashField))
            {
                return _db.HashDecrementAsync(key, hashField, value, flags);
            }
        }

        public Task<double> HashDecrementAsync(RedisKey key, RedisValue hashField, double value, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("HashDecrementAsync", key, hashField))
            {
                return _db.HashDecrementAsync(key, hashField, value, flags);
            }
        }

        public Task<bool> HashDeleteAsync(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("HashDeleteAsync", key, hashField))
            {
                return _db.HashDeleteAsync(key, hashField, flags);
            }
        }

        public Task<long> HashDeleteAsync(RedisKey key, RedisValue[] hashFields, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("HashDeleteAsync", key, string.Join(",", hashFields)))
            {
                return _db.HashDeleteAsync(key, hashFields, flags);
            }
        }

        public Task<bool> HashExistsAsync(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("HashExistsAsync", key, hashField))
            {
                return _db.HashExistsAsync(key, hashField, flags);
            }
        }

        public Task<RedisValue> HashGetAsync(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("HashGetAsync", key, hashField))
            {
                return _db.HashGetAsync(key, hashField, flags);
            }
        }

        public Task<Lease<byte>> HashGetLeaseAsync(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("HashGetLeaseAsync", key, hashField))
            {
                return _db.HashGetLeaseAsync(key, hashField, flags);
            }
        }

        public Task<RedisValue[]> HashGetAsync(RedisKey key, RedisValue[] hashFields, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("HashGetAsync", key, String.Join(",", hashFields)))
            {
                return _db.HashGetAsync(key, hashFields, flags);
            }
        }

        public Task<HashEntry[]> HashGetAllAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("HashGetAllAsync", key))
            {
                return _db.HashGetAllAsync(key, flags);
            }
        }

        public Task<long> HashIncrementAsync(RedisKey key, RedisValue hashField, long value = 1, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("HashIncrementAsync", key, hashField))
            {
                return _db.HashIncrementAsync(key, hashField, value, flags);
            }
        }

        public Task<double> HashIncrementAsync(RedisKey key, RedisValue hashField, double value, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("HashIncrementAsync", key, hashField))
            {
                return _db.HashIncrementAsync(key, hashField, value, flags);
            }
        }

        public Task<RedisValue[]> HashKeysAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("HashKeysAsync", key))
            {
                return _db.HashKeysAsync(key, flags);
            }
        }

        public Task<long> HashLengthAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("HashLengthAsync", key))
            {
                return _db.HashLengthAsync(key, flags);
            }
        }

        public IAsyncEnumerable<HashEntry> HashScanAsync(RedisKey key, RedisValue pattern = new RedisValue(), int pageSize = 250, long cursor = 0, int pageOffset = 0, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("HashScanAsync", key))
            {
                return _db.HashScanAsync(key, pattern, pageSize, cursor, pageOffset, flags);
            }
        }

        public Task HashSetAsync(RedisKey key, HashEntry[] hashFields, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("HashSetAsync", key, String.Join(",", hashFields)))
            {
                return _db.HashSetAsync(key, hashFields, flags);
            }
        }

        public Task<bool> HashSetAsync(RedisKey key, RedisValue hashField, RedisValue value, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("HashSetAsync", key, hashField))
            {
                return _db.HashSetAsync(key, hashField, value, when, flags);
            }
        }

        public Task<long> HashStringLengthAsync(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("HashStringLengthAsync", key, hashField))
            {
                return _db.HashStringLengthAsync(key, hashField, flags);
            }
        }

        public Task<RedisValue[]> HashValuesAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("HashValuesAsync", key))
            {
                return _db.HashValuesAsync(key, flags);
            }
        }

        public Task<bool> HyperLogLogAddAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("HyperLogLogAddAsync", key))
            {
                return _db.HyperLogLogAddAsync(key, value, flags);
            }
        }

        public Task<bool> HyperLogLogAddAsync(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("HyperLogLogAddAsync", key))
            {
                return _db.HyperLogLogAddAsync(key, values, flags);
            }
        }

        public Task<long> HyperLogLogLengthAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("HyperLogLogLengthAsync", key))
            {
                return _db.HyperLogLogLengthAsync(key, flags);
            }
        }

        public Task<long> HyperLogLogLengthAsync(RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("HyperLogLogLengthAsync"))
            {
                return _db.HyperLogLogLengthAsync(keys, flags);
            }
        }

        public Task HyperLogLogMergeAsync(RedisKey destination, RedisKey first, RedisKey second, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("HyperLogLogMergeAsync"))
            {
                return _db.HyperLogLogMergeAsync(destination, first, second, flags);
            }
        }

        public Task HyperLogLogMergeAsync(RedisKey destination, RedisKey[] sourceKeys, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("HyperLogLogMergeAsync"))
            {
                return _db.HyperLogLogMergeAsync(destination, sourceKeys, flags);
            }
        }

        public Task<EndPoint> IdentifyEndpointAsync(RedisKey key = new RedisKey(), CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("IdentifyEndpointAsync", key))
            {
                return _db.IdentifyEndpointAsync(key, flags);
            }
        }

        public Task<bool> KeyDeleteAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("KeyDeleteAsync", key))
            {
                return _db.KeyDeleteAsync(key, flags);
            }
        }

        public Task<long> KeyDeleteAsync(RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("KeyDeleteAsync"))
            {
                return _db.KeyDeleteAsync(keys, flags);
            }
        }

        public Task<byte[]> KeyDumpAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("KeyDumpAsync", key))
            {
                return _db.KeyDumpAsync(key, flags);
            }
        }

        public Task<bool> KeyExistsAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("KeyExistsAsync", key))
            {
                return _db.KeyExistsAsync(key, flags);
            }
        }

        public Task<long> KeyExistsAsync(RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("KeyExistsAsync"))
            {
                return _db.KeyExistsAsync(keys, flags);
            }
        }

        public Task<bool> KeyExpireAsync(RedisKey key, TimeSpan? expiry, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("KeyExpireAsync", key))
            {
                return _db.KeyExpireAsync(key, expiry, flags);
            }
        }

        public Task<bool> KeyExpireAsync(RedisKey key, DateTime? expiry, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("KeyExpireAsync", key))
            {
                return _db.KeyExpireAsync(key, expiry, flags);
            }
        }

        public Task<TimeSpan?> KeyIdleTimeAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("KeyIdleTimeAsync", key))
            {
                return _db.KeyIdleTimeAsync(key, flags);
            }
        }

        public Task<bool> KeyMoveAsync(RedisKey key, int database, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("KeyMoveAsync", key))
            {
                return _db.KeyMoveAsync(key, database, flags);
            }
        }

        public Task<bool> KeyPersistAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("KeyPersistAsync", key))
            {
                return _db.KeyPersistAsync(key, flags);
            }
        }

        public Task<RedisKey> KeyRandomAsync(CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("KeyRandomAsync"))
            {
                return _db.KeyRandomAsync(flags);
            }
        }

        public Task<bool> KeyRenameAsync(RedisKey key, RedisKey newKey, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("KeyRenameAsync", key))
            {
                return _db.KeyRenameAsync(key, newKey, when, flags);
            }
        }

        public Task KeyRestoreAsync(RedisKey key, byte[] value, TimeSpan? expiry = null, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("KeyRestoreAsync", key))
            {
                return _db.KeyRestoreAsync(key, value, expiry, flags);
            }
        }

        public Task<TimeSpan?> KeyTimeToLiveAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("KeyTimeToLiveAsync", key))
            {
                return _db.KeyTimeToLiveAsync(key, flags);
            }
        }

        public Task<RedisType> KeyTypeAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("KeyTypeAsync", key))
            {
                return _db.KeyTypeAsync(key, flags);
            }
        }

        public Task<RedisValue> ListGetByIndexAsync(RedisKey key, long index, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("ListGetByIndexAsync", key))
            {
                return _db.ListGetByIndexAsync(key, index, flags);
            }
        }

        public Task<long> ListInsertAfterAsync(RedisKey key, RedisValue pivot, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("ListInsertAfterAsync", key))
            {
                return _db.ListInsertAfterAsync(key, pivot, value, flags);
            }
        }

        public Task<long> ListInsertBeforeAsync(RedisKey key, RedisValue pivot, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("ListInsertBeforeAsync", key))
            {
                return _db.ListInsertBeforeAsync(key, pivot, value, flags);
            }
        }

        public Task<RedisValue> ListLeftPopAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("ListLeftPopAsync", key))
            {
                return _db.ListLeftPopAsync(key, flags);
            }
        }

        public Task<long> ListLeftPushAsync(RedisKey key, RedisValue value, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("ListLeftPushAsync", key))
            {
                return _db.ListLeftPushAsync(key, value, when, flags);
            }
        }

        public Task<long> ListLeftPushAsync(RedisKey key, RedisValue[] values, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("ListLeftPushAsync", key))
            {
                return _db.ListLeftPushAsync(key, values, when, flags);
            }
        }

        public Task<long> ListLeftPushAsync(RedisKey key, RedisValue[] values, CommandFlags flags)
        {
            using (FsLinkTrack.TrackRedis("ListLeftPushAsync", key))
            {
                return _db.ListLeftPushAsync(key, values, flags);
            }
        }

        public Task<long> ListLengthAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("ListLengthAsync", key))
            {
                return _db.ListLengthAsync(key, flags);
            }
        }

        public Task<RedisValue[]> ListRangeAsync(RedisKey key, long start = 0, long stop = -1, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("ListRangeAsync", key))
            {
                return _db.ListRangeAsync(key, start, stop, flags);
            }
        }

        public Task<long> ListRemoveAsync(RedisKey key, RedisValue value, long count = 0, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("ListRemoveAsync", key))
            {
                return _db.ListRemoveAsync(key, value, count, flags);
            }
        }

        public Task<RedisValue> ListRightPopAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("ListRightPopAsync", key))
            {
                return _db.ListRightPopAsync(key, flags);
            }
        }

        public Task<RedisValue> ListRightPopLeftPushAsync(RedisKey source, RedisKey destination, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("ListRightPopLeftPushAsync"))
            {
                return _db.ListRightPopLeftPushAsync(source, destination, flags);
            }
        }

        public Task<long> ListRightPushAsync(RedisKey key, RedisValue value, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("ListRightPushAsync", key))
            {
                return _db.ListRightPushAsync(key, value, when, flags);
            }
        }

        public Task<long> ListRightPushAsync(RedisKey key, RedisValue[] values, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("ListRightPushAsync", key))
            {
                return _db.ListRightPushAsync(key, values, when, flags);
            }
        }

        public Task<long> ListRightPushAsync(RedisKey key, RedisValue[] values, CommandFlags flags)
        {
            using (FsLinkTrack.TrackRedis("ListRightPushAsync", key))
            {
                return _db.ListRightPushAsync(key, values, flags);
            }
        }

        public Task ListSetByIndexAsync(RedisKey key, long index, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("ListSetByIndexAsync", key))
            {
                return _db.ListSetByIndexAsync(key, index, value, flags);
            }
        }

        public Task ListTrimAsync(RedisKey key, long start, long stop, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("ListTrimAsync", key))
            {
                return _db.ListTrimAsync(key, start, stop, flags);
            }
        }

        public Task<bool> LockExtendAsync(RedisKey key, RedisValue value, TimeSpan expiry, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("LockExtendAsync", key))
            {
                return _db.LockExtendAsync(key, value, expiry, flags);
            }
        }

        public Task<RedisValue> LockQueryAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("LockQueryAsync", key))
            {
                return _db.LockQueryAsync(key, flags);
            }
        }

        public Task<bool> LockReleaseAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("LockReleaseAsync", key))
            {
                return _db.LockReleaseAsync(key, value, flags);
            }
        }

        public Task<bool> LockTakeAsync(RedisKey key, RedisValue value, TimeSpan expiry, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("LockTakeAsync", key))
            {
                return _db.LockTakeAsync(key, value, expiry, flags);
            }
        }

        public Task<long> PublishAsync(RedisChannel channel, RedisValue message, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("PublishAsync"))
            {
                return _db.PublishAsync(channel, message, flags);
            }
        }

        public Task<RedisResult> ExecuteAsync(string command, params object[] args)
        {
            using (FsLinkTrack.TrackRedis("ExecuteAsync"))
            {
                return _db.ExecuteAsync(command, args);
            }
        }

        public Task<RedisResult> ExecuteAsync(string command, ICollection<object> args, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("ExecuteAsync"))
            {
                return _db.ExecuteAsync(command, args, flags);
            }
        }

        public Task<RedisResult> ScriptEvaluateAsync(string script, RedisKey[] keys = null, RedisValue[] values = null, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("ScriptEvaluateAsync"))
            {
                return _db.ScriptEvaluateAsync(script, keys, values, flags);
            }
        }

        public Task<RedisResult> ScriptEvaluateAsync(byte[] hash, RedisKey[] keys = null, RedisValue[] values = null, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("ScriptEvaluateAsync"))
            {
                return _db.ScriptEvaluateAsync(hash, keys, values, flags);
            }
        }

        public Task<RedisResult> ScriptEvaluateAsync(LuaScript script, object parameters = null, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("ScriptEvaluateAsync"))
            {
                return _db.ScriptEvaluateAsync(script, parameters, flags);
            }
        }

        public Task<RedisResult> ScriptEvaluateAsync(LoadedLuaScript script, object parameters = null, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("ScriptEvaluateAsync"))
            {
                return _db.ScriptEvaluateAsync(script, parameters, flags);
            }
        }

        public Task<bool> SetAddAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SetAddAsync", key))
            {
                return _db.SetAddAsync(key, value, flags);
            }
        }

        public Task<long> SetAddAsync(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SetAddAsync", key))
            {
                return _db.SetAddAsync(key, values, flags);
            }
        }

        public Task<RedisValue[]> SetCombineAsync(SetOperation operation, RedisKey first, RedisKey second, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SetCombineAsync"))
            {
                return _db.SetCombineAsync(operation, first, second, flags);
            }
        }

        public Task<RedisValue[]> SetCombineAsync(SetOperation operation, RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SetCombineAsync"))
            {
                return _db.SetCombineAsync(operation, keys, flags);
            }
        }

        public Task<long> SetCombineAndStoreAsync(SetOperation operation, RedisKey destination, RedisKey first, RedisKey second, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SetCombineAndStoreAsync"))
            {
                return _db.SetCombineAndStoreAsync(operation, destination, first, second, flags);
            }
        }

        public Task<long> SetCombineAndStoreAsync(SetOperation operation, RedisKey destination, RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SetCombineAndStoreAsync"))
            {
                return _db.SetCombineAndStoreAsync(operation, destination, keys, flags);
            }
        }

        public Task<bool> SetContainsAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SetContainsAsync", key))
            {
                return _db.SetContainsAsync(key, value, flags);
            }
        }

        public Task<long> SetLengthAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SetLengthAsync", key))
            {
                return _db.SetLengthAsync(key, flags);
            }
        }

        public Task<RedisValue[]> SetMembersAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SetMembersAsync", key))
            {
                return _db.SetMembersAsync(key, flags);
            }
        }

        public Task<bool> SetMoveAsync(RedisKey source, RedisKey destination, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SetMoveAsync"))
            {
                return _db.SetMoveAsync(source, destination, value, flags);
            }
        }

        public Task<RedisValue> SetPopAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SetPopAsync", key))
            {
                return _db.SetPopAsync(key, flags);
            }
        }

        public Task<RedisValue[]> SetPopAsync(RedisKey key, long count, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SetPopAsync", key))
            {
                return _db.SetPopAsync(key, count, flags);
            }
        }

        public Task<RedisValue> SetRandomMemberAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SetRandomMemberAsync", key))
            {
                return _db.SetRandomMemberAsync(key, flags);
            }
        }

        public Task<RedisValue[]> SetRandomMembersAsync(RedisKey key, long count, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SetRandomMembersAsync", key))
            {
                return _db.SetRandomMembersAsync(key, count, flags);
            }
        }

        public Task<bool> SetRemoveAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SetRemoveAsync", key))
            {
                return _db.SetRemoveAsync(key, value, flags);
            }
        }

        public Task<long> SetRemoveAsync(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SetRemoveAsync", key))
            {
                return _db.SetRemoveAsync(key, values, flags);
            }
        }

        public Task<RedisValue[]> SortAsync(RedisKey key, long skip = 0, long take = -1, Order order = Order.Ascending, SortType sortType = SortType.Numeric, RedisValue @by = new RedisValue(), RedisValue[] get = null, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SortAsync", key))
            {
                return _db.SortAsync(key, skip, take, order, sortType, by, get, flags);
            }
        }

        public Task<long> SortAndStoreAsync(RedisKey destination, RedisKey key, long skip = 0, long take = -1, Order order = Order.Ascending, SortType sortType = SortType.Numeric, RedisValue @by = new RedisValue(), RedisValue[] get = null, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SortAndStoreAsync", key))
            {
                return _db.SortAndStoreAsync(destination, key, skip, take, order, sortType, by, get, flags);
            }
        }

        public Task<bool> SortedSetAddAsync(RedisKey key, RedisValue member, double score, CommandFlags flags)
        {
            using (FsLinkTrack.TrackRedis("SortedSetAddAsync", key))
            {
                return _db.SortedSetAddAsync(key, member, score, flags);
            }
        }

        public Task<bool> SortedSetAddAsync(RedisKey key, RedisValue member, double score, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SortedSetAddAsync", key))
            {
                return _db.SortedSetAddAsync(key, member, score, when, flags);
            }
        }

        public Task<long> SortedSetAddAsync(RedisKey key, SortedSetEntry[] values, CommandFlags flags)
        {
            using (FsLinkTrack.TrackRedis("SortedSetAddAsync", key))
            {
                return _db.SortedSetAddAsync(key, values, flags);
            }
        }

        public Task<long> SortedSetAddAsync(RedisKey key, SortedSetEntry[] values, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SortedSetAddAsync", key))
            {
                return _db.SortedSetAddAsync(key, values, when, flags);
            }
        }

        public Task<long> SortedSetCombineAndStoreAsync(SetOperation operation, RedisKey destination, RedisKey first, RedisKey second, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SortedSetCombineAndStoreAsync"))
            {
                return _db.SortedSetCombineAndStoreAsync(operation, destination, first, second, aggregate, flags);
            }
        }

        public Task<long> SortedSetCombineAndStoreAsync(SetOperation operation, RedisKey destination, RedisKey[] keys, double[] weights = null, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SortedSetCombineAndStoreAsync"))
            {
                return _db.SortedSetCombineAndStoreAsync(operation, destination, keys, weights, aggregate, flags);
            }
        }

        public Task<double> SortedSetDecrementAsync(RedisKey key, RedisValue member, double value, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SortedSetDecrementAsync", key))
            {
                return _db.SortedSetDecrementAsync(key, member, value, flags);
            }
        }

        public Task<double> SortedSetIncrementAsync(RedisKey key, RedisValue member, double value, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SortedSetIncrementAsync", key))
            {
                return _db.SortedSetIncrementAsync(key, member, value, flags);
            }
        }

        public Task<long> SortedSetLengthAsync(RedisKey key, double min = double.NegativeInfinity, double max = double.PositiveInfinity, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SortedSetLengthAsync", key))
            {
                return _db.SortedSetLengthAsync(key, min, max, exclude, flags);
            }
        }

        public Task<long> SortedSetLengthByValueAsync(RedisKey key, RedisValue min, RedisValue max, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SortedSetLengthByValueAsync", key))
            {
                return _db.SortedSetLengthByValueAsync(key, min, max, exclude, flags);
            }
        }

        public Task<RedisValue[]> SortedSetRangeByRankAsync(RedisKey key, long start = 0, long stop = -1, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SortedSetRangeByRankAsync", key))
            {
                return _db.SortedSetRangeByRankAsync(key, start, stop, order, flags);
            }
        }

        public Task<SortedSetEntry[]> SortedSetRangeByRankWithScoresAsync(RedisKey key, long start = 0, long stop = -1, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SortedSetRangeByRankWithScoresAsync", key))
            {
                return _db.SortedSetRangeByRankWithScoresAsync(key, start, stop, order, flags);
            }
        }

        public Task<RedisValue[]> SortedSetRangeByScoreAsync(RedisKey key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SortedSetRangeByScoreAsync", key))
            {
                return _db.SortedSetRangeByScoreAsync(key, start, stop, exclude, order, skip, take, flags);
            }
        }

        public Task<SortedSetEntry[]> SortedSetRangeByScoreWithScoresAsync(RedisKey key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SortedSetRangeByScoreWithScoresAsync", key))
            {
                return _db.SortedSetRangeByScoreWithScoresAsync(key, start, stop, exclude, order, skip, take, flags);
            }
        }

        public Task<RedisValue[]> SortedSetRangeByValueAsync(RedisKey key, RedisValue min, RedisValue max, Exclude exclude, long skip, long take = -1, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SortedSetRangeByValueAsync", key))
            {
                return _db.SortedSetRangeByValueAsync(key, min, max, exclude, skip, take, flags);
            }
        }

        public Task<RedisValue[]> SortedSetRangeByValueAsync(RedisKey key, RedisValue min = new RedisValue(), RedisValue max = new RedisValue(), Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SortedSetRangeByValueAsync", key))
            {
                return _db.SortedSetRangeByValueAsync(key, min, max, exclude, order, skip, take, flags);
            }
        }

        public Task<long?> SortedSetRankAsync(RedisKey key, RedisValue member, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SortedSetRankAsync", key))
            {
                return _db.SortedSetRankAsync(key, member, order, flags);
            }
        }

        public Task<bool> SortedSetRemoveAsync(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SortedSetRemoveAsync", key))
            {
                return _db.SortedSetRemoveAsync(key, member, flags);
            }
        }

        public Task<long> SortedSetRemoveAsync(RedisKey key, RedisValue[] members, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SortedSetRemoveAsync", key))
            {
                return _db.SortedSetRemoveAsync(key, members, flags);
            }
        }

        public Task<long> SortedSetRemoveRangeByRankAsync(RedisKey key, long start, long stop, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SortedSetRemoveRangeByRankAsync", key))
            {
                return _db.SortedSetRemoveRangeByRankAsync(key, start, stop, flags);
            }
        }

        public Task<long> SortedSetRemoveRangeByScoreAsync(RedisKey key, double start, double stop, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SortedSetRemoveRangeByScoreAsync", key))
            {
                return _db.SortedSetRemoveRangeByScoreAsync(key, start, stop, exclude, flags);
            }
        }

        public Task<long> SortedSetRemoveRangeByValueAsync(RedisKey key, RedisValue min, RedisValue max, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SortedSetRemoveRangeByValueAsync", key))
            {
                return _db.SortedSetRemoveRangeByValueAsync(key, min, max, exclude, flags);
            }
        }

        public IAsyncEnumerable<RedisValue> SetScanAsync(RedisKey key, RedisValue pattern = new RedisValue(), int pageSize = 250, long cursor = 0, int pageOffset = 0, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SetScanAsync", key))
            {
                return _db.SetScanAsync(key, pattern, pageSize, cursor, pageOffset, flags);
            }
        }

        public IAsyncEnumerable<SortedSetEntry> SortedSetScanAsync(RedisKey key, RedisValue pattern = new RedisValue(), int pageSize = 250, long cursor = 0, int pageOffset = 0, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SortedSetScanAsync", key))
            {
                return _db.SortedSetScanAsync(key, pattern, pageSize, cursor, pageOffset, flags);
            }
        }

        public Task<double?> SortedSetScoreAsync(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SortedSetScoreAsync", key))
            {
                return _db.SortedSetScoreAsync(key, member, flags);
            }
        }

        public Task<SortedSetEntry?> SortedSetPopAsync(RedisKey key, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SortedSetPopAsync", key))
            {
                return _db.SortedSetPopAsync(key, order, flags);
            }
        }

        public Task<SortedSetEntry[]> SortedSetPopAsync(RedisKey key, long count, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SortedSetPopAsync", key))
            {
                return _db.SortedSetPopAsync(key, count, order, flags);
            }
        }

        public Task<long> StreamAcknowledgeAsync(RedisKey key, RedisValue groupName, RedisValue messageId, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StreamAcknowledgeAsync", key))
            {
                return _db.StreamAcknowledgeAsync(key, groupName, messageId, flags);
            }
        }

        public Task<long> StreamAcknowledgeAsync(RedisKey key, RedisValue groupName, RedisValue[] messageIds, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StreamAcknowledgeAsync", key))
            {
                return _db.StreamAcknowledgeAsync(key, groupName, messageIds, flags);
            }
        }

        public Task<RedisValue> StreamAddAsync(RedisKey key, RedisValue streamField, RedisValue streamValue, RedisValue? messageId = null, int? maxLength = null, bool useApproximateMaxLength = false, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StreamAddAsync", key))
            {
                return _db.StreamAddAsync(key, streamField, streamValue, messageId, maxLength, useApproximateMaxLength, flags);
            }
        }

        public Task<RedisValue> StreamAddAsync(RedisKey key, NameValueEntry[] streamPairs, RedisValue? messageId = null, int? maxLength = null, bool useApproximateMaxLength = false, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StreamAddAsync", key))
            {
                return _db.StreamAddAsync(key, streamPairs, messageId, maxLength, useApproximateMaxLength, flags);
            }
        }

        public Task<StreamEntry[]> StreamClaimAsync(RedisKey key, RedisValue consumerGroup, RedisValue claimingConsumer, long minIdleTimeInMs, RedisValue[] messageIds, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StreamClaimAsync", key))
            {
                return _db.StreamClaimAsync(key, consumerGroup, claimingConsumer, minIdleTimeInMs, messageIds, flags);
            }
        }

        public Task<RedisValue[]> StreamClaimIdsOnlyAsync(RedisKey key, RedisValue consumerGroup, RedisValue claimingConsumer, long minIdleTimeInMs, RedisValue[] messageIds, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StreamClaimIdsOnlyAsync", key))
            {
                return _db.StreamClaimIdsOnlyAsync(key, consumerGroup, claimingConsumer, minIdleTimeInMs, messageIds, flags);
            }
        }

        public Task<bool> StreamConsumerGroupSetPositionAsync(RedisKey key, RedisValue groupName, RedisValue position, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StreamConsumerGroupSetPositionAsync", key))
            {
                return _db.StreamConsumerGroupSetPositionAsync(key, groupName, position, flags);
            }
        }

        public Task<StreamConsumerInfo[]> StreamConsumerInfoAsync(RedisKey key, RedisValue groupName, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StreamConsumerInfoAsync", key))
            {
                return _db.StreamConsumerInfoAsync(key, groupName, flags);
            }
        }

        public Task<bool> StreamCreateConsumerGroupAsync(RedisKey key, RedisValue groupName, RedisValue? position, CommandFlags flags)
        {
            using (FsLinkTrack.TrackRedis("StreamCreateConsumerGroupAsync", key))
            {
                return _db.StreamCreateConsumerGroupAsync(key, groupName, position, flags);
            }
        }

        public Task<bool> StreamCreateConsumerGroupAsync(RedisKey key, RedisValue groupName, RedisValue? position = null, bool createStream = true, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StreamCreateConsumerGroupAsync", key))
            {
                return _db.StreamCreateConsumerGroupAsync(key, groupName, position, createStream, flags);
            }
        }

        public Task<long> StreamDeleteAsync(RedisKey key, RedisValue[] messageIds, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StreamDeleteAsync", key))
            {
                return _db.StreamDeleteAsync(key, messageIds, flags);
            }
        }

        public Task<long> StreamDeleteConsumerAsync(RedisKey key, RedisValue groupName, RedisValue consumerName, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StreamDeleteConsumerAsync", key))
            {
                return _db.StreamDeleteConsumerAsync(key, groupName, consumerName, flags);
            }
        }

        public Task<bool> StreamDeleteConsumerGroupAsync(RedisKey key, RedisValue groupName, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StreamDeleteConsumerGroupAsync", key))
            {
                return _db.StreamDeleteConsumerGroupAsync(key, groupName, flags);
            }
        }

        public Task<StreamGroupInfo[]> StreamGroupInfoAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StreamGroupInfoAsync", key))
            {
                return _db.StreamGroupInfoAsync(key, flags);
            }
        }

        public Task<StreamInfo> StreamInfoAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StreamInfoAsync", key))
            {
                return _db.StreamInfoAsync(key, flags);
            }
        }

        public Task<long> StreamLengthAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StreamLengthAsync", key))
            {
                return _db.StreamLengthAsync(key, flags);
            }
        }

        public Task<StreamPendingInfo> StreamPendingAsync(RedisKey key, RedisValue groupName, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StreamPendingAsync", key))
            {
                return _db.StreamPendingAsync(key, groupName, flags);
            }
        }

        public Task<StreamPendingMessageInfo[]> StreamPendingMessagesAsync(RedisKey key, RedisValue groupName, int count, RedisValue consumerName, RedisValue? minId = null, RedisValue? maxId = null, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StreamPendingMessagesAsync", key))
            {
                return _db.StreamPendingMessagesAsync(key, groupName, count, consumerName, minId, maxId, flags);
            }
        }

        public Task<StreamEntry[]> StreamRangeAsync(RedisKey key, RedisValue? minId = null, RedisValue? maxId = null, int? count = null, Order messageOrder = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StreamRangeAsync", key))
            {
                return _db.StreamRangeAsync(key, minId, maxId, count, messageOrder, flags);
            }
        }

        public Task<StreamEntry[]> StreamReadAsync(RedisKey key, RedisValue position, int? count = null, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StreamReadAsync", key))
            {
                return _db.StreamReadAsync(key, position, count, flags);
            }
        }

        public Task<RedisStream[]> StreamReadAsync(StreamPosition[] streamPositions, int? countPerStream = null, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StreamReadAsync"))
            {
                return _db.StreamReadAsync(streamPositions, countPerStream, flags);
            }
        }

        public Task<StreamEntry[]> StreamReadGroupAsync(RedisKey key, RedisValue groupName, RedisValue consumerName, RedisValue? position, int? count, CommandFlags flags)
        {
            using (FsLinkTrack.TrackRedis("StreamReadGroupAsync", key))
            {
                return _db.StreamReadGroupAsync(key, groupName, consumerName, position, count, flags);
            }
        }

        public Task<StreamEntry[]> StreamReadGroupAsync(RedisKey key, RedisValue groupName, RedisValue consumerName, RedisValue? position = null, int? count = null, bool noAck = false, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StreamReadGroupAsync", key))
            {
                return _db.StreamReadGroupAsync(key, groupName, consumerName, position, count, noAck, flags);
            }
        }

        public Task<RedisStream[]> StreamReadGroupAsync(StreamPosition[] streamPositions, RedisValue groupName, RedisValue consumerName, int? countPerStream, CommandFlags flags)
        {
            using (FsLinkTrack.TrackRedis("StreamReadGroupAsync"))
            {
                return _db.StreamReadGroupAsync(streamPositions, groupName, consumerName, countPerStream, flags);
            }
        }

        public Task<RedisStream[]> StreamReadGroupAsync(StreamPosition[] streamPositions, RedisValue groupName, RedisValue consumerName, int? countPerStream = null, bool noAck = false, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StreamReadGroupAsync"))
            {
                return _db.StreamReadGroupAsync(streamPositions, groupName, consumerName, countPerStream, noAck, flags);
            }
        }

        public Task<long> StreamTrimAsync(RedisKey key, int maxLength, bool useApproximateMaxLength = false, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StreamTrimAsync", key))
            {
                return _db.StreamTrimAsync(key, maxLength, useApproximateMaxLength, flags);
            }
        }

        public Task<long> StringAppendAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StringAppendAsync", key))
            {
                return _db.StringAppendAsync(key, value, flags);
            }
        }

        public Task<long> StringBitCountAsync(RedisKey key, long start = 0, long end = -1, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StringBitCountAsync", key))
            {
                return _db.StringBitCountAsync(key, start, end, flags);
            }
        }

        public Task<long> StringBitOperationAsync(Bitwise operation, RedisKey destination, RedisKey first, RedisKey second = new RedisKey(), CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StringBitOperationAsync"))
            {
                return _db.StringBitOperationAsync(operation, destination, first, second, flags);
            }
        }

        public Task<long> StringBitOperationAsync(Bitwise operation, RedisKey destination, RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StringBitOperationAsync"))
            {
                return _db.StringBitOperationAsync(operation, destination, keys, flags);
            }
        }

        public Task<long> StringBitPositionAsync(RedisKey key, bool bit, long start = 0, long end = -1, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StringBitPositionAsync", key))
            {
                return _db.StringBitPositionAsync(key, bit, start, end, flags);
            }
        }

        public Task<long> StringDecrementAsync(RedisKey key, long value = 1, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StringDecrementAsync", key))
            {
                return _db.StringDecrementAsync(key, value, flags);
            }
        }

        public Task<double> StringDecrementAsync(RedisKey key, double value, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StringDecrementAsync", key))
            {
                return _db.StringDecrementAsync(key, value, flags);
            }
        }

        public Task<RedisValue> StringGetAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StringGetAsync", key))
            {
                return _db.StringGetAsync(key, flags);
            }
        }

        public Task<RedisValue[]> StringGetAsync(RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StringGetAsync"))
            {
                return _db.StringGetAsync(keys, flags);
            }
        }

        public Task<Lease<byte>> StringGetLeaseAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StringGetLeaseAsync", key))
            {
                return _db.StringGetLeaseAsync(key, flags);
            }
        }

        public Task<bool> StringGetBitAsync(RedisKey key, long offset, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StringGetBitAsync", key))
            {
                return _db.StringGetBitAsync(key, offset, flags);
            }
        }

        public Task<RedisValue> StringGetRangeAsync(RedisKey key, long start, long end, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StringGetRangeAsync", key))
            {
                return _db.StringGetRangeAsync(key, start, end, flags);
            }
        }

        public Task<RedisValue> StringGetSetAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StringGetSetAsync", key))
            {
                return _db.StringGetSetAsync(key, value, flags);
            }
        }

        public Task<RedisValueWithExpiry> StringGetWithExpiryAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StringGetWithExpiryAsync", key))
            {
                return _db.StringGetWithExpiryAsync(key, flags);
            }
        }

        public Task<long> StringIncrementAsync(RedisKey key, long value = 1, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StringIncrementAsync", key))
            {
                return _db.StringIncrementAsync(key, value, flags);
            }
        }

        public Task<double> StringIncrementAsync(RedisKey key, double value, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StringIncrementAsync", key))
            {
                return _db.StringIncrementAsync(key, value, flags);
            }
        }

        public Task<long> StringLengthAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StringLengthAsync", key))
            {
                return _db.StringLengthAsync(key, flags);
            }
        }

        public Task<bool> StringSetAsync(RedisKey key, RedisValue value, TimeSpan? expiry = null, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StringSetAsync", key))
            {
                return _db.StringSetAsync(key, value, expiry, when, flags);
            }
        }

        public Task<bool> StringSetAsync(KeyValuePair<RedisKey, RedisValue>[] values, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StringSetAsync"))
            {
                return _db.StringSetAsync(values, when, flags);
            }
        }

        public Task<bool> StringSetBitAsync(RedisKey key, long offset, bool bit, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StringSetBitAsync", key))
            {
                return _db.StringSetBitAsync(key, offset, bit, flags);
            }
        }

        public Task<RedisValue> StringSetRangeAsync(RedisKey key, long offset, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StringSetRangeAsync", key))
            {
                return _db.StringSetRangeAsync(key, offset, value, flags);
            }
        }

        public Task<bool> KeyTouchAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("KeyTouchAsync", key))
            {
                return _db.KeyTouchAsync(key, flags);
            }
        }

        public Task<long> KeyTouchAsync(RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("KeyTouchAsync"))
            {
                return _db.KeyTouchAsync(keys, flags);
            }
        }

        public IBatch CreateBatch(object asyncState = null) => _db.CreateBatch(asyncState);

        public ITransaction CreateTransaction(object asyncState = null) => _db.CreateTransaction(asyncState);

        public void KeyMigrate(RedisKey key, EndPoint toServer, int toDatabase = 0, int timeoutMilliseconds = 0, MigrateOptions migrateOptions = MigrateOptions.None, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("KeyMigrate", key))
                _db.KeyMigrate(key, toServer, toDatabase, timeoutMilliseconds, migrateOptions, flags);
        }

        public RedisValue DebugObject(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("DebugObject", key))
            {
                return _db.DebugObject(key, flags);
            }
        }

        public bool GeoAdd(RedisKey key, double longitude, double latitude, RedisValue member, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("GeoAdd", key))
            {
                return _db.GeoAdd(key, longitude, latitude, member, flags);
            }
        }

        public bool GeoAdd(RedisKey key, GeoEntry value, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("GeoAdd", key))
            {
                return _db.GeoAdd(key, value, flags);
            }
        }

        public long GeoAdd(RedisKey key, GeoEntry[] values, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("GeoAdd", key))
            {
                return _db.GeoAdd(key, values, flags);
            }
        }

        public bool GeoRemove(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("GeoRemove", key))
            {
                return _db.GeoRemove(key, member, flags);
            }
        }

        public double? GeoDistance(RedisKey key, RedisValue member1, RedisValue member2, GeoUnit unit = GeoUnit.Meters, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("GeoDistance", key))
            {
                return _db.GeoDistance(key, member1, member2, unit, flags);
            }
        }

        public string[] GeoHash(RedisKey key, RedisValue[] members, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("GeoHash", key))
            {
                return _db.GeoHash(key, members, flags);
            }
        }

        public string GeoHash(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("GeoHash", key))
            {
                return _db.GeoHash(key, member, flags);
            }
        }

        public GeoPosition?[] GeoPosition(RedisKey key, RedisValue[] members, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("GeoPosition", key))
            {
                return _db.GeoPosition(key, members, flags);
            }
        }

        public GeoPosition? GeoPosition(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("GeoPosition", key))
            {
                return _db.GeoPosition(key, member, flags);
            }
        }

        public GeoRadiusResult[] GeoRadius(RedisKey key, RedisValue member, double radius, GeoUnit unit = GeoUnit.Meters, int count = -1, Order? order = null, GeoRadiusOptions options = GeoRadiusOptions.Default, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("GeoRadius", key))
            {
                return _db.GeoRadius(key, member, radius, unit, count, order, options, flags);
            }
        }

        public GeoRadiusResult[] GeoRadius(RedisKey key, double longitude, double latitude, double radius, GeoUnit unit = GeoUnit.Meters, int count = -1, Order? order = null, GeoRadiusOptions options = GeoRadiusOptions.Default, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("GeoRadius", key))
            {
                return _db.GeoRadius(key, longitude, latitude, radius, unit, count, order, options, flags);
            }
        }

        public long HashDecrement(RedisKey key, RedisValue hashField, long value = 1, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("HashDecrement", key, hashField))
            {
                return _db.HashDecrement(key, hashField, value, flags);
            }
        }

        public double HashDecrement(RedisKey key, RedisValue hashField, double value, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("HashDecrement", key, hashField))
            {
                return _db.HashDecrement(key, hashField, value, flags);
            }
        }

        public bool HashDelete(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("HashDelete", key, hashField))
            {
                return _db.HashDelete(key, hashField, flags);
            }
        }

        public long HashDelete(RedisKey key, RedisValue[] hashFields, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("HashDelete", key, String.Join(",", hashFields)))
            {
                return _db.HashDelete(key, hashFields, flags);
            }
        }

        public bool HashExists(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("HashExists", key, hashField))
            {
                return _db.HashExists(key, hashField, flags);
            }
        }

        public RedisValue HashGet(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("HashGet", key, hashField))
            {
                return _db.HashGet(key, hashField, flags);
            }
        }

        public Lease<byte> HashGetLease(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("HashGetLease", key, hashField))
            {
                return _db.HashGetLease(key, hashField, flags);
            }
        }

        public RedisValue[] HashGet(RedisKey key, RedisValue[] hashFields, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("HashGet", key, String.Join(",", hashFields)))
            {
                return _db.HashGet(key, hashFields, flags);
            }
        }

        public HashEntry[] HashGetAll(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("HashGetAll", key))
            {
                return _db.HashGetAll(key, flags);
            }
        }

        public long HashIncrement(RedisKey key, RedisValue hashField, long value = 1, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("HashIncrement", key, hashField))
            {
                return _db.HashIncrement(key, hashField, value, flags);
            }
        }

        public double HashIncrement(RedisKey key, RedisValue hashField, double value, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("HashIncrement", key, hashField))
            {
                return _db.HashIncrement(key, hashField, value, flags);
            }
        }

        public RedisValue[] HashKeys(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("HashKeys", key))
            {
                return _db.HashKeys(key, flags);
            }
        }

        public long HashLength(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("HashLength", key))
            {
                return _db.HashLength(key, flags);
            }
        }

        public IEnumerable<HashEntry> HashScan(RedisKey key, RedisValue pattern, int pageSize, CommandFlags flags)
        {
            using (FsLinkTrack.TrackRedis("HashScan", key))
            {
                return _db.HashScan(key, pattern, pageSize, flags);
            }
        }

        public IEnumerable<HashEntry> HashScan(RedisKey key, RedisValue pattern = new RedisValue(), int pageSize = 250, long cursor = 0, int pageOffset = 0, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("HashScan", key))
            {
                return _db.HashScan(key, pattern, pageSize, cursor, pageOffset, flags);
            }
        }

        public void HashSet(RedisKey key, HashEntry[] hashFields, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("HashSet", key, String.Join(",", hashFields)))
                _db.HashSet(key, hashFields, flags);
        }

        public bool HashSet(RedisKey key, RedisValue hashField, RedisValue value, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("HashSet", key, hashField))
            {
                return _db.HashSet(key, hashField, value, when, flags);
            }
        }

        public long HashStringLength(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("HashStringLength", key, hashField))
            {
                return _db.HashStringLength(key, hashField, flags);
            }
        }

        public RedisValue[] HashValues(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("HashValues", key))
            {
                return _db.HashValues(key, flags);
            }
        }

        public bool HyperLogLogAdd(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("HyperLogLogAdd", key))
            {
                return _db.HyperLogLogAdd(key, value, flags);
            }
        }

        public bool HyperLogLogAdd(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("HyperLogLogAdd", key))
            {
                return _db.HyperLogLogAdd(key, values, flags);
            }
        }

        public long HyperLogLogLength(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("HyperLogLogLength", key))
            {
                return _db.HyperLogLogLength(key, flags);
            }
        }

        public long HyperLogLogLength(RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("HyperLogLogLength"))
            {
                return _db.HyperLogLogLength(keys, flags);
            }
        }

        public void HyperLogLogMerge(RedisKey destination, RedisKey first, RedisKey second, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("HyperLogLogMerge"))
                _db.HyperLogLogMerge(destination, first, second, flags);
        }

        public void HyperLogLogMerge(RedisKey destination, RedisKey[] sourceKeys, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("HyperLogLogMerge"))
                _db.HyperLogLogMerge(destination, sourceKeys, flags);
        }

        public EndPoint IdentifyEndpoint(RedisKey key = new RedisKey(), CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("IdentifyEndpoint", key))
            {
                return _db.IdentifyEndpoint(key, flags);
            }
        }

        public bool KeyDelete(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("KeyDelete", key))
            {
                return _db.KeyDelete(key, flags);
            }
        }

        public long KeyDelete(RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("KeyDelete"))
            {
                return _db.KeyDelete(keys, flags);
            }
        }

        public byte[] KeyDump(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("KeyDump", key))
            {
                return _db.KeyDump(key, flags);
            }
        }

        public bool KeyExists(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("KeyExists", key))
            {
                return _db.KeyExists(key, flags);
            }
        }

        public long KeyExists(RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("KeyExists"))
            {
                return _db.KeyExists(keys, flags);
            }
        }

        public bool KeyExpire(RedisKey key, TimeSpan? expiry, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("KeyExpire", key))
            {
                return _db.KeyExpire(key, expiry, flags);
            }
        }

        public bool KeyExpire(RedisKey key, DateTime? expiry, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("KeyExpire", key))
            {
                return _db.KeyExpire(key, expiry, flags);
            }
        }

        public TimeSpan? KeyIdleTime(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("KeyIdleTime", key))
            {
                return _db.KeyIdleTime(key, flags);
            }
        }

        public bool KeyMove(RedisKey key, int database, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("KeyMove", key))
            {
                return _db.KeyMove(key, database, flags);
            }
        }

        public bool KeyPersist(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("KeyPersist", key))
            {
                return _db.KeyPersist(key, flags);
            }
        }

        public RedisKey KeyRandom(CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("KeyRandom"))
            {
                return _db.KeyRandom(flags);
            }
        }

        public bool KeyRename(RedisKey key, RedisKey newKey, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("KeyRename", key))
            {
                return _db.KeyRename(key, newKey, when, flags);
            }
        }

        public void KeyRestore(RedisKey key, byte[] value, TimeSpan? expiry = null, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("KeyRestore", key))
                _db.KeyRestore(key, value, expiry, flags);
        }

        public TimeSpan? KeyTimeToLive(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("KeyTimeToLive", key))
            {
                return _db.KeyTimeToLive(key, flags);
            }
        }

        public RedisType KeyType(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("KeyType", key))
            {
                return _db.KeyType(key, flags);
            }
        }

        public RedisValue ListGetByIndex(RedisKey key, long index, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("ListGetByIndex", key))
            {
                return _db.ListGetByIndex(key, index, flags);
            }
        }

        public long ListInsertAfter(RedisKey key, RedisValue pivot, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("ListInsertAfter", key))
            {
                return _db.ListInsertAfter(key, pivot, value, flags);
            }
        }

        public long ListInsertBefore(RedisKey key, RedisValue pivot, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("ListInsertBefore", key))
            {
                return _db.ListInsertBefore(key, pivot, value, flags);
            }
        }

        public RedisValue ListLeftPop(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("ListLeftPop", key))
            {
                return _db.ListLeftPop(key, flags);
            }
        }

        public long ListLeftPush(RedisKey key, RedisValue value, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("ListLeftPush", key))
            {
                return _db.ListLeftPush(key, value, when, flags);
            }
        }

        public long ListLeftPush(RedisKey key, RedisValue[] values, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("ListLeftPush", key))
            {
                return _db.ListLeftPush(key, values, when, flags);
            }
        }

        public long ListLeftPush(RedisKey key, RedisValue[] values, CommandFlags flags)
        {
            using (FsLinkTrack.TrackRedis("ListLeftPush", key))
            {
                return _db.ListLeftPush(key, values, flags);
            }
        }

        public long ListLength(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("ListLength", key))
            {
                return _db.ListLength(key, flags);
            }
        }

        public RedisValue[] ListRange(RedisKey key, long start = 0, long stop = -1, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("ListRange", key))
            {
                return _db.ListRange(key, start, stop, flags);
            }
        }

        public long ListRemove(RedisKey key, RedisValue value, long count = 0, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("ListRemove", key))
            {
                return _db.ListRemove(key, value, count, flags);
            }
        }

        public RedisValue ListRightPop(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("ListRightPop", key))
            {
                return _db.ListRightPop(key, flags);
            }
        }

        public RedisValue ListRightPopLeftPush(RedisKey source, RedisKey destination, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("ListRightPopLeftPush"))
            {
                return _db.ListRightPopLeftPush(source, destination, flags);
            }
        }

        public long ListRightPush(RedisKey key, RedisValue value, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("ListRightPush", key))
            {
                return _db.ListRightPush(key, value, when, flags);
            }
        }

        public long ListRightPush(RedisKey key, RedisValue[] values, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("ListRightPush", key))
            {
                return _db.ListRightPush(key, values, when, flags);
            }
        }

        public long ListRightPush(RedisKey key, RedisValue[] values, CommandFlags flags)
        {
            using (FsLinkTrack.TrackRedis("ListRightPush", key))
            {
                return _db.ListRightPush(key, values, flags);
            }
        }

        public void ListSetByIndex(RedisKey key, long index, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("ListSetByIndex", key))
                _db.ListSetByIndex(key, index, value, flags);
        }

        public void ListTrim(RedisKey key, long start, long stop, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("ListTrim", key))
                _db.ListTrim(key, start, stop, flags);
        }

        public bool LockExtend(RedisKey key, RedisValue value, TimeSpan expiry, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("LockExtend", key))
            {
                return _db.LockExtend(key, value, expiry, flags);
            }
        }

        public RedisValue LockQuery(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("LockQuery", key))
            {
                return _db.LockQuery(key, flags);
            }
        }

        public bool LockRelease(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("LockRelease", key))
            {
                return _db.LockRelease(key, value, flags);
            }
        }

        public bool LockTake(RedisKey key, RedisValue value, TimeSpan expiry, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("LockTake", key))
            {
                return _db.LockTake(key, value, expiry, flags);
            }
        }

        public long Publish(RedisChannel channel, RedisValue message, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("Publish"))
            {
                return _db.Publish(channel, message, flags);
            }
        }

        public RedisResult Execute(string command, params object[] args)
        {
            using (FsLinkTrack.TrackRedis("Execute"))
            {
                return _db.Execute(command, args);
            }
        }

        public RedisResult Execute(string command, ICollection<object> args, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("Execute"))
            {
                return _db.Execute(command, args, flags);
            }
        }

        public RedisResult ScriptEvaluate(string script, RedisKey[] keys = null, RedisValue[] values = null, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("ScriptEvaluate"))
            {
                return _db.ScriptEvaluate(script, keys, values, flags);
            }
        }

        public RedisResult ScriptEvaluate(byte[] hash, RedisKey[] keys = null, RedisValue[] values = null, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("ScriptEvaluate"))
            {
                return _db.ScriptEvaluate(hash, keys, values, flags);
            }
        }

        public RedisResult ScriptEvaluate(LuaScript script, object parameters = null, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("ScriptEvaluate"))
            {
                return _db.ScriptEvaluate(script, parameters, flags);
            }
        }

        public RedisResult ScriptEvaluate(LoadedLuaScript script, object parameters = null, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("ScriptEvaluate"))
            {
                return _db.ScriptEvaluate(script, parameters, flags);
            }
        }

        public bool SetAdd(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SetAdd", key))
            {
                return _db.SetAdd(key, value, flags);
            }
        }

        public long SetAdd(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SetAdd", key))
            {
                return _db.SetAdd(key, values, flags);
            }
        }

        public RedisValue[] SetCombine(SetOperation operation, RedisKey first, RedisKey second, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SetCombine"))
            {
                return _db.SetCombine(operation, first, second, flags);
            }
        }

        public RedisValue[] SetCombine(SetOperation operation, RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SetCombine"))
            {
                return _db.SetCombine(operation, keys, flags);
            }
        }

        public long SetCombineAndStore(SetOperation operation, RedisKey destination, RedisKey first, RedisKey second, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SetCombineAndStore"))
            {
                return _db.SetCombineAndStore(operation, destination, first, second, flags);
            }
        }

        public long SetCombineAndStore(SetOperation operation, RedisKey destination, RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SetCombineAndStore"))
            {
                return _db.SetCombineAndStore(operation, destination, keys, flags);
            }
        }

        public bool SetContains(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SetContains", key))
            {
                return _db.SetContains(key, value, flags);
            }
        }

        public long SetLength(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SetLength", key))
            {
                return _db.SetLength(key, flags);
            }
        }

        public RedisValue[] SetMembers(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SetMembers", key))
            {
                return _db.SetMembers(key, flags);
            }
        }

        public bool SetMove(RedisKey source, RedisKey destination, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SetMove"))
            {
                return _db.SetMove(source, destination, value, flags);
            }
        }

        public RedisValue SetPop(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SetPop", key))
            {
                return _db.SetPop(key, flags);
            }
        }

        public RedisValue[] SetPop(RedisKey key, long count, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SetPop", key))
            {
                return _db.SetPop(key, count, flags);
            }
        }

        public RedisValue SetRandomMember(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SetRandomMember", key))
            {
                return _db.SetRandomMember(key, flags);
            }
        }

        public RedisValue[] SetRandomMembers(RedisKey key, long count, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SetRandomMembers", key))
            {
                return _db.SetRandomMembers(key, count, flags);
            }
        }

        public bool SetRemove(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SetRemove", key))
            {
                return _db.SetRemove(key, value, flags);
            }
        }

        public long SetRemove(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SetRemove", key))
            {
                return _db.SetRemove(key, values, flags);
            }
        }

        public IEnumerable<RedisValue> SetScan(RedisKey key, RedisValue pattern, int pageSize, CommandFlags flags)
        {
            using (FsLinkTrack.TrackRedis("SetScan", key))
            {
                return _db.SetScan(key, pattern, pageSize, flags);
            }
        }

        public IEnumerable<RedisValue> SetScan(RedisKey key, RedisValue pattern = new RedisValue(), int pageSize = 250, long cursor = 0, int pageOffset = 0, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SetScan", key))
            {
                return _db.SetScan(key, pattern, pageSize, cursor, pageOffset, flags);
            }
        }

        public RedisValue[] Sort(RedisKey key, long skip = 0, long take = -1, Order order = Order.Ascending, SortType sortType = SortType.Numeric, RedisValue @by = new RedisValue(), RedisValue[] get = null, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("Sort", key))
            {
                return _db.Sort(key, skip, take, order, sortType, by, get, flags);
            }
        }

        public long SortAndStore(RedisKey destination, RedisKey key, long skip = 0, long take = -1, Order order = Order.Ascending, SortType sortType = SortType.Numeric, RedisValue @by = new RedisValue(), RedisValue[] get = null, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SortAndStore", key))
            {
                return _db.SortAndStore(destination, key, skip, take, order, sortType, by, get, flags);
            }
        }

        public bool SortedSetAdd(RedisKey key, RedisValue member, double score, CommandFlags flags)
        {
            using (FsLinkTrack.TrackRedis("SortedSetAdd", key))
            {
                return _db.SortedSetAdd(key, member, score, flags);
            }
        }

        public bool SortedSetAdd(RedisKey key, RedisValue member, double score, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SortedSetAdd", key))
            {
                return _db.SortedSetAdd(key, member, score, when, flags);
            }
        }

        public long SortedSetAdd(RedisKey key, SortedSetEntry[] values, CommandFlags flags)
        {
            using (FsLinkTrack.TrackRedis("SortedSetAdd", key))
            {
                return _db.SortedSetAdd(key, values, flags);
            }
        }

        public long SortedSetAdd(RedisKey key, SortedSetEntry[] values, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SortedSetAdd", key))
            {
                return _db.SortedSetAdd(key, values, when, flags);
            }
        }

        public long SortedSetCombineAndStore(SetOperation operation, RedisKey destination, RedisKey first, RedisKey second, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SortedSetCombineAndStore"))
            {
                return _db.SortedSetCombineAndStore(operation, destination, first, second, aggregate, flags);
            }
        }

        public long SortedSetCombineAndStore(SetOperation operation, RedisKey destination, RedisKey[] keys, double[] weights = null, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SortedSetCombineAndStore"))
            {
                return _db.SortedSetCombineAndStore(operation, destination, keys, weights, aggregate, flags);
            }
        }

        public double SortedSetDecrement(RedisKey key, RedisValue member, double value, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SortedSetDecrement", key))
            {
                return _db.SortedSetDecrement(key, member, value, flags);
            }
        }

        public double SortedSetIncrement(RedisKey key, RedisValue member, double value, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SortedSetIncrement", key))
            {
                return _db.SortedSetIncrement(key, member, value, flags);
            }
        }

        public long SortedSetLength(RedisKey key, double min = double.NegativeInfinity, double max = double.PositiveInfinity, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SortedSetLength", key))
            {
                return _db.SortedSetLength(key, min, max, exclude, flags);
            }
        }

        public long SortedSetLengthByValue(RedisKey key, RedisValue min, RedisValue max, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SortedSetLengthByValue", key))
            {
                return _db.SortedSetLengthByValue(key, min, max, exclude, flags);
            }
        }

        public RedisValue[] SortedSetRangeByRank(RedisKey key, long start = 0, long stop = -1, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SortedSetRangeByRank", key))
            {
                return _db.SortedSetRangeByRank(key, start, stop, order, flags);
            }
        }

        public SortedSetEntry[] SortedSetRangeByRankWithScores(RedisKey key, long start = 0, long stop = -1, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SortedSetRangeByRankWithScores", key))
            {
                return _db.SortedSetRangeByRankWithScores(key, start, stop, order, flags);
            }
        }

        public RedisValue[] SortedSetRangeByScore(RedisKey key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SortedSetRangeByScore", key))
            {
                return _db.SortedSetRangeByScore(key, start, stop, exclude, order, skip, take, flags);
            }
        }

        public SortedSetEntry[] SortedSetRangeByScoreWithScores(RedisKey key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SortedSetRangeByScoreWithScores", key))
            {
                return _db.SortedSetRangeByScoreWithScores(key, start, stop, exclude, order, skip, take, flags);
            }
        }

        public RedisValue[] SortedSetRangeByValue(RedisKey key, RedisValue min, RedisValue max, Exclude exclude, long skip, long take = -1, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SortedSetRangeByValue", key))
            {
                return _db.SortedSetRangeByValue(key, min, max, exclude, skip, take, flags);
            }
        }

        public RedisValue[] SortedSetRangeByValue(RedisKey key, RedisValue min = new RedisValue(), RedisValue max = new RedisValue(), Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SortedSetRangeByValue", key))
            {
                return _db.SortedSetRangeByValue(key, min, max, exclude, order, skip, take, flags);
            }
        }

        public long? SortedSetRank(RedisKey key, RedisValue member, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SortedSetRank", key))
            {
                return _db.SortedSetRank(key, member, order, flags);
            }
        }

        public bool SortedSetRemove(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SortedSetRemove", key))
            {
                return _db.SortedSetRemove(key, member, flags);
            }
        }

        public long SortedSetRemove(RedisKey key, RedisValue[] members, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SortedSetRemove", key))
            {
                return _db.SortedSetRemove(key, members, flags);
            }
        }

        public long SortedSetRemoveRangeByRank(RedisKey key, long start, long stop, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SortedSetRemoveRangeByRank", key))
            {
                return _db.SortedSetRemoveRangeByRank(key, start, stop, flags);
            }
        }

        public long SortedSetRemoveRangeByScore(RedisKey key, double start, double stop, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SortedSetRemoveRangeByScore", key))
            {
                return _db.SortedSetRemoveRangeByScore(key, start, stop, exclude, flags);
            }
        }

        public long SortedSetRemoveRangeByValue(RedisKey key, RedisValue min, RedisValue max, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SortedSetRemoveRangeByValue", key))
            {
                return _db.SortedSetRemoveRangeByValue(key, min, max, exclude, flags);
            }
        }

        public IEnumerable<SortedSetEntry> SortedSetScan(RedisKey key, RedisValue pattern, int pageSize, CommandFlags flags)
        {
            using (FsLinkTrack.TrackRedis("SortedSetScan", key))
            {
                return _db.SortedSetScan(key, pattern, pageSize, flags);
            }
        }

        public IEnumerable<SortedSetEntry> SortedSetScan(RedisKey key, RedisValue pattern = new RedisValue(), int pageSize = 250, long cursor = 0, int pageOffset = 0, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SortedSetScan", key))
            {
                return _db.SortedSetScan(key, pattern, pageSize, cursor, pageOffset, flags);
            }
        }

        public double? SortedSetScore(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SortedSetScore", key))
            {
                return _db.SortedSetScore(key, member, flags);
            }
        }

        public SortedSetEntry? SortedSetPop(RedisKey key, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SortedSetPop", key))
            {
                return _db.SortedSetPop(key, order, flags);
            }
        }

        public SortedSetEntry[] SortedSetPop(RedisKey key, long count, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("SortedSetPop", key))
            {
                return _db.SortedSetPop(key, count, order, flags);
            }
        }

        public long StreamAcknowledge(RedisKey key, RedisValue groupName, RedisValue messageId, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StreamAcknowledge", key))
            {
                return _db.StreamAcknowledge(key, groupName, messageId, flags);
            }
        }

        public long StreamAcknowledge(RedisKey key, RedisValue groupName, RedisValue[] messageIds, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StreamAcknowledge", key))
            {
                return _db.StreamAcknowledge(key, groupName, messageIds, flags);
            }
        }

        public RedisValue StreamAdd(RedisKey key, RedisValue streamField, RedisValue streamValue, RedisValue? messageId = null, int? maxLength = null, bool useApproximateMaxLength = false, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StreamAdd", key))
            {
                return _db.StreamAdd(key, streamField, streamValue, messageId, maxLength, useApproximateMaxLength, flags);
            }
        }

        public RedisValue StreamAdd(RedisKey key, NameValueEntry[] streamPairs, RedisValue? messageId = null, int? maxLength = null, bool useApproximateMaxLength = false, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StreamAdd", key))
            {
                return _db.StreamAdd(key, streamPairs, messageId, maxLength, useApproximateMaxLength, flags);
            }
        }

        public StreamEntry[] StreamClaim(RedisKey key, RedisValue consumerGroup, RedisValue claimingConsumer, long minIdleTimeInMs, RedisValue[] messageIds, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StreamClaim", key))
            {
                return _db.StreamClaim(key, consumerGroup, claimingConsumer, minIdleTimeInMs, messageIds, flags);
            }
        }

        public RedisValue[] StreamClaimIdsOnly(RedisKey key, RedisValue consumerGroup, RedisValue claimingConsumer, long minIdleTimeInMs, RedisValue[] messageIds, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StreamClaimIdsOnly", key))
            {
                return _db.StreamClaimIdsOnly(key, consumerGroup, claimingConsumer, minIdleTimeInMs, messageIds, flags);
            }
        }

        public bool StreamConsumerGroupSetPosition(RedisKey key, RedisValue groupName, RedisValue position, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StreamConsumerGroupSetPosition", key))
            {
                return _db.StreamConsumerGroupSetPosition(key, groupName, position, flags);
            }
        }

        public StreamConsumerInfo[] StreamConsumerInfo(RedisKey key, RedisValue groupName, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StreamConsumerInfo", key))
            {
                return _db.StreamConsumerInfo(key, groupName, flags);
            }
        }

        public bool StreamCreateConsumerGroup(RedisKey key, RedisValue groupName, RedisValue? position, CommandFlags flags)
        {
            using (FsLinkTrack.TrackRedis("StreamCreateConsumerGroup", key))
            {
                return _db.StreamCreateConsumerGroup(key, groupName, position, flags);
            }
        }

        public bool StreamCreateConsumerGroup(RedisKey key, RedisValue groupName, RedisValue? position = null, bool createStream = true, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StreamCreateConsumerGroup", key))
            {
                return _db.StreamCreateConsumerGroup(key, groupName, position, createStream, flags);
            }
        }

        public long StreamDelete(RedisKey key, RedisValue[] messageIds, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StreamDelete", key))
            {
                return _db.StreamDelete(key, messageIds, flags);
            }
        }

        public long StreamDeleteConsumer(RedisKey key, RedisValue groupName, RedisValue consumerName, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StreamDeleteConsumer", key))
            {
                return _db.StreamDeleteConsumer(key, groupName, consumerName, flags);
            }
        }

        public bool StreamDeleteConsumerGroup(RedisKey key, RedisValue groupName, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StreamDeleteConsumerGroup", key))
            {
                return _db.StreamDeleteConsumerGroup(key, groupName, flags);
            }
        }

        public StreamGroupInfo[] StreamGroupInfo(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StreamGroupInfo", key))
            {
                return _db.StreamGroupInfo(key, flags);
            }
        }

        public StreamInfo StreamInfo(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StreamInfo", key))
            {
                return _db.StreamInfo(key, flags);
            }
        }

        public long StreamLength(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StreamLength", key))
            {
                return _db.StreamLength(key, flags);
            }
        }

        public StreamPendingInfo StreamPending(RedisKey key, RedisValue groupName, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StreamPending", key))
            {
                return _db.StreamPending(key, groupName, flags);
            }
        }

        public StreamPendingMessageInfo[] StreamPendingMessages(RedisKey key, RedisValue groupName, int count, RedisValue consumerName, RedisValue? minId = null, RedisValue? maxId = null, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StreamPendingMessages", key))
            {
                return _db.StreamPendingMessages(key, groupName, count, consumerName, minId, maxId, flags);
            }
        }

        public StreamEntry[] StreamRange(RedisKey key, RedisValue? minId = null, RedisValue? maxId = null, int? count = null, Order messageOrder = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StreamRange", key))
            {
                return _db.StreamRange(key, minId, maxId, count, messageOrder, flags);
            }
        }

        public StreamEntry[] StreamRead(RedisKey key, RedisValue position, int? count = null, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StreamRead", key))
            {
                return _db.StreamRead(key, position, count, flags);
            }
        }

        public RedisStream[] StreamRead(StreamPosition[] streamPositions, int? countPerStream = null, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StreamRead"))
            {
                return _db.StreamRead(streamPositions, countPerStream, flags);
            }
        }

        public StreamEntry[] StreamReadGroup(RedisKey key, RedisValue groupName, RedisValue consumerName, RedisValue? position, int? count, CommandFlags flags)
        {
            using (FsLinkTrack.TrackRedis("StreamReadGroup", key))
            {
                return _db.StreamReadGroup(key, groupName, consumerName, position, count, flags);
            }
        }

        public StreamEntry[] StreamReadGroup(RedisKey key, RedisValue groupName, RedisValue consumerName, RedisValue? position = null, int? count = null, bool noAck = false, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StreamReadGroup", key))
            {
                return _db.StreamReadGroup(key, groupName, consumerName, position, count, noAck, flags);
            }
        }

        public RedisStream[] StreamReadGroup(StreamPosition[] streamPositions, RedisValue groupName, RedisValue consumerName, int? countPerStream, CommandFlags flags)
        {
            using (FsLinkTrack.TrackRedis("StreamReadGroup"))
            {
                return _db.StreamReadGroup(streamPositions, groupName, consumerName, countPerStream, flags);
            }
        }

        public RedisStream[] StreamReadGroup(StreamPosition[] streamPositions, RedisValue groupName, RedisValue consumerName, int? countPerStream = null, bool noAck = false, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StreamReadGroup"))
            {
                return _db.StreamReadGroup(streamPositions, groupName, consumerName, countPerStream, noAck, flags);
            }
        }

        public long StreamTrim(RedisKey key, int maxLength, bool useApproximateMaxLength = false, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StreamTrim", key))
            {
                return _db.StreamTrim(key, maxLength, useApproximateMaxLength, flags);
            }
        }

        public long StringAppend(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StringAppend", key))
            {
                return _db.StringAppend(key, value, flags);
            }
        }

        public long StringBitCount(RedisKey key, long start = 0, long end = -1, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StringBitCount", key))
            {
                return _db.StringBitCount(key, start, end, flags);
            }
        }

        public long StringBitOperation(Bitwise operation, RedisKey destination, RedisKey first, RedisKey second = new RedisKey(), CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StringBitOperation"))
            {
                return _db.StringBitOperation(operation, destination, first, second, flags);
            }
        }

        public long StringBitOperation(Bitwise operation, RedisKey destination, RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StringBitOperation"))
            {
                return _db.StringBitOperation(operation, destination, keys, flags);
            }
        }

        public long StringBitPosition(RedisKey key, bool bit, long start = 0, long end = -1, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StringBitPosition", key))
            {
                return _db.StringBitPosition(key, bit, start, end, flags);
            }
        }

        public long StringDecrement(RedisKey key, long value = 1, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StringDecrement", key))
            {
                return _db.StringDecrement(key, value, flags);
            }
        }

        public double StringDecrement(RedisKey key, double value, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StringDecrement", key))
            {
                return _db.StringDecrement(key, value, flags);
            }
        }

        public RedisValue StringGet(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StringGet", key))
            {
                return _db.StringGet(key, flags);
            }
        }

        public RedisValue[] StringGet(RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StringGet"))
            {
                return _db.StringGet(keys, flags);
            }
        }

        public Lease<byte> StringGetLease(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StringGetLease", key))
            {
                return _db.StringGetLease(key, flags);
            }
        }

        public bool StringGetBit(RedisKey key, long offset, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StringGetBit", key))
            {
                return _db.StringGetBit(key, offset, flags);
            }
        }

        public RedisValue StringGetRange(RedisKey key, long start, long end, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StringGetRange", key))
            {
                return _db.StringGetRange(key, start, end, flags);
            }
        }

        public RedisValue StringGetSet(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StringGetSet", key))
            {
                return _db.StringGetSet(key, value, flags);
            }
        }

        public RedisValueWithExpiry StringGetWithExpiry(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StringGetWithExpiry", key))
            {
                return _db.StringGetWithExpiry(key, flags);
            }
        }

        public long StringIncrement(RedisKey key, long value = 1, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StringIncrement", key))
            {
                return _db.StringIncrement(key, value, flags);
            }
        }

        public double StringIncrement(RedisKey key, double value, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StringIncrement", key))
            {
                return _db.StringIncrement(key, value, flags);
            }
        }

        public long StringLength(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StringLength", key))
            {
                return _db.StringLength(key, flags);
            }
        }

        public bool StringSet(RedisKey key, RedisValue value, TimeSpan? expiry = null, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StringSet", key))
            {
                return _db.StringSet(key, value, expiry, when, flags);
            }
        }

        public bool StringSet(KeyValuePair<RedisKey, RedisValue>[] values, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StringSet"))
            {
                return _db.StringSet(values, when, flags);
            }
        }

        public bool StringSetBit(RedisKey key, long offset, bool bit, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StringSetBit", key))
            {
                return _db.StringSetBit(key, offset, bit, flags);
            }
        }

        public RedisValue StringSetRange(RedisKey key, long offset, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("StringSetRange", key))
            {
                return _db.StringSetRange(key, offset, value, flags);
            }
        }

        public bool KeyTouch(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("KeyTouch", key))
            {
                return _db.KeyTouch(key, flags);
            }
        }

        public long KeyTouch(RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis("KeyTouch"))
            {
                return _db.KeyTouch(keys, flags);
            }
        }
    }
}