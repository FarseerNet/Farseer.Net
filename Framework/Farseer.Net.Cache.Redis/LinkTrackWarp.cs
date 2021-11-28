using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FS.Core.LinkTrack;
using StackExchange.Redis;

namespace FS.Cache.Redis
{
    public class LinkTrackWarp : IDatabase
    {
        private readonly IDatabase _db;

        public LinkTrackWarp(IDatabase db)
        {
            _db = db;
        }

        public int            Database                                          => _db.Database;
        public Task<TimeSpan> PingAsync(CommandFlags flags = CommandFlags.None) => _db.PingAsync(flags: flags);

        public bool                   TryWait(Task          task)                  => _db.TryWait(task: task);
        public void                   Wait(Task             task)                  => _db.Wait(task: task);
        public T                      Wait<T>(Task<T>       task)                  => _db.Wait(task: task);
        public void                   WaitAll(params Task[] tasks)                 => _db.WaitAll(tasks: tasks);
        public IConnectionMultiplexer Multiplexer                                  => _db.Multiplexer;
        public TimeSpan               Ping(CommandFlags flags = CommandFlags.None) => _db.Ping(flags: flags);

        public bool IsConnected(RedisKey key, CommandFlags flags = CommandFlags.None) => _db.IsConnected(key: key, flags: flags);

        public Task KeyMigrateAsync(RedisKey key, EndPoint toServer, int toDatabase = 0, int timeoutMilliseconds = 0, MigrateOptions migrateOptions = MigrateOptions.None, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "KeyMigrateAsync", key: key))
            {
                return _db.KeyMigrateAsync(key: key, toServer: toServer, toDatabase: toDatabase, timeoutMilliseconds: timeoutMilliseconds, migrateOptions: migrateOptions, flags: flags);
            }
        }

        public Task<RedisValue> DebugObjectAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "DebugObjectAsync", key: key))
            {
                return _db.DebugObjectAsync(key: key, flags: flags);
            }
        }

        public Task<bool> GeoAddAsync(RedisKey key, double longitude, double latitude, RedisValue member, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "GeoAddAsync", key: key))
            {
                return _db.GeoAddAsync(key: key, longitude: longitude, latitude: latitude, member: member, flags: flags);
            }
        }

        public Task<bool> GeoAddAsync(RedisKey key, GeoEntry value, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "GeoAddAsync", key: key))
            {
                return _db.GeoAddAsync(key: key, value: value, flags: flags);
            }
        }

        public Task<long> GeoAddAsync(RedisKey key, GeoEntry[] values, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "GeoAddAsync", key: key))
            {
                return _db.GeoAddAsync(key: key, values: values, flags: flags);
            }
        }

        public Task<bool> GeoRemoveAsync(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "GeoRemoveAsync", key: key))
            {
                return _db.GeoRemoveAsync(key: key, member: member, flags: flags);
            }
        }

        public Task<double?> GeoDistanceAsync(RedisKey key, RedisValue member1, RedisValue member2, GeoUnit unit = GeoUnit.Meters, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "GeoDistanceAsync", key: key))
            {
                return _db.GeoDistanceAsync(key: key, member1: member1, member2: member2, unit: unit, flags: flags);
            }
        }

        public Task<string[]> GeoHashAsync(RedisKey key, RedisValue[] members, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "GeoHashAsync", key: key))
            {
                return _db.GeoHashAsync(key: key, members: members, flags: flags);
            }
        }

        public Task<string> GeoHashAsync(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "GeoHashAsync", key: key))
            {
                return _db.GeoHashAsync(key: key, member: member, flags: flags);
            }
        }

        public Task<GeoPosition?[]> GeoPositionAsync(RedisKey key, RedisValue[] members, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "GeoPositionAsync", key: key))
            {
                return _db.GeoPositionAsync(key: key, members: members, flags: flags);
            }
        }

        public Task<GeoPosition?> GeoPositionAsync(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "GeoPositionAsync", key: key))
            {
                return _db.GeoPositionAsync(key: key, member: member, flags: flags);
            }
        }

        public Task<GeoRadiusResult[]> GeoRadiusAsync(RedisKey key, RedisValue member, double radius, GeoUnit unit = GeoUnit.Meters, int count = -1, Order? order = null, GeoRadiusOptions options = GeoRadiusOptions.Default, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "GeoRadiusAsync", key: key))
            {
                return _db.GeoRadiusAsync(key: key, member: member, radius: radius, unit: unit, count: count, order: order, options: options, flags: flags);
            }
        }

        public Task<GeoRadiusResult[]> GeoRadiusAsync(RedisKey key, double longitude, double latitude, double radius, GeoUnit unit = GeoUnit.Meters, int count = -1, Order? order = null, GeoRadiusOptions options = GeoRadiusOptions.Default, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "GeoRadiusAsync", key: key))
            {
                return _db.GeoRadiusAsync(key: key, longitude: longitude, latitude: latitude, radius: radius, unit: unit, count: count, order: order, options: options, flags: flags);
            }
        }

        public Task<long> HashDecrementAsync(RedisKey key, RedisValue hashField, long value = 1, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "HashDecrementAsync", key: key, member: hashField))
            {
                return _db.HashDecrementAsync(key: key, hashField: hashField, value: value, flags: flags);
            }
        }

        public Task<double> HashDecrementAsync(RedisKey key, RedisValue hashField, double value, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "HashDecrementAsync", key: key, member: hashField))
            {
                return _db.HashDecrementAsync(key: key, hashField: hashField, value: value, flags: flags);
            }
        }

        public Task<bool> HashDeleteAsync(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "HashDeleteAsync", key: key, member: hashField))
            {
                return _db.HashDeleteAsync(key: key, hashField: hashField, flags: flags);
            }
        }

        public Task<long> HashDeleteAsync(RedisKey key, RedisValue[] hashFields, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "HashDeleteAsync", key: key, member: string.Join(separator: ",", values: hashFields)))
            {
                return _db.HashDeleteAsync(key: key, hashFields: hashFields, flags: flags);
            }
        }

        public Task<bool> HashExistsAsync(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "HashExistsAsync", key: key, member: hashField))
            {
                return _db.HashExistsAsync(key: key, hashField: hashField, flags: flags);
            }
        }

        public async Task<RedisValue> HashGetAsync(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "HashGetAsync", key: key, member: hashField))
            {
                return await _db.HashGetAsync(key: key, hashField: hashField, flags: flags);
            }
        }

        public Task<Lease<byte>> HashGetLeaseAsync(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "HashGetLeaseAsync", key: key, member: hashField))
            {
                return _db.HashGetLeaseAsync(key: key, hashField: hashField, flags: flags);
            }
        }

        public Task<RedisValue[]> HashGetAsync(RedisKey key, RedisValue[] hashFields, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "HashGetAsync", key: key, member: string.Join(separator: ",", values: hashFields)))
            {
                return _db.HashGetAsync(key: key, hashFields: hashFields, flags: flags);
            }
        }

        public Task<HashEntry[]> HashGetAllAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "HashGetAllAsync", key: key))
            {
                return _db.HashGetAllAsync(key: key, flags: flags);
            }
        }

        public Task<long> HashIncrementAsync(RedisKey key, RedisValue hashField, long value = 1, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "HashIncrementAsync", key: key, member: hashField))
            {
                return _db.HashIncrementAsync(key: key, hashField: hashField, value: value, flags: flags);
            }
        }

        public Task<double> HashIncrementAsync(RedisKey key, RedisValue hashField, double value, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "HashIncrementAsync", key: key, member: hashField))
            {
                return _db.HashIncrementAsync(key: key, hashField: hashField, value: value, flags: flags);
            }
        }

        public Task<RedisValue[]> HashKeysAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "HashKeysAsync", key: key))
            {
                return _db.HashKeysAsync(key: key, flags: flags);
            }
        }

        public Task<long> HashLengthAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "HashLengthAsync", key: key))
            {
                return _db.HashLengthAsync(key: key, flags: flags);
            }
        }

        public IAsyncEnumerable<HashEntry> HashScanAsync(RedisKey key, RedisValue pattern = new(), int pageSize = 250, long cursor = 0, int pageOffset = 0, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "HashScanAsync", key: key))
            {
                return _db.HashScanAsync(key: key, pattern: pattern, pageSize: pageSize, cursor: cursor, pageOffset: pageOffset, flags: flags);
            }
        }

        public Task HashSetAsync(RedisKey key, HashEntry[] hashFields, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "HashSetAsync", key: key, member: string.Join(separator: ",", values: hashFields)))
            {
                return _db.HashSetAsync(key: key, hashFields: hashFields, flags: flags);
            }
        }

        public Task<bool> HashSetAsync(RedisKey key, RedisValue hashField, RedisValue value, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "HashSetAsync", key: key, member: hashField))
            {
                return _db.HashSetAsync(key: key, hashField: hashField, value: value, when: when, flags: flags);
            }
        }

        public Task<long> HashStringLengthAsync(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "HashStringLengthAsync", key: key, member: hashField))
            {
                return _db.HashStringLengthAsync(key: key, hashField: hashField, flags: flags);
            }
        }

        public Task<RedisValue[]> HashValuesAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "HashValuesAsync", key: key))
            {
                return _db.HashValuesAsync(key: key, flags: flags);
            }
        }

        public Task<bool> HyperLogLogAddAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "HyperLogLogAddAsync", key: key))
            {
                return _db.HyperLogLogAddAsync(key: key, value: value, flags: flags);
            }
        }

        public Task<bool> HyperLogLogAddAsync(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "HyperLogLogAddAsync", key: key))
            {
                return _db.HyperLogLogAddAsync(key: key, values: values, flags: flags);
            }
        }

        public Task<long> HyperLogLogLengthAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "HyperLogLogLengthAsync", key: key))
            {
                return _db.HyperLogLogLengthAsync(key: key, flags: flags);
            }
        }

        public Task<long> HyperLogLogLengthAsync(RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "HyperLogLogLengthAsync", key: string.Join(separator: ",", values: keys)))
            {
                return _db.HyperLogLogLengthAsync(keys: keys, flags: flags);
            }
        }

        public Task HyperLogLogMergeAsync(RedisKey destination, RedisKey first, RedisKey second, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "HyperLogLogMergeAsync"))
            {
                return _db.HyperLogLogMergeAsync(destination: destination, first: first, second: second, flags: flags);
            }
        }

        public Task HyperLogLogMergeAsync(RedisKey destination, RedisKey[] sourceKeys, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "HyperLogLogMergeAsync"))
            {
                return _db.HyperLogLogMergeAsync(destination: destination, sourceKeys: sourceKeys, flags: flags);
            }
        }

        public Task<EndPoint> IdentifyEndpointAsync(RedisKey key = new(), CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "IdentifyEndpointAsync", key: key))
            {
                return _db.IdentifyEndpointAsync(key: key, flags: flags);
            }
        }

        public Task<bool> KeyDeleteAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "KeyDeleteAsync", key: key))
            {
                return _db.KeyDeleteAsync(key: key, flags: flags);
            }
        }

        public Task<long> KeyDeleteAsync(RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "KeyDeleteAsync", key: string.Join(separator: ",", values: keys)))
            {
                return _db.KeyDeleteAsync(keys: keys, flags: flags);
            }
        }

        public Task<byte[]> KeyDumpAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "KeyDumpAsync", key: key))
            {
                return _db.KeyDumpAsync(key: key, flags: flags);
            }
        }

        public Task<bool> KeyExistsAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "KeyExistsAsync", key: key))
            {
                return _db.KeyExistsAsync(key: key, flags: flags);
            }
        }

        public Task<long> KeyExistsAsync(RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "KeyExistsAsync", key: string.Join(separator: ",", values: keys)))
            {
                return _db.KeyExistsAsync(keys: keys, flags: flags);
            }
        }

        public Task<bool> KeyExpireAsync(RedisKey key, TimeSpan? expiry, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "KeyExpireAsync", key: key))
            {
                return _db.KeyExpireAsync(key: key, expiry: expiry, flags: flags);
            }
        }

        public Task<bool> KeyExpireAsync(RedisKey key, DateTime? expiry, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "KeyExpireAsync", key: key))
            {
                return _db.KeyExpireAsync(key: key, expiry: expiry, flags: flags);
            }
        }

        public Task<TimeSpan?> KeyIdleTimeAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "KeyIdleTimeAsync", key: key))
            {
                return _db.KeyIdleTimeAsync(key: key, flags: flags);
            }
        }

        public Task<bool> KeyMoveAsync(RedisKey key, int database, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "KeyMoveAsync", key: key))
            {
                return _db.KeyMoveAsync(key: key, database: database, flags: flags);
            }
        }

        public Task<bool> KeyPersistAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "KeyPersistAsync", key: key))
            {
                return _db.KeyPersistAsync(key: key, flags: flags);
            }
        }

        public Task<RedisKey> KeyRandomAsync(CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "KeyRandomAsync"))
            {
                return _db.KeyRandomAsync(flags: flags);
            }
        }

        public Task<bool> KeyRenameAsync(RedisKey key, RedisKey newKey, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "KeyRenameAsync", key: key))
            {
                return _db.KeyRenameAsync(key: key, newKey: newKey, when: when, flags: flags);
            }
        }

        public Task KeyRestoreAsync(RedisKey key, byte[] value, TimeSpan? expiry = null, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "KeyRestoreAsync", key: key))
            {
                return _db.KeyRestoreAsync(key: key, value: value, expiry: expiry, flags: flags);
            }
        }

        public Task<TimeSpan?> KeyTimeToLiveAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "KeyTimeToLiveAsync", key: key))
            {
                return _db.KeyTimeToLiveAsync(key: key, flags: flags);
            }
        }

        public Task<RedisType> KeyTypeAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "KeyTypeAsync", key: key))
            {
                return _db.KeyTypeAsync(key: key, flags: flags);
            }
        }

        public Task<RedisValue> ListGetByIndexAsync(RedisKey key, long index, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "ListGetByIndexAsync", key: key))
            {
                return _db.ListGetByIndexAsync(key: key, index: index, flags: flags);
            }
        }

        public Task<long> ListInsertAfterAsync(RedisKey key, RedisValue pivot, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "ListInsertAfterAsync", key: key))
            {
                return _db.ListInsertAfterAsync(key: key, pivot: pivot, value: value, flags: flags);
            }
        }

        public Task<long> ListInsertBeforeAsync(RedisKey key, RedisValue pivot, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "ListInsertBeforeAsync", key: key))
            {
                return _db.ListInsertBeforeAsync(key: key, pivot: pivot, value: value, flags: flags);
            }
        }

        public Task<RedisValue> ListLeftPopAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "ListLeftPopAsync", key: key))
            {
                return _db.ListLeftPopAsync(key: key, flags: flags);
            }
        }
        public Task<RedisValue[]> ListLeftPopAsync(RedisKey key, long count, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "ListLeftPopAsync", key: key))
            {
                return _db.ListLeftPopAsync(key: key, count, flags: flags);
            }
        }

        public Task<long> ListLeftPushAsync(RedisKey key, RedisValue value, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "ListLeftPushAsync", key: key))
            {
                return _db.ListLeftPushAsync(key: key, value: value, when: when, flags: flags);
            }
        }

        public Task<long> ListLeftPushAsync(RedisKey key, RedisValue[] values, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "ListLeftPushAsync", key: key))
            {
                return _db.ListLeftPushAsync(key: key, values: values, when: when, flags: flags);
            }
        }

        public Task<long> ListLeftPushAsync(RedisKey key, RedisValue[] values, CommandFlags flags)
        {
            using (FsLinkTrack.TrackRedis(method: "ListLeftPushAsync", key: key))
            {
                return _db.ListLeftPushAsync(key: key, values: values, flags: flags);
            }
        }

        public Task<long> ListLengthAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "ListLengthAsync", key: key))
            {
                return _db.ListLengthAsync(key: key, flags: flags);
            }
        }

        public Task<RedisValue[]> ListRangeAsync(RedisKey key, long start = 0, long stop = -1, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "ListRangeAsync", key: key))
            {
                return _db.ListRangeAsync(key: key, start: start, stop: stop, flags: flags);
            }
        }

        public Task<long> ListRemoveAsync(RedisKey key, RedisValue value, long count = 0, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "ListRemoveAsync", key: key))
            {
                return _db.ListRemoveAsync(key: key, value: value, count: count, flags: flags);
            }
        }

        public Task<RedisValue> ListRightPopAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "ListRightPopAsync", key: key))
            {
                return _db.ListRightPopAsync(key: key, flags: flags);
            }
        }
        public Task<RedisValue[]> ListRightPopAsync(RedisKey key, long count, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "ListRightPopAsync", key: key))
            {
                return _db.ListRightPopAsync(key: key, count, flags: flags);
            }
        }

        public Task<RedisValue> ListRightPopLeftPushAsync(RedisKey source, RedisKey destination, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "ListRightPopLeftPushAsync"))
            {
                return _db.ListRightPopLeftPushAsync(source: source, destination: destination, flags: flags);
            }
        }

        public Task<long> ListRightPushAsync(RedisKey key, RedisValue value, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "ListRightPushAsync", key: key))
            {
                return _db.ListRightPushAsync(key: key, value: value, when: when, flags: flags);
            }
        }

        public Task<long> ListRightPushAsync(RedisKey key, RedisValue[] values, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "ListRightPushAsync", key: key))
            {
                return _db.ListRightPushAsync(key: key, values: values, when: when, flags: flags);
            }
        }

        public Task<long> ListRightPushAsync(RedisKey key, RedisValue[] values, CommandFlags flags)
        {
            using (FsLinkTrack.TrackRedis(method: "ListRightPushAsync", key: key))
            {
                return _db.ListRightPushAsync(key: key, values: values, flags: flags);
            }
        }

        public Task ListSetByIndexAsync(RedisKey key, long index, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "ListSetByIndexAsync", key: key))
            {
                return _db.ListSetByIndexAsync(key: key, index: index, value: value, flags: flags);
            }
        }

        public Task ListTrimAsync(RedisKey key, long start, long stop, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "ListTrimAsync", key: key))
            {
                return _db.ListTrimAsync(key: key, start: start, stop: stop, flags: flags);
            }
        }

        public Task<bool> LockExtendAsync(RedisKey key, RedisValue value, TimeSpan expiry, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "LockExtendAsync", key: key))
            {
                return _db.LockExtendAsync(key: key, value: value, expiry: expiry, flags: flags);
            }
        }

        public Task<RedisValue> LockQueryAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "LockQueryAsync", key: key))
            {
                return _db.LockQueryAsync(key: key, flags: flags);
            }
        }

        public Task<bool> LockReleaseAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "LockReleaseAsync", key: key))
            {
                return _db.LockReleaseAsync(key: key, value: value, flags: flags);
            }
        }

        public Task<bool> LockTakeAsync(RedisKey key, RedisValue value, TimeSpan expiry, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "LockTakeAsync", key: key))
            {
                return _db.LockTakeAsync(key: key, value: value, expiry: expiry, flags: flags);
            }
        }

        public Task<long> PublishAsync(RedisChannel channel, RedisValue message, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "PublishAsync"))
            {
                return _db.PublishAsync(channel: channel, message: message, flags: flags);
            }
        }

        public Task<RedisResult> ExecuteAsync(string command, params object[] args)
        {
            using (FsLinkTrack.TrackRedis(method: "ExecuteAsync"))
            {
                return _db.ExecuteAsync(command: command, args: args);
            }
        }

        public Task<RedisResult> ExecuteAsync(string command, ICollection<object> args, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "ExecuteAsync"))
            {
                return _db.ExecuteAsync(command: command, args: args, flags: flags);
            }
        }

        public Task<RedisResult> ScriptEvaluateAsync(string script, RedisKey[] keys = null, RedisValue[] values = null, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "ScriptEvaluateAsync", key: string.Join(separator: ",", values: keys)))
            {
                return _db.ScriptEvaluateAsync(script: script, keys: keys, values: values, flags: flags);
            }
        }

        public Task<RedisResult> ScriptEvaluateAsync(byte[] hash, RedisKey[] keys = null, RedisValue[] values = null, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "ScriptEvaluateAsync", key: string.Join(separator: ",", values: keys)))
            {
                return _db.ScriptEvaluateAsync(hash: hash, keys: keys, values: values, flags: flags);
            }
        }

        public Task<RedisResult> ScriptEvaluateAsync(LuaScript script, object parameters = null, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "ScriptEvaluateAsync"))
            {
                return _db.ScriptEvaluateAsync(script: script, parameters: parameters, flags: flags);
            }
        }

        public Task<RedisResult> ScriptEvaluateAsync(LoadedLuaScript script, object parameters = null, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "ScriptEvaluateAsync"))
            {
                return _db.ScriptEvaluateAsync(script: script, parameters: parameters, flags: flags);
            }
        }

        public Task<bool> SetAddAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SetAddAsync", key: key))
            {
                return _db.SetAddAsync(key: key, value: value, flags: flags);
            }
        }

        public Task<long> SetAddAsync(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SetAddAsync", key: key))
            {
                return _db.SetAddAsync(key: key, values: values, flags: flags);
            }
        }

        public Task<RedisValue[]> SetCombineAsync(SetOperation operation, RedisKey first, RedisKey second, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SetCombineAsync"))
            {
                return _db.SetCombineAsync(operation: operation, first: first, second: second, flags: flags);
            }
        }

        public Task<RedisValue[]> SetCombineAsync(SetOperation operation, RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SetCombineAsync", key: string.Join(separator: ",", values: keys)))
            {
                return _db.SetCombineAsync(operation: operation, keys: keys, flags: flags);
            }
        }

        public Task<long> SetCombineAndStoreAsync(SetOperation operation, RedisKey destination, RedisKey first, RedisKey second, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SetCombineAndStoreAsync"))
            {
                return _db.SetCombineAndStoreAsync(operation: operation, destination: destination, first: first, second: second, flags: flags);
            }
        }

        public Task<long> SetCombineAndStoreAsync(SetOperation operation, RedisKey destination, RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SetCombineAndStoreAsync", key: string.Join(separator: ",", values: keys)))
            {
                return _db.SetCombineAndStoreAsync(operation: operation, destination: destination, keys: keys, flags: flags);
            }
        }

        public Task<bool> SetContainsAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SetContainsAsync", key: key))
            {
                return _db.SetContainsAsync(key: key, value: value, flags: flags);
            }
        }

        public Task<long> SetLengthAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SetLengthAsync", key: key))
            {
                return _db.SetLengthAsync(key: key, flags: flags);
            }
        }

        public Task<RedisValue[]> SetMembersAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SetMembersAsync", key: key))
            {
                return _db.SetMembersAsync(key: key, flags: flags);
            }
        }

        public Task<bool> SetMoveAsync(RedisKey source, RedisKey destination, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SetMoveAsync"))
            {
                return _db.SetMoveAsync(source: source, destination: destination, value: value, flags: flags);
            }
        }

        public Task<RedisValue> SetPopAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SetPopAsync", key: key))
            {
                return _db.SetPopAsync(key: key, flags: flags);
            }
        }

        public Task<RedisValue[]> SetPopAsync(RedisKey key, long count, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SetPopAsync", key: key))
            {
                return _db.SetPopAsync(key: key, count: count, flags: flags);
            }
        }

        public Task<RedisValue> SetRandomMemberAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SetRandomMemberAsync", key: key))
            {
                return _db.SetRandomMemberAsync(key: key, flags: flags);
            }
        }

        public Task<RedisValue[]> SetRandomMembersAsync(RedisKey key, long count, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SetRandomMembersAsync", key: key))
            {
                return _db.SetRandomMembersAsync(key: key, count: count, flags: flags);
            }
        }

        public Task<bool> SetRemoveAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SetRemoveAsync", key: key))
            {
                return _db.SetRemoveAsync(key: key, value: value, flags: flags);
            }
        }

        public Task<long> SetRemoveAsync(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SetRemoveAsync", key: key))
            {
                return _db.SetRemoveAsync(key: key, values: values, flags: flags);
            }
        }

        public Task<RedisValue[]> SortAsync(RedisKey key, long skip = 0, long take = -1, Order order = Order.Ascending, SortType sortType = SortType.Numeric, RedisValue by = new(), RedisValue[] get = null, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SortAsync", key: key))
            {
                return _db.SortAsync(key: key, skip: skip, take: take, order: order, sortType: sortType, by: by, get: get, flags: flags);
            }
        }

        public Task<long> SortAndStoreAsync(RedisKey destination, RedisKey key, long skip = 0, long take = -1, Order order = Order.Ascending, SortType sortType = SortType.Numeric, RedisValue by = new(), RedisValue[] get = null, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SortAndStoreAsync", key: key))
            {
                return _db.SortAndStoreAsync(destination: destination, key: key, skip: skip, take: take, order: order, sortType: sortType, by: by, get: get, flags: flags);
            }
        }

        public Task<bool> SortedSetAddAsync(RedisKey key, RedisValue member, double score, CommandFlags flags)
        {
            using (FsLinkTrack.TrackRedis(method: "SortedSetAddAsync", key: key))
            {
                return _db.SortedSetAddAsync(key: key, member: member, score: score, flags: flags);
            }
        }

        public Task<bool> SortedSetAddAsync(RedisKey key, RedisValue member, double score, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SortedSetAddAsync", key: key))
            {
                return _db.SortedSetAddAsync(key: key, member: member, score: score, when: when, flags: flags);
            }
        }

        public Task<long> SortedSetAddAsync(RedisKey key, SortedSetEntry[] values, CommandFlags flags)
        {
            using (FsLinkTrack.TrackRedis(method: "SortedSetAddAsync", key: key))
            {
                return _db.SortedSetAddAsync(key: key, values: values, flags: flags);
            }
        }

        public Task<long> SortedSetAddAsync(RedisKey key, SortedSetEntry[] values, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SortedSetAddAsync", key: key))
            {
                return _db.SortedSetAddAsync(key: key, values: values, when: when, flags: flags);
            }
        }

        public Task<long> SortedSetCombineAndStoreAsync(SetOperation operation, RedisKey destination, RedisKey first, RedisKey second, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SortedSetCombineAndStoreAsync"))
            {
                return _db.SortedSetCombineAndStoreAsync(operation: operation, destination: destination, first: first, second: second, aggregate: aggregate, flags: flags);
            }
        }

        public Task<long> SortedSetCombineAndStoreAsync(SetOperation operation, RedisKey destination, RedisKey[] keys, double[] weights = null, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SortedSetCombineAndStoreAsync", key: string.Join(separator: ",", values: keys)))
            {
                return _db.SortedSetCombineAndStoreAsync(operation: operation, destination: destination, keys: keys, weights: weights, aggregate: aggregate, flags: flags);
            }
        }

        public Task<double> SortedSetDecrementAsync(RedisKey key, RedisValue member, double value, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SortedSetDecrementAsync", key: key))
            {
                return _db.SortedSetDecrementAsync(key: key, member: member, value: value, flags: flags);
            }
        }

        public Task<double> SortedSetIncrementAsync(RedisKey key, RedisValue member, double value, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SortedSetIncrementAsync", key: key))
            {
                return _db.SortedSetIncrementAsync(key: key, member: member, value: value, flags: flags);
            }
        }

        public Task<long> SortedSetLengthAsync(RedisKey key, double min = double.NegativeInfinity, double max = double.PositiveInfinity, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SortedSetLengthAsync", key: key))
            {
                return _db.SortedSetLengthAsync(key: key, min: min, max: max, exclude: exclude, flags: flags);
            }
        }

        public Task<long> SortedSetLengthByValueAsync(RedisKey key, RedisValue min, RedisValue max, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SortedSetLengthByValueAsync", key: key))
            {
                return _db.SortedSetLengthByValueAsync(key: key, min: min, max: max, exclude: exclude, flags: flags);
            }
        }

        public Task<RedisValue[]> SortedSetRangeByRankAsync(RedisKey key, long start = 0, long stop = -1, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SortedSetRangeByRankAsync", key: key))
            {
                return _db.SortedSetRangeByRankAsync(key: key, start: start, stop: stop, order: order, flags: flags);
            }
        }

        public Task<SortedSetEntry[]> SortedSetRangeByRankWithScoresAsync(RedisKey key, long start = 0, long stop = -1, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SortedSetRangeByRankWithScoresAsync", key: key))
            {
                return _db.SortedSetRangeByRankWithScoresAsync(key: key, start: start, stop: stop, order: order, flags: flags);
            }
        }

        public Task<RedisValue[]> SortedSetRangeByScoreAsync(RedisKey key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SortedSetRangeByScoreAsync", key: key))
            {
                return _db.SortedSetRangeByScoreAsync(key: key, start: start, stop: stop, exclude: exclude, order: order, skip: skip, take: take, flags: flags);
            }
        }

        public Task<SortedSetEntry[]> SortedSetRangeByScoreWithScoresAsync(RedisKey key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SortedSetRangeByScoreWithScoresAsync", key: key))
            {
                return _db.SortedSetRangeByScoreWithScoresAsync(key: key, start: start, stop: stop, exclude: exclude, order: order, skip: skip, take: take, flags: flags);
            }
        }

        public Task<RedisValue[]> SortedSetRangeByValueAsync(RedisKey key, RedisValue min, RedisValue max, Exclude exclude, long skip, long take = -1, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SortedSetRangeByValueAsync", key: key))
            {
                return _db.SortedSetRangeByValueAsync(key: key, min: min, max: max, exclude: exclude, skip: skip, take: take, flags: flags);
            }
        }

        public Task<RedisValue[]> SortedSetRangeByValueAsync(RedisKey key, RedisValue min = new(), RedisValue max = new(), Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SortedSetRangeByValueAsync", key: key))
            {
                return _db.SortedSetRangeByValueAsync(key: key, min: min, max: max, exclude: exclude, order: order, skip: skip, take: take, flags: flags);
            }
        }

        public Task<long?> SortedSetRankAsync(RedisKey key, RedisValue member, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SortedSetRankAsync", key: key))
            {
                return _db.SortedSetRankAsync(key: key, member: member, order: order, flags: flags);
            }
        }

        public Task<bool> SortedSetRemoveAsync(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SortedSetRemoveAsync", key: key))
            {
                return _db.SortedSetRemoveAsync(key: key, member: member, flags: flags);
            }
        }

        public Task<long> SortedSetRemoveAsync(RedisKey key, RedisValue[] members, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SortedSetRemoveAsync", key: key))
            {
                return _db.SortedSetRemoveAsync(key: key, members: members, flags: flags);
            }
        }

        public Task<long> SortedSetRemoveRangeByRankAsync(RedisKey key, long start, long stop, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SortedSetRemoveRangeByRankAsync", key: key))
            {
                return _db.SortedSetRemoveRangeByRankAsync(key: key, start: start, stop: stop, flags: flags);
            }
        }

        public Task<long> SortedSetRemoveRangeByScoreAsync(RedisKey key, double start, double stop, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SortedSetRemoveRangeByScoreAsync", key: key))
            {
                return _db.SortedSetRemoveRangeByScoreAsync(key: key, start: start, stop: stop, exclude: exclude, flags: flags);
            }
        }

        public Task<long> SortedSetRemoveRangeByValueAsync(RedisKey key, RedisValue min, RedisValue max, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SortedSetRemoveRangeByValueAsync", key: key))
            {
                return _db.SortedSetRemoveRangeByValueAsync(key: key, min: min, max: max, exclude: exclude, flags: flags);
            }
        }

        public IAsyncEnumerable<RedisValue> SetScanAsync(RedisKey key, RedisValue pattern = new(), int pageSize = 250, long cursor = 0, int pageOffset = 0, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SetScanAsync", key: key))
            {
                return _db.SetScanAsync(key: key, pattern: pattern, pageSize: pageSize, cursor: cursor, pageOffset: pageOffset, flags: flags);
            }
        }

        public IAsyncEnumerable<SortedSetEntry> SortedSetScanAsync(RedisKey key, RedisValue pattern = new(), int pageSize = 250, long cursor = 0, int pageOffset = 0, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SortedSetScanAsync", key: key))
            {
                return _db.SortedSetScanAsync(key: key, pattern: pattern, pageSize: pageSize, cursor: cursor, pageOffset: pageOffset, flags: flags);
            }
        }

        public Task<double?> SortedSetScoreAsync(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SortedSetScoreAsync", key: key))
            {
                return _db.SortedSetScoreAsync(key: key, member: member, flags: flags);
            }
        }

        public Task<SortedSetEntry?> SortedSetPopAsync(RedisKey key, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SortedSetPopAsync", key: key))
            {
                return _db.SortedSetPopAsync(key: key, order: order, flags: flags);
            }
        }

        public Task<SortedSetEntry[]> SortedSetPopAsync(RedisKey key, long count, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SortedSetPopAsync", key: key))
            {
                return _db.SortedSetPopAsync(key: key, count: count, order: order, flags: flags);
            }
        }

        public Task<long> StreamAcknowledgeAsync(RedisKey key, RedisValue groupName, RedisValue messageId, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StreamAcknowledgeAsync", key: key))
            {
                return _db.StreamAcknowledgeAsync(key: key, groupName: groupName, messageId: messageId, flags: flags);
            }
        }

        public Task<long> StreamAcknowledgeAsync(RedisKey key, RedisValue groupName, RedisValue[] messageIds, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StreamAcknowledgeAsync", key: key))
            {
                return _db.StreamAcknowledgeAsync(key: key, groupName: groupName, messageIds: messageIds, flags: flags);
            }
        }

        public Task<RedisValue> StreamAddAsync(RedisKey key, RedisValue streamField, RedisValue streamValue, RedisValue? messageId = null, int? maxLength = null, bool useApproximateMaxLength = false, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StreamAddAsync", key: key))
            {
                return _db.StreamAddAsync(key: key, streamField: streamField, streamValue: streamValue, messageId: messageId, maxLength: maxLength, useApproximateMaxLength: useApproximateMaxLength, flags: flags);
            }
        }

        public Task<RedisValue> StreamAddAsync(RedisKey key, NameValueEntry[] streamPairs, RedisValue? messageId = null, int? maxLength = null, bool useApproximateMaxLength = false, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StreamAddAsync", key: key))
            {
                return _db.StreamAddAsync(key: key, streamPairs: streamPairs, messageId: messageId, maxLength: maxLength, useApproximateMaxLength: useApproximateMaxLength, flags: flags);
            }
        }

        public Task<StreamEntry[]> StreamClaimAsync(RedisKey key, RedisValue consumerGroup, RedisValue claimingConsumer, long minIdleTimeInMs, RedisValue[] messageIds, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StreamClaimAsync", key: key))
            {
                return _db.StreamClaimAsync(key: key, consumerGroup: consumerGroup, claimingConsumer: claimingConsumer, minIdleTimeInMs: minIdleTimeInMs, messageIds: messageIds, flags: flags);
            }
        }

        public Task<RedisValue[]> StreamClaimIdsOnlyAsync(RedisKey key, RedisValue consumerGroup, RedisValue claimingConsumer, long minIdleTimeInMs, RedisValue[] messageIds, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StreamClaimIdsOnlyAsync", key: key))
            {
                return _db.StreamClaimIdsOnlyAsync(key: key, consumerGroup: consumerGroup, claimingConsumer: claimingConsumer, minIdleTimeInMs: minIdleTimeInMs, messageIds: messageIds, flags: flags);
            }
        }

        public Task<bool> StreamConsumerGroupSetPositionAsync(RedisKey key, RedisValue groupName, RedisValue position, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StreamConsumerGroupSetPositionAsync", key: key))
            {
                return _db.StreamConsumerGroupSetPositionAsync(key: key, groupName: groupName, position: position, flags: flags);
            }
        }

        public Task<StreamConsumerInfo[]> StreamConsumerInfoAsync(RedisKey key, RedisValue groupName, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StreamConsumerInfoAsync", key: key))
            {
                return _db.StreamConsumerInfoAsync(key: key, groupName: groupName, flags: flags);
            }
        }

        public Task<bool> StreamCreateConsumerGroupAsync(RedisKey key, RedisValue groupName, RedisValue? position, CommandFlags flags)
        {
            using (FsLinkTrack.TrackRedis(method: "StreamCreateConsumerGroupAsync", key: key))
            {
                return _db.StreamCreateConsumerGroupAsync(key: key, groupName: groupName, position: position, flags: flags);
            }
        }

        public Task<bool> StreamCreateConsumerGroupAsync(RedisKey key, RedisValue groupName, RedisValue? position = null, bool createStream = true, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StreamCreateConsumerGroupAsync", key: key))
            {
                return _db.StreamCreateConsumerGroupAsync(key: key, groupName: groupName, position: position, createStream: createStream, flags: flags);
            }
        }

        public Task<long> StreamDeleteAsync(RedisKey key, RedisValue[] messageIds, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StreamDeleteAsync", key: key))
            {
                return _db.StreamDeleteAsync(key: key, messageIds: messageIds, flags: flags);
            }
        }

        public Task<long> StreamDeleteConsumerAsync(RedisKey key, RedisValue groupName, RedisValue consumerName, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StreamDeleteConsumerAsync", key: key))
            {
                return _db.StreamDeleteConsumerAsync(key: key, groupName: groupName, consumerName: consumerName, flags: flags);
            }
        }

        public Task<bool> StreamDeleteConsumerGroupAsync(RedisKey key, RedisValue groupName, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StreamDeleteConsumerGroupAsync", key: key))
            {
                return _db.StreamDeleteConsumerGroupAsync(key: key, groupName: groupName, flags: flags);
            }
        }

        public Task<StreamGroupInfo[]> StreamGroupInfoAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StreamGroupInfoAsync", key: key))
            {
                return _db.StreamGroupInfoAsync(key: key, flags: flags);
            }
        }

        public Task<StreamInfo> StreamInfoAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StreamInfoAsync", key: key))
            {
                return _db.StreamInfoAsync(key: key, flags: flags);
            }
        }

        public Task<long> StreamLengthAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StreamLengthAsync", key: key))
            {
                return _db.StreamLengthAsync(key: key, flags: flags);
            }
        }

        public Task<StreamPendingInfo> StreamPendingAsync(RedisKey key, RedisValue groupName, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StreamPendingAsync", key: key))
            {
                return _db.StreamPendingAsync(key: key, groupName: groupName, flags: flags);
            }
        }

        public Task<StreamPendingMessageInfo[]> StreamPendingMessagesAsync(RedisKey key, RedisValue groupName, int count, RedisValue consumerName, RedisValue? minId = null, RedisValue? maxId = null, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StreamPendingMessagesAsync", key: key))
            {
                return _db.StreamPendingMessagesAsync(key: key, groupName: groupName, count: count, consumerName: consumerName, minId: minId, maxId: maxId, flags: flags);
            }
        }

        public Task<StreamEntry[]> StreamRangeAsync(RedisKey key, RedisValue? minId = null, RedisValue? maxId = null, int? count = null, Order messageOrder = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StreamRangeAsync", key: key))
            {
                return _db.StreamRangeAsync(key: key, minId: minId, maxId: maxId, count: count, messageOrder: messageOrder, flags: flags);
            }
        }

        public Task<StreamEntry[]> StreamReadAsync(RedisKey key, RedisValue position, int? count = null, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StreamReadAsync", key: key))
            {
                return _db.StreamReadAsync(key: key, position: position, count: count, flags: flags);
            }
        }

        public Task<RedisStream[]> StreamReadAsync(StreamPosition[] streamPositions, int? countPerStream = null, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StreamReadAsync"))
            {
                return _db.StreamReadAsync(streamPositions: streamPositions, countPerStream: countPerStream, flags: flags);
            }
        }

        public Task<StreamEntry[]> StreamReadGroupAsync(RedisKey key, RedisValue groupName, RedisValue consumerName, RedisValue? position, int? count, CommandFlags flags)
        {
            using (FsLinkTrack.TrackRedis(method: "StreamReadGroupAsync", key: key))
            {
                return _db.StreamReadGroupAsync(key: key, groupName: groupName, consumerName: consumerName, position: position, count: count, flags: flags);
            }
        }

        public Task<StreamEntry[]> StreamReadGroupAsync(RedisKey key, RedisValue groupName, RedisValue consumerName, RedisValue? position = null, int? count = null, bool noAck = false, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StreamReadGroupAsync", key: key))
            {
                return _db.StreamReadGroupAsync(key: key, groupName: groupName, consumerName: consumerName, position: position, count: count, noAck: noAck, flags: flags);
            }
        }

        public Task<RedisStream[]> StreamReadGroupAsync(StreamPosition[] streamPositions, RedisValue groupName, RedisValue consumerName, int? countPerStream, CommandFlags flags)
        {
            using (FsLinkTrack.TrackRedis(method: "StreamReadGroupAsync"))
            {
                return _db.StreamReadGroupAsync(streamPositions: streamPositions, groupName: groupName, consumerName: consumerName, countPerStream: countPerStream, flags: flags);
            }
        }

        public Task<RedisStream[]> StreamReadGroupAsync(StreamPosition[] streamPositions, RedisValue groupName, RedisValue consumerName, int? countPerStream = null, bool noAck = false, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StreamReadGroupAsync"))
            {
                return _db.StreamReadGroupAsync(streamPositions: streamPositions, groupName: groupName, consumerName: consumerName, countPerStream: countPerStream, noAck: noAck, flags: flags);
            }
        }

        public Task<long> StreamTrimAsync(RedisKey key, int maxLength, bool useApproximateMaxLength = false, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StreamTrimAsync", key: key))
            {
                return _db.StreamTrimAsync(key: key, maxLength: maxLength, useApproximateMaxLength: useApproximateMaxLength, flags: flags);
            }
        }

        public Task<long> StringAppendAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StringAppendAsync", key: key))
            {
                return _db.StringAppendAsync(key: key, value: value, flags: flags);
            }
        }

        public Task<long> StringBitCountAsync(RedisKey key, long start = 0, long end = -1, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StringBitCountAsync", key: key))
            {
                return _db.StringBitCountAsync(key: key, start: start, end: end, flags: flags);
            }
        }

        public Task<long> StringBitOperationAsync(Bitwise operation, RedisKey destination, RedisKey first, RedisKey second = new(), CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StringBitOperationAsync"))
            {
                return _db.StringBitOperationAsync(operation: operation, destination: destination, first: first, second: second, flags: flags);
            }
        }

        public Task<long> StringBitOperationAsync(Bitwise operation, RedisKey destination, RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StringBitOperationAsync", key: string.Join(separator: ",", values: keys)))
            {
                return _db.StringBitOperationAsync(operation: operation, destination: destination, keys: keys, flags: flags);
            }
        }

        public Task<long> StringBitPositionAsync(RedisKey key, bool bit, long start = 0, long end = -1, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StringBitPositionAsync", key: key))
            {
                return _db.StringBitPositionAsync(key: key, bit: bit, start: start, end: end, flags: flags);
            }
        }

        public Task<long> StringDecrementAsync(RedisKey key, long value = 1, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StringDecrementAsync", key: key))
            {
                return _db.StringDecrementAsync(key: key, value: value, flags: flags);
            }
        }

        public Task<double> StringDecrementAsync(RedisKey key, double value, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StringDecrementAsync", key: key))
            {
                return _db.StringDecrementAsync(key: key, value: value, flags: flags);
            }
        }

        public Task<RedisValue> StringGetAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StringGetAsync", key: key))
            {
                return _db.StringGetAsync(key: key, flags: flags);
            }
        }

        public Task<RedisValue[]> StringGetAsync(RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StringGetAsync", key: string.Join(separator: ",", values: keys)))
            {
                return _db.StringGetAsync(keys: keys, flags: flags);
            }
        }

        public Task<Lease<byte>> StringGetLeaseAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StringGetLeaseAsync", key: key))
            {
                return _db.StringGetLeaseAsync(key: key, flags: flags);
            }
        }

        public Task<bool> StringGetBitAsync(RedisKey key, long offset, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StringGetBitAsync", key: key))
            {
                return _db.StringGetBitAsync(key: key, offset: offset, flags: flags);
            }
        }

        public Task<RedisValue> StringGetRangeAsync(RedisKey key, long start, long end, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StringGetRangeAsync", key: key))
            {
                return _db.StringGetRangeAsync(key: key, start: start, end: end, flags: flags);
            }
        }

        public Task<RedisValue> StringGetSetAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StringGetSetAsync", key: key))
            {
                return _db.StringGetSetAsync(key: key, value: value, flags: flags);
            }
        }
        public Task<RedisValue> StringGetDeleteAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StringGetDeleteAsync", key: key))
            {
                return _db.StringGetDeleteAsync(key: key, flags: flags);
            }
        }

        public Task<RedisValueWithExpiry> StringGetWithExpiryAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StringGetWithExpiryAsync", key: key))
            {
                return _db.StringGetWithExpiryAsync(key: key, flags: flags);
            }
        }

        public Task<long> StringIncrementAsync(RedisKey key, long value = 1, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StringIncrementAsync", key: key))
            {
                return _db.StringIncrementAsync(key: key, value: value, flags: flags);
            }
        }

        public Task<double> StringIncrementAsync(RedisKey key, double value, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StringIncrementAsync", key: key))
            {
                return _db.StringIncrementAsync(key: key, value: value, flags: flags);
            }
        }

        public Task<long> StringLengthAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StringLengthAsync", key: key))
            {
                return _db.StringLengthAsync(key: key, flags: flags);
            }
        }

        public Task<bool> StringSetAsync(RedisKey key, RedisValue value, TimeSpan? expiry = null, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StringSetAsync", key: key))
            {
                return _db.StringSetAsync(key: key, value: value, expiry: expiry, when: when, flags: flags);
            }
        }

        public Task<bool> StringSetAsync(KeyValuePair<RedisKey, RedisValue>[] values, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StringSetAsync"))
            {
                return _db.StringSetAsync(values: values, when: when, flags: flags);
            }
        }

        public Task<bool> StringSetBitAsync(RedisKey key, long offset, bool bit, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StringSetBitAsync", key: key))
            {
                return _db.StringSetBitAsync(key: key, offset: offset, bit: bit, flags: flags);
            }
        }

        public Task<RedisValue> StringSetRangeAsync(RedisKey key, long offset, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StringSetRangeAsync", key: key))
            {
                return _db.StringSetRangeAsync(key: key, offset: offset, value: value, flags: flags);
            }
        }

        public Task<bool> KeyTouchAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "KeyTouchAsync", key: key))
            {
                return _db.KeyTouchAsync(key: key, flags: flags);
            }
        }

        public Task<long> KeyTouchAsync(RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "KeyTouchAsync", key: string.Join(separator: ",", values: keys)))
            {
                return _db.KeyTouchAsync(keys: keys, flags: flags);
            }
        }

        public IBatch CreateBatch(object asyncState = null) => _db.CreateBatch(asyncState: asyncState);

        public ITransaction CreateTransaction(object asyncState = null) => _db.CreateTransaction(asyncState: asyncState);

        public void KeyMigrate(RedisKey key, EndPoint toServer, int toDatabase = 0, int timeoutMilliseconds = 0, MigrateOptions migrateOptions = MigrateOptions.None, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "KeyMigrate", key: key))
            {
                _db.KeyMigrate(key: key, toServer: toServer, toDatabase: toDatabase, timeoutMilliseconds: timeoutMilliseconds, migrateOptions: migrateOptions, flags: flags);
            }
        }

        public RedisValue DebugObject(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "DebugObject", key: key))
            {
                return _db.DebugObject(key: key, flags: flags);
            }
        }

        public bool GeoAdd(RedisKey key, double longitude, double latitude, RedisValue member, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "GeoAdd", key: key))
            {
                return _db.GeoAdd(key: key, longitude: longitude, latitude: latitude, member: member, flags: flags);
            }
        }

        public bool GeoAdd(RedisKey key, GeoEntry value, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "GeoAdd", key: key))
            {
                return _db.GeoAdd(key: key, value: value, flags: flags);
            }
        }

        public long GeoAdd(RedisKey key, GeoEntry[] values, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "GeoAdd", key: key))
            {
                return _db.GeoAdd(key: key, values: values, flags: flags);
            }
        }

        public bool GeoRemove(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "GeoRemove", key: key))
            {
                return _db.GeoRemove(key: key, member: member, flags: flags);
            }
        }

        public double? GeoDistance(RedisKey key, RedisValue member1, RedisValue member2, GeoUnit unit = GeoUnit.Meters, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "GeoDistance", key: key))
            {
                return _db.GeoDistance(key: key, member1: member1, member2: member2, unit: unit, flags: flags);
            }
        }

        public string[] GeoHash(RedisKey key, RedisValue[] members, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "GeoHash", key: key))
            {
                return _db.GeoHash(key: key, members: members, flags: flags);
            }
        }

        public string GeoHash(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "GeoHash", key: key))
            {
                return _db.GeoHash(key: key, member: member, flags: flags);
            }
        }

        public GeoPosition?[] GeoPosition(RedisKey key, RedisValue[] members, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "GeoPosition", key: key))
            {
                return _db.GeoPosition(key: key, members: members, flags: flags);
            }
        }

        public GeoPosition? GeoPosition(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "GeoPosition", key: key))
            {
                return _db.GeoPosition(key: key, member: member, flags: flags);
            }
        }

        public GeoRadiusResult[] GeoRadius(RedisKey key, RedisValue member, double radius, GeoUnit unit = GeoUnit.Meters, int count = -1, Order? order = null, GeoRadiusOptions options = GeoRadiusOptions.Default, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "GeoRadius", key: key))
            {
                return _db.GeoRadius(key: key, member: member, radius: radius, unit: unit, count: count, order: order, options: options, flags: flags);
            }
        }

        public GeoRadiusResult[] GeoRadius(RedisKey key, double longitude, double latitude, double radius, GeoUnit unit = GeoUnit.Meters, int count = -1, Order? order = null, GeoRadiusOptions options = GeoRadiusOptions.Default, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "GeoRadius", key: key))
            {
                return _db.GeoRadius(key: key, longitude: longitude, latitude: latitude, radius: radius, unit: unit, count: count, order: order, options: options, flags: flags);
            }
        }

        public long HashDecrement(RedisKey key, RedisValue hashField, long value = 1, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "HashDecrement", key: key, member: hashField))
            {
                return _db.HashDecrement(key: key, hashField: hashField, value: value, flags: flags);
            }
        }

        public double HashDecrement(RedisKey key, RedisValue hashField, double value, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "HashDecrement", key: key, member: hashField))
            {
                return _db.HashDecrement(key: key, hashField: hashField, value: value, flags: flags);
            }
        }

        public bool HashDelete(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "HashDelete", key: key, member: hashField))
            {
                return _db.HashDelete(key: key, hashField: hashField, flags: flags);
            }
        }

        public long HashDelete(RedisKey key, RedisValue[] hashFields, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "HashDelete", key: key, member: string.Join(separator: ",", values: hashFields)))
            {
                return _db.HashDelete(key: key, hashFields: hashFields, flags: flags);
            }
        }

        public bool HashExists(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "HashExists", key: key, member: hashField))
            {
                return _db.HashExists(key: key, hashField: hashField, flags: flags);
            }
        }

        public RedisValue HashGet(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "HashGet", key: key, member: hashField))
            {
                return _db.HashGet(key: key, hashField: hashField, flags: flags);
            }
        }

        public Lease<byte> HashGetLease(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "HashGetLease", key: key, member: hashField))
            {
                return _db.HashGetLease(key: key, hashField: hashField, flags: flags);
            }
        }

        public RedisValue[] HashGet(RedisKey key, RedisValue[] hashFields, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "HashGet", key: key, member: string.Join(separator: ",", values: hashFields)))
            {
                return _db.HashGet(key: key, hashFields: hashFields, flags: flags);
            }
        }

        public HashEntry[] HashGetAll(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "HashGetAll", key: key))
            {
                return _db.HashGetAll(key: key, flags: flags);
            }
        }

        public long HashIncrement(RedisKey key, RedisValue hashField, long value = 1, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "HashIncrement", key: key, member: hashField))
            {
                return _db.HashIncrement(key: key, hashField: hashField, value: value, flags: flags);
            }
        }

        public double HashIncrement(RedisKey key, RedisValue hashField, double value, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "HashIncrement", key: key, member: hashField))
            {
                return _db.HashIncrement(key: key, hashField: hashField, value: value, flags: flags);
            }
        }

        public RedisValue[] HashKeys(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "HashKeys", key: key))
            {
                return _db.HashKeys(key: key, flags: flags);
            }
        }

        public long HashLength(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "HashLength", key: key))
            {
                return _db.HashLength(key: key, flags: flags);
            }
        }

        public IEnumerable<HashEntry> HashScan(RedisKey key, RedisValue pattern, int pageSize, CommandFlags flags)
        {
            using (FsLinkTrack.TrackRedis(method: "HashScan", key: key))
            {
                return _db.HashScan(key: key, pattern: pattern, pageSize: pageSize, flags: flags);
            }
        }

        public IEnumerable<HashEntry> HashScan(RedisKey key, RedisValue pattern = new(), int pageSize = 250, long cursor = 0, int pageOffset = 0, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "HashScan", key: key))
            {
                return _db.HashScan(key: key, pattern: pattern, pageSize: pageSize, cursor: cursor, pageOffset: pageOffset, flags: flags);
            }
        }

        public void HashSet(RedisKey key, HashEntry[] hashFields, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "HashSet", key: key, member: string.Join(separator: ",", values: hashFields)))
            {
                _db.HashSet(key: key, hashFields: hashFields, flags: flags);
            }
        }

        public bool HashSet(RedisKey key, RedisValue hashField, RedisValue value, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "HashSet", key: key, member: hashField))
            {
                return _db.HashSet(key: key, hashField: hashField, value: value, when: when, flags: flags);
            }
        }

        public long HashStringLength(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "HashStringLength", key: key, member: hashField))
            {
                return _db.HashStringLength(key: key, hashField: hashField, flags: flags);
            }
        }

        public RedisValue[] HashValues(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "HashValues", key: key))
            {
                return _db.HashValues(key: key, flags: flags);
            }
        }

        public bool HyperLogLogAdd(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "HyperLogLogAdd", key: key))
            {
                return _db.HyperLogLogAdd(key: key, value: value, flags: flags);
            }
        }

        public bool HyperLogLogAdd(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "HyperLogLogAdd", key: key))
            {
                return _db.HyperLogLogAdd(key: key, values: values, flags: flags);
            }
        }

        public long HyperLogLogLength(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "HyperLogLogLength", key: key))
            {
                return _db.HyperLogLogLength(key: key, flags: flags);
            }
        }

        public long HyperLogLogLength(RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "HyperLogLogLength", key: string.Join(separator: ",", values: keys)))
            {
                return _db.HyperLogLogLength(keys: keys, flags: flags);
            }
        }

        public void HyperLogLogMerge(RedisKey destination, RedisKey first, RedisKey second, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "HyperLogLogMerge"))
            {
                _db.HyperLogLogMerge(destination: destination, first: first, second: second, flags: flags);
            }
        }

        public void HyperLogLogMerge(RedisKey destination, RedisKey[] sourceKeys, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "HyperLogLogMerge"))
            {
                _db.HyperLogLogMerge(destination: destination, sourceKeys: sourceKeys, flags: flags);
            }
        }

        public EndPoint IdentifyEndpoint(RedisKey key = new(), CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "IdentifyEndpoint", key: key))
            {
                return _db.IdentifyEndpoint(key: key, flags: flags);
            }
        }

        public bool KeyDelete(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "KeyDelete", key: key))
            {
                return _db.KeyDelete(key: key, flags: flags);
            }
        }

        public long KeyDelete(RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "KeyDelete", key: string.Join(separator: ",", values: keys)))
            {
                return _db.KeyDelete(keys: keys, flags: flags);
            }
        }

        public byte[] KeyDump(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "KeyDump", key: key))
            {
                return _db.KeyDump(key: key, flags: flags);
            }
        }

        public bool KeyExists(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "KeyExists", key: key))
            {
                return _db.KeyExists(key: key, flags: flags);
            }
        }

        public long KeyExists(RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "KeyExists", key: string.Join(separator: ",", values: keys)))
            {
                return _db.KeyExists(keys: keys, flags: flags);
            }
        }

        public bool KeyExpire(RedisKey key, TimeSpan? expiry, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "KeyExpire", key: key))
            {
                return _db.KeyExpire(key: key, expiry: expiry, flags: flags);
            }
        }

        public bool KeyExpire(RedisKey key, DateTime? expiry, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "KeyExpire", key: key))
            {
                return _db.KeyExpire(key: key, expiry: expiry, flags: flags);
            }
        }

        public TimeSpan? KeyIdleTime(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "KeyIdleTime", key: key))
            {
                return _db.KeyIdleTime(key: key, flags: flags);
            }
        }

        public bool KeyMove(RedisKey key, int database, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "KeyMove", key: key))
            {
                return _db.KeyMove(key: key, database: database, flags: flags);
            }
        }

        public bool KeyPersist(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "KeyPersist", key: key))
            {
                return _db.KeyPersist(key: key, flags: flags);
            }
        }

        public RedisKey KeyRandom(CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "KeyRandom"))
            {
                return _db.KeyRandom(flags: flags);
            }
        }

        public bool KeyRename(RedisKey key, RedisKey newKey, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "KeyRename", key: key))
            {
                return _db.KeyRename(key: key, newKey: newKey, when: when, flags: flags);
            }
        }

        public void KeyRestore(RedisKey key, byte[] value, TimeSpan? expiry = null, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "KeyRestore", key: key))
            {
                _db.KeyRestore(key: key, value: value, expiry: expiry, flags: flags);
            }
        }

        public TimeSpan? KeyTimeToLive(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "KeyTimeToLive", key: key))
            {
                return _db.KeyTimeToLive(key: key, flags: flags);
            }
        }

        public RedisType KeyType(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "KeyType", key: key))
            {
                return _db.KeyType(key: key, flags: flags);
            }
        }

        public RedisValue ListGetByIndex(RedisKey key, long index, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "ListGetByIndex", key: key))
            {
                return _db.ListGetByIndex(key: key, index: index, flags: flags);
            }
        }

        public long ListInsertAfter(RedisKey key, RedisValue pivot, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "ListInsertAfter", key: key))
            {
                return _db.ListInsertAfter(key: key, pivot: pivot, value: value, flags: flags);
            }
        }

        public long ListInsertBefore(RedisKey key, RedisValue pivot, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "ListInsertBefore", key: key))
            {
                return _db.ListInsertBefore(key: key, pivot: pivot, value: value, flags: flags);
            }
        }

        public RedisValue ListLeftPop(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "ListLeftPop", key: key))
            {
                return _db.ListLeftPop(key: key, flags: flags);
            }
        }
        public RedisValue[] ListLeftPop(RedisKey key, long count, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "ListLeftPop", key: key))
            {
                return _db.ListLeftPop(key: key, count, flags: flags);
            }
        }

        public long ListLeftPush(RedisKey key, RedisValue value, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "ListLeftPush", key: key))
            {
                return _db.ListLeftPush(key: key, value: value, when: when, flags: flags);
            }
        }

        public long ListLeftPush(RedisKey key, RedisValue[] values, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "ListLeftPush", key: key))
            {
                return _db.ListLeftPush(key: key, values: values, when: when, flags: flags);
            }
        }

        public long ListLeftPush(RedisKey key, RedisValue[] values, CommandFlags flags)
        {
            using (FsLinkTrack.TrackRedis(method: "ListLeftPush", key: key))
            {
                return _db.ListLeftPush(key: key, values: values, flags: flags);
            }
        }

        public long ListLength(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "ListLength", key: key))
            {
                return _db.ListLength(key: key, flags: flags);
            }
        }

        public RedisValue[] ListRange(RedisKey key, long start = 0, long stop = -1, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "ListRange", key: key))
            {
                return _db.ListRange(key: key, start: start, stop: stop, flags: flags);
            }
        }

        public long ListRemove(RedisKey key, RedisValue value, long count = 0, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "ListRemove", key: key))
            {
                return _db.ListRemove(key: key, value: value, count: count, flags: flags);
            }
        }

        public RedisValue ListRightPop(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "ListRightPop", key: key))
            {
                return _db.ListRightPop(key: key, flags: flags);
            }
        }
        public RedisValue[] ListRightPop(RedisKey key, long count, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "ListRightPop", key: key))
            {
                return _db.ListRightPop(key: key, count, flags: flags);
            }
        }

        public RedisValue ListRightPopLeftPush(RedisKey source, RedisKey destination, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "ListRightPopLeftPush"))
            {
                return _db.ListRightPopLeftPush(source: source, destination: destination, flags: flags);
            }
        }

        public long ListRightPush(RedisKey key, RedisValue value, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "ListRightPush", key: key))
            {
                return _db.ListRightPush(key: key, value: value, when: when, flags: flags);
            }
        }

        public long ListRightPush(RedisKey key, RedisValue[] values, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "ListRightPush", key: key))
            {
                return _db.ListRightPush(key: key, values: values, when: when, flags: flags);
            }
        }

        public long ListRightPush(RedisKey key, RedisValue[] values, CommandFlags flags)
        {
            using (FsLinkTrack.TrackRedis(method: "ListRightPush", key: key))
            {
                return _db.ListRightPush(key: key, values: values, flags: flags);
            }
        }

        public void ListSetByIndex(RedisKey key, long index, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "ListSetByIndex", key: key))
            {
                _db.ListSetByIndex(key: key, index: index, value: value, flags: flags);
            }
        }

        public void ListTrim(RedisKey key, long start, long stop, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "ListTrim", key: key))
            {
                _db.ListTrim(key: key, start: start, stop: stop, flags: flags);
            }
        }

        public bool LockExtend(RedisKey key, RedisValue value, TimeSpan expiry, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "LockExtend", key: key))
            {
                return _db.LockExtend(key: key, value: value, expiry: expiry, flags: flags);
            }
        }

        public RedisValue LockQuery(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "LockQuery", key: key))
            {
                return _db.LockQuery(key: key, flags: flags);
            }
        }

        public bool LockRelease(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "LockRelease", key: key))
            {
                return _db.LockRelease(key: key, value: value, flags: flags);
            }
        }

        public bool LockTake(RedisKey key, RedisValue value, TimeSpan expiry, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "LockTake", key: key))
            {
                return _db.LockTake(key: key, value: value, expiry: expiry, flags: flags);
            }
        }

        public long Publish(RedisChannel channel, RedisValue message, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "Publish"))
            {
                return _db.Publish(channel: channel, message: message, flags: flags);
            }
        }

        public RedisResult Execute(string command, params object[] args)
        {
            using (FsLinkTrack.TrackRedis(method: "Execute"))
            {
                return _db.Execute(command: command, args: args);
            }
        }

        public RedisResult Execute(string command, ICollection<object> args, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "Execute"))
            {
                return _db.Execute(command: command, args: args, flags: flags);
            }
        }

        public RedisResult ScriptEvaluate(string script, RedisKey[] keys = null, RedisValue[] values = null, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "ScriptEvaluate", key: string.Join(separator: ",", values: keys)))
            {
                return _db.ScriptEvaluate(script: script, keys: keys, values: values, flags: flags);
            }
        }

        public RedisResult ScriptEvaluate(byte[] hash, RedisKey[] keys = null, RedisValue[] values = null, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "ScriptEvaluate", key: string.Join(separator: ",", values: keys)))
            {
                return _db.ScriptEvaluate(hash: hash, keys: keys, values: values, flags: flags);
            }
        }

        public RedisResult ScriptEvaluate(LuaScript script, object parameters = null, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "ScriptEvaluate"))
            {
                return _db.ScriptEvaluate(script: script, parameters: parameters, flags: flags);
            }
        }

        public RedisResult ScriptEvaluate(LoadedLuaScript script, object parameters = null, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "ScriptEvaluate"))
            {
                return _db.ScriptEvaluate(script: script, parameters: parameters, flags: flags);
            }
        }

        public bool SetAdd(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SetAdd", key: key))
            {
                return _db.SetAdd(key: key, value: value, flags: flags);
            }
        }

        public long SetAdd(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SetAdd", key: key))
            {
                return _db.SetAdd(key: key, values: values, flags: flags);
            }
        }

        public RedisValue[] SetCombine(SetOperation operation, RedisKey first, RedisKey second, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SetCombine"))
            {
                return _db.SetCombine(operation: operation, first: first, second: second, flags: flags);
            }
        }

        public RedisValue[] SetCombine(SetOperation operation, RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SetCombine", key: string.Join(separator: ",", values: keys)))
            {
                return _db.SetCombine(operation: operation, keys: keys, flags: flags);
            }
        }

        public long SetCombineAndStore(SetOperation operation, RedisKey destination, RedisKey first, RedisKey second, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SetCombineAndStore"))
            {
                return _db.SetCombineAndStore(operation: operation, destination: destination, first: first, second: second, flags: flags);
            }
        }

        public long SetCombineAndStore(SetOperation operation, RedisKey destination, RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SetCombineAndStore", key: string.Join(separator: ",", values: keys)))
            {
                return _db.SetCombineAndStore(operation: operation, destination: destination, keys: keys, flags: flags);
            }
        }

        public bool SetContains(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SetContains", key: key))
            {
                return _db.SetContains(key: key, value: value, flags: flags);
            }
        }

        public long SetLength(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SetLength", key: key))
            {
                return _db.SetLength(key: key, flags: flags);
            }
        }

        public RedisValue[] SetMembers(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SetMembers", key: key))
            {
                return _db.SetMembers(key: key, flags: flags);
            }
        }

        public bool SetMove(RedisKey source, RedisKey destination, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SetMove"))
            {
                return _db.SetMove(source: source, destination: destination, value: value, flags: flags);
            }
        }

        public RedisValue SetPop(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SetPop", key: key))
            {
                return _db.SetPop(key: key, flags: flags);
            }
        }

        public RedisValue[] SetPop(RedisKey key, long count, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SetPop", key: key))
            {
                return _db.SetPop(key: key, count: count, flags: flags);
            }
        }

        public RedisValue SetRandomMember(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SetRandomMember", key: key))
            {
                return _db.SetRandomMember(key: key, flags: flags);
            }
        }

        public RedisValue[] SetRandomMembers(RedisKey key, long count, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SetRandomMembers", key: key))
            {
                return _db.SetRandomMembers(key: key, count: count, flags: flags);
            }
        }

        public bool SetRemove(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SetRemove", key: key))
            {
                return _db.SetRemove(key: key, value: value, flags: flags);
            }
        }

        public long SetRemove(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SetRemove", key: key))
            {
                return _db.SetRemove(key: key, values: values, flags: flags);
            }
        }

        public IEnumerable<RedisValue> SetScan(RedisKey key, RedisValue pattern, int pageSize, CommandFlags flags)
        {
            using (FsLinkTrack.TrackRedis(method: "SetScan", key: key))
            {
                return _db.SetScan(key: key, pattern: pattern, pageSize: pageSize, flags: flags);
            }
        }

        public IEnumerable<RedisValue> SetScan(RedisKey key, RedisValue pattern = new(), int pageSize = 250, long cursor = 0, int pageOffset = 0, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SetScan", key: key))
            {
                return _db.SetScan(key: key, pattern: pattern, pageSize: pageSize, cursor: cursor, pageOffset: pageOffset, flags: flags);
            }
        }

        public RedisValue[] Sort(RedisKey key, long skip = 0, long take = -1, Order order = Order.Ascending, SortType sortType = SortType.Numeric, RedisValue by = new(), RedisValue[] get = null, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "Sort", key: key))
            {
                return _db.Sort(key: key, skip: skip, take: take, order: order, sortType: sortType, by: by, get: get, flags: flags);
            }
        }

        public long SortAndStore(RedisKey destination, RedisKey key, long skip = 0, long take = -1, Order order = Order.Ascending, SortType sortType = SortType.Numeric, RedisValue by = new(), RedisValue[] get = null, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SortAndStore", key: key))
            {
                return _db.SortAndStore(destination: destination, key: key, skip: skip, take: take, order: order, sortType: sortType, by: by, get: get, flags: flags);
            }
        }

        public bool SortedSetAdd(RedisKey key, RedisValue member, double score, CommandFlags flags)
        {
            using (FsLinkTrack.TrackRedis(method: "SortedSetAdd", key: key))
            {
                return _db.SortedSetAdd(key: key, member: member, score: score, flags: flags);
            }
        }

        public bool SortedSetAdd(RedisKey key, RedisValue member, double score, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SortedSetAdd", key: key))
            {
                return _db.SortedSetAdd(key: key, member: member, score: score, when: when, flags: flags);
            }
        }

        public long SortedSetAdd(RedisKey key, SortedSetEntry[] values, CommandFlags flags)
        {
            using (FsLinkTrack.TrackRedis(method: "SortedSetAdd", key: key))
            {
                return _db.SortedSetAdd(key: key, values: values, flags: flags);
            }
        }

        public long SortedSetAdd(RedisKey key, SortedSetEntry[] values, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SortedSetAdd", key: key))
            {
                return _db.SortedSetAdd(key: key, values: values, when: when, flags: flags);
            }
        }

        public long SortedSetCombineAndStore(SetOperation operation, RedisKey destination, RedisKey first, RedisKey second, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SortedSetCombineAndStore"))
            {
                return _db.SortedSetCombineAndStore(operation: operation, destination: destination, first: first, second: second, aggregate: aggregate, flags: flags);
            }
        }

        public long SortedSetCombineAndStore(SetOperation operation, RedisKey destination, RedisKey[] keys, double[] weights = null, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SortedSetCombineAndStore", key: string.Join(separator: ",", values: keys)))
            {
                return _db.SortedSetCombineAndStore(operation: operation, destination: destination, keys: keys, weights: weights, aggregate: aggregate, flags: flags);
            }
        }

        public double SortedSetDecrement(RedisKey key, RedisValue member, double value, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SortedSetDecrement", key: key))
            {
                return _db.SortedSetDecrement(key: key, member: member, value: value, flags: flags);
            }
        }

        public double SortedSetIncrement(RedisKey key, RedisValue member, double value, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SortedSetIncrement", key: key))
            {
                return _db.SortedSetIncrement(key: key, member: member, value: value, flags: flags);
            }
        }

        public long SortedSetLength(RedisKey key, double min = double.NegativeInfinity, double max = double.PositiveInfinity, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SortedSetLength", key: key))
            {
                return _db.SortedSetLength(key: key, min: min, max: max, exclude: exclude, flags: flags);
            }
        }

        public long SortedSetLengthByValue(RedisKey key, RedisValue min, RedisValue max, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SortedSetLengthByValue", key: key))
            {
                return _db.SortedSetLengthByValue(key: key, min: min, max: max, exclude: exclude, flags: flags);
            }
        }

        public RedisValue[] SortedSetRangeByRank(RedisKey key, long start = 0, long stop = -1, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SortedSetRangeByRank", key: key))
            {
                return _db.SortedSetRangeByRank(key: key, start: start, stop: stop, order: order, flags: flags);
            }
        }

        public SortedSetEntry[] SortedSetRangeByRankWithScores(RedisKey key, long start = 0, long stop = -1, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SortedSetRangeByRankWithScores", key: key))
            {
                return _db.SortedSetRangeByRankWithScores(key: key, start: start, stop: stop, order: order, flags: flags);
            }
        }

        public RedisValue[] SortedSetRangeByScore(RedisKey key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SortedSetRangeByScore", key: key))
            {
                return _db.SortedSetRangeByScore(key: key, start: start, stop: stop, exclude: exclude, order: order, skip: skip, take: take, flags: flags);
            }
        }

        public SortedSetEntry[] SortedSetRangeByScoreWithScores(RedisKey key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SortedSetRangeByScoreWithScores", key: key))
            {
                return _db.SortedSetRangeByScoreWithScores(key: key, start: start, stop: stop, exclude: exclude, order: order, skip: skip, take: take, flags: flags);
            }
        }

        public RedisValue[] SortedSetRangeByValue(RedisKey key, RedisValue min, RedisValue max, Exclude exclude, long skip, long take = -1, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SortedSetRangeByValue", key: key))
            {
                return _db.SortedSetRangeByValue(key: key, min: min, max: max, exclude: exclude, skip: skip, take: take, flags: flags);
            }
        }

        public RedisValue[] SortedSetRangeByValue(RedisKey key, RedisValue min = new(), RedisValue max = new(), Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SortedSetRangeByValue", key: key))
            {
                return _db.SortedSetRangeByValue(key: key, min: min, max: max, exclude: exclude, order: order, skip: skip, take: take, flags: flags);
            }
        }

        public long? SortedSetRank(RedisKey key, RedisValue member, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SortedSetRank", key: key))
            {
                return _db.SortedSetRank(key: key, member: member, order: order, flags: flags);
            }
        }

        public bool SortedSetRemove(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SortedSetRemove", key: key))
            {
                return _db.SortedSetRemove(key: key, member: member, flags: flags);
            }
        }

        public long SortedSetRemove(RedisKey key, RedisValue[] members, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SortedSetRemove", key: key))
            {
                return _db.SortedSetRemove(key: key, members: members, flags: flags);
            }
        }

        public long SortedSetRemoveRangeByRank(RedisKey key, long start, long stop, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SortedSetRemoveRangeByRank", key: key))
            {
                return _db.SortedSetRemoveRangeByRank(key: key, start: start, stop: stop, flags: flags);
            }
        }

        public long SortedSetRemoveRangeByScore(RedisKey key, double start, double stop, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SortedSetRemoveRangeByScore", key: key))
            {
                return _db.SortedSetRemoveRangeByScore(key: key, start: start, stop: stop, exclude: exclude, flags: flags);
            }
        }

        public long SortedSetRemoveRangeByValue(RedisKey key, RedisValue min, RedisValue max, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SortedSetRemoveRangeByValue", key: key))
            {
                return _db.SortedSetRemoveRangeByValue(key: key, min: min, max: max, exclude: exclude, flags: flags);
            }
        }

        public IEnumerable<SortedSetEntry> SortedSetScan(RedisKey key, RedisValue pattern, int pageSize, CommandFlags flags)
        {
            using (FsLinkTrack.TrackRedis(method: "SortedSetScan", key: key))
            {
                return _db.SortedSetScan(key: key, pattern: pattern, pageSize: pageSize, flags: flags);
            }
        }

        public IEnumerable<SortedSetEntry> SortedSetScan(RedisKey key, RedisValue pattern = new(), int pageSize = 250, long cursor = 0, int pageOffset = 0, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SortedSetScan", key: key))
            {
                return _db.SortedSetScan(key: key, pattern: pattern, pageSize: pageSize, cursor: cursor, pageOffset: pageOffset, flags: flags);
            }
        }

        public double? SortedSetScore(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SortedSetScore", key: key))
            {
                return _db.SortedSetScore(key: key, member: member, flags: flags);
            }
        }

        public SortedSetEntry? SortedSetPop(RedisKey key, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SortedSetPop", key: key))
            {
                return _db.SortedSetPop(key: key, order: order, flags: flags);
            }
        }

        public SortedSetEntry[] SortedSetPop(RedisKey key, long count, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "SortedSetPop", key: key))
            {
                return _db.SortedSetPop(key: key, count: count, order: order, flags: flags);
            }
        }

        public long StreamAcknowledge(RedisKey key, RedisValue groupName, RedisValue messageId, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StreamAcknowledge", key: key))
            {
                return _db.StreamAcknowledge(key: key, groupName: groupName, messageId: messageId, flags: flags);
            }
        }

        public long StreamAcknowledge(RedisKey key, RedisValue groupName, RedisValue[] messageIds, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StreamAcknowledge", key: key))
            {
                return _db.StreamAcknowledge(key: key, groupName: groupName, messageIds: messageIds, flags: flags);
            }
        }

        public RedisValue StreamAdd(RedisKey key, RedisValue streamField, RedisValue streamValue, RedisValue? messageId = null, int? maxLength = null, bool useApproximateMaxLength = false, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StreamAdd", key: key))
            {
                return _db.StreamAdd(key: key, streamField: streamField, streamValue: streamValue, messageId: messageId, maxLength: maxLength, useApproximateMaxLength: useApproximateMaxLength, flags: flags);
            }
        }

        public RedisValue StreamAdd(RedisKey key, NameValueEntry[] streamPairs, RedisValue? messageId = null, int? maxLength = null, bool useApproximateMaxLength = false, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StreamAdd", key: key))
            {
                return _db.StreamAdd(key: key, streamPairs: streamPairs, messageId: messageId, maxLength: maxLength, useApproximateMaxLength: useApproximateMaxLength, flags: flags);
            }
        }

        public StreamEntry[] StreamClaim(RedisKey key, RedisValue consumerGroup, RedisValue claimingConsumer, long minIdleTimeInMs, RedisValue[] messageIds, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StreamClaim", key: key))
            {
                return _db.StreamClaim(key: key, consumerGroup: consumerGroup, claimingConsumer: claimingConsumer, minIdleTimeInMs: minIdleTimeInMs, messageIds: messageIds, flags: flags);
            }
        }

        public RedisValue[] StreamClaimIdsOnly(RedisKey key, RedisValue consumerGroup, RedisValue claimingConsumer, long minIdleTimeInMs, RedisValue[] messageIds, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StreamClaimIdsOnly", key: key))
            {
                return _db.StreamClaimIdsOnly(key: key, consumerGroup: consumerGroup, claimingConsumer: claimingConsumer, minIdleTimeInMs: minIdleTimeInMs, messageIds: messageIds, flags: flags);
            }
        }

        public bool StreamConsumerGroupSetPosition(RedisKey key, RedisValue groupName, RedisValue position, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StreamConsumerGroupSetPosition", key: key))
            {
                return _db.StreamConsumerGroupSetPosition(key: key, groupName: groupName, position: position, flags: flags);
            }
        }

        public StreamConsumerInfo[] StreamConsumerInfo(RedisKey key, RedisValue groupName, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StreamConsumerInfo", key: key))
            {
                return _db.StreamConsumerInfo(key: key, groupName: groupName, flags: flags);
            }
        }

        public bool StreamCreateConsumerGroup(RedisKey key, RedisValue groupName, RedisValue? position, CommandFlags flags)
        {
            using (FsLinkTrack.TrackRedis(method: "StreamCreateConsumerGroup", key: key))
            {
                return _db.StreamCreateConsumerGroup(key: key, groupName: groupName, position: position, flags: flags);
            }
        }

        public bool StreamCreateConsumerGroup(RedisKey key, RedisValue groupName, RedisValue? position = null, bool createStream = true, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StreamCreateConsumerGroup", key: key))
            {
                return _db.StreamCreateConsumerGroup(key: key, groupName: groupName, position: position, createStream: createStream, flags: flags);
            }
        }

        public long StreamDelete(RedisKey key, RedisValue[] messageIds, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StreamDelete", key: key))
            {
                return _db.StreamDelete(key: key, messageIds: messageIds, flags: flags);
            }
        }

        public long StreamDeleteConsumer(RedisKey key, RedisValue groupName, RedisValue consumerName, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StreamDeleteConsumer", key: key))
            {
                return _db.StreamDeleteConsumer(key: key, groupName: groupName, consumerName: consumerName, flags: flags);
            }
        }

        public bool StreamDeleteConsumerGroup(RedisKey key, RedisValue groupName, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StreamDeleteConsumerGroup", key: key))
            {
                return _db.StreamDeleteConsumerGroup(key: key, groupName: groupName, flags: flags);
            }
        }

        public StreamGroupInfo[] StreamGroupInfo(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StreamGroupInfo", key: key))
            {
                return _db.StreamGroupInfo(key: key, flags: flags);
            }
        }

        public StreamInfo StreamInfo(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StreamInfo", key: key))
            {
                return _db.StreamInfo(key: key, flags: flags);
            }
        }

        public long StreamLength(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StreamLength", key: key))
            {
                return _db.StreamLength(key: key, flags: flags);
            }
        }

        public StreamPendingInfo StreamPending(RedisKey key, RedisValue groupName, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StreamPending", key: key))
            {
                return _db.StreamPending(key: key, groupName: groupName, flags: flags);
            }
        }

        public StreamPendingMessageInfo[] StreamPendingMessages(RedisKey key, RedisValue groupName, int count, RedisValue consumerName, RedisValue? minId = null, RedisValue? maxId = null, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StreamPendingMessages", key: key))
            {
                return _db.StreamPendingMessages(key: key, groupName: groupName, count: count, consumerName: consumerName, minId: minId, maxId: maxId, flags: flags);
            }
        }

        public StreamEntry[] StreamRange(RedisKey key, RedisValue? minId = null, RedisValue? maxId = null, int? count = null, Order messageOrder = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StreamRange", key: key))
            {
                return _db.StreamRange(key: key, minId: minId, maxId: maxId, count: count, messageOrder: messageOrder, flags: flags);
            }
        }

        public StreamEntry[] StreamRead(RedisKey key, RedisValue position, int? count = null, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StreamRead", key: key))
            {
                return _db.StreamRead(key: key, position: position, count: count, flags: flags);
            }
        }

        public RedisStream[] StreamRead(StreamPosition[] streamPositions, int? countPerStream = null, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StreamRead"))
            {
                return _db.StreamRead(streamPositions: streamPositions, countPerStream: countPerStream, flags: flags);
            }
        }

        public StreamEntry[] StreamReadGroup(RedisKey key, RedisValue groupName, RedisValue consumerName, RedisValue? position, int? count, CommandFlags flags)
        {
            using (FsLinkTrack.TrackRedis(method: "StreamReadGroup", key: key))
            {
                return _db.StreamReadGroup(key: key, groupName: groupName, consumerName: consumerName, position: position, count: count, flags: flags);
            }
        }

        public StreamEntry[] StreamReadGroup(RedisKey key, RedisValue groupName, RedisValue consumerName, RedisValue? position = null, int? count = null, bool noAck = false, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StreamReadGroup", key: key))
            {
                return _db.StreamReadGroup(key: key, groupName: groupName, consumerName: consumerName, position: position, count: count, noAck: noAck, flags: flags);
            }
        }

        public RedisStream[] StreamReadGroup(StreamPosition[] streamPositions, RedisValue groupName, RedisValue consumerName, int? countPerStream, CommandFlags flags)
        {
            using (FsLinkTrack.TrackRedis(method: "StreamReadGroup"))
            {
                return _db.StreamReadGroup(streamPositions: streamPositions, groupName: groupName, consumerName: consumerName, countPerStream: countPerStream, flags: flags);
            }
        }

        public RedisStream[] StreamReadGroup(StreamPosition[] streamPositions, RedisValue groupName, RedisValue consumerName, int? countPerStream = null, bool noAck = false, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StreamReadGroup"))
            {
                return _db.StreamReadGroup(streamPositions: streamPositions, groupName: groupName, consumerName: consumerName, countPerStream: countPerStream, noAck: noAck, flags: flags);
            }
        }

        public long StreamTrim(RedisKey key, int maxLength, bool useApproximateMaxLength = false, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StreamTrim", key: key))
            {
                return _db.StreamTrim(key: key, maxLength: maxLength, useApproximateMaxLength: useApproximateMaxLength, flags: flags);
            }
        }

        public long StringAppend(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StringAppend", key: key))
            {
                return _db.StringAppend(key: key, value: value, flags: flags);
            }
        }

        public long StringBitCount(RedisKey key, long start = 0, long end = -1, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StringBitCount", key: key))
            {
                return _db.StringBitCount(key: key, start: start, end: end, flags: flags);
            }
        }

        public long StringBitOperation(Bitwise operation, RedisKey destination, RedisKey first, RedisKey second = new(), CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StringBitOperation"))
            {
                return _db.StringBitOperation(operation: operation, destination: destination, first: first, second: second, flags: flags);
            }
        }

        public long StringBitOperation(Bitwise operation, RedisKey destination, RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StringBitOperation", key: string.Join(separator: ",", values: keys)))
            {
                return _db.StringBitOperation(operation: operation, destination: destination, keys: keys, flags: flags);
            }
        }

        public long StringBitPosition(RedisKey key, bool bit, long start = 0, long end = -1, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StringBitPosition", key: key))
            {
                return _db.StringBitPosition(key: key, bit: bit, start: start, end: end, flags: flags);
            }
        }

        public long StringDecrement(RedisKey key, long value = 1, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StringDecrement", key: key))
            {
                return _db.StringDecrement(key: key, value: value, flags: flags);
            }
        }

        public double StringDecrement(RedisKey key, double value, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StringDecrement", key: key))
            {
                return _db.StringDecrement(key: key, value: value, flags: flags);
            }
        }

        public RedisValue StringGet(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StringGet", key: key))
            {
                return _db.StringGet(key: key, flags: flags);
            }
        }

        public RedisValue[] StringGet(RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StringGet", key: string.Join(separator: ",", values: keys)))
            {
                return _db.StringGet(keys: keys, flags: flags);
            }
        }

        public Lease<byte> StringGetLease(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StringGetLease", key: key))
            {
                return _db.StringGetLease(key: key, flags: flags);
            }
        }

        public bool StringGetBit(RedisKey key, long offset, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StringGetBit", key: key))
            {
                return _db.StringGetBit(key: key, offset: offset, flags: flags);
            }
        }

        public RedisValue StringGetRange(RedisKey key, long start, long end, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StringGetRange", key: key))
            {
                return _db.StringGetRange(key: key, start: start, end: end, flags: flags);
            }
        }

        public RedisValue StringGetSet(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StringGetSet", key: key))
            {
                return _db.StringGetSet(key: key, value: value, flags: flags);
            }
        }
        public RedisValue StringGetDelete(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StringGetDelete", key: key))
            {
                return _db.StringGetDelete(key: key, flags: flags);
            }
        }

        public RedisValueWithExpiry StringGetWithExpiry(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StringGetWithExpiry", key: key))
            {
                return _db.StringGetWithExpiry(key: key, flags: flags);
            }
        }

        public long StringIncrement(RedisKey key, long value = 1, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StringIncrement", key: key))
            {
                return _db.StringIncrement(key: key, value: value, flags: flags);
            }
        }

        public double StringIncrement(RedisKey key, double value, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StringIncrement", key: key))
            {
                return _db.StringIncrement(key: key, value: value, flags: flags);
            }
        }

        public long StringLength(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StringLength", key: key))
            {
                return _db.StringLength(key: key, flags: flags);
            }
        }

        public bool StringSet(RedisKey key, RedisValue value, TimeSpan? expiry = null, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StringSet", key: key))
            {
                return _db.StringSet(key: key, value: value, expiry: expiry, when: when, flags: flags);
            }
        }

        public bool StringSet(KeyValuePair<RedisKey, RedisValue>[] values, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StringSet"))
            {
                return _db.StringSet(values: values, when: when, flags: flags);
            }
        }

        public bool StringSetBit(RedisKey key, long offset, bool bit, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StringSetBit", key: key))
            {
                return _db.StringSetBit(key: key, offset: offset, bit: bit, flags: flags);
            }
        }

        public RedisValue StringSetRange(RedisKey key, long offset, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "StringSetRange", key: key))
            {
                return _db.StringSetRange(key: key, offset: offset, value: value, flags: flags);
            }
        }

        public bool KeyTouch(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "KeyTouch", key: key))
            {
                return _db.KeyTouch(key: key, flags: flags);
            }
        }

        public long KeyTouch(RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            using (FsLinkTrack.TrackRedis(method: "KeyTouch", key: string.Join(separator: ",", values: keys)))
            {
                return _db.KeyTouch(keys: keys, flags: flags);
            }
        }
    }
}