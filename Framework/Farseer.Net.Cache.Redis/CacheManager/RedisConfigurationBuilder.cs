using System;
using System.Collections.Generic;
using CacheManager.Core.Utility;

#pragma warning disable CS3021 // 由于程序集没有 CLSCompliant 特性，因此类型或成员不需要 CLSCompliant 特性
namespace FS.Cache.Manager.Redis
{
    /// <summary>
    /// <see cref="RedisConfigurationBuilder"/> helps creating the <see cref="RedisConfiguration"/>
    /// object via code.
    /// </summary>
    public class RedisConfigurationBuilder
    {
        private bool _allowAdmin = false;
        private int _connectionTimeout = 5000;
        private int _database = 0;
        private IList<ServerEndPoint> _endpoints = new List<ServerEndPoint>();
        private bool _isSsl = false;
        private string _key = string.Empty;
        private string _password = null;
        private string _sslHost = null;
        private bool _enabledKeyspaceNotifications = false;
        private string _useVersion;
        private bool _useTwemproxy;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisConfigurationBuilder"/> class.
        /// </summary>
        /// <param name="configurationKey">The configuration key.</param>
        /// <exception cref="System.ArgumentNullException">If configurationKey is null.</exception>
        public RedisConfigurationBuilder(string configurationKey)
        {
            Guard.NotNullOrWhiteSpace(configurationKey, nameof(configurationKey));

            _key = configurationKey;
        }

        /// <summary>
        /// Creates the <see cref="RedisConfiguration"/> out of the currently specified properties,
        /// if possible.
        /// </summary>
        /// <returns>The <c>RedisConfiguration</c></returns>
        public RedisConfiguration Build() =>
            new RedisConfiguration(
                key: _key,
                endpoints: _endpoints,
                database: _database,
                password: _password,
                isSsl: _isSsl,
                sslHost: _sslHost,
                connectionTimeout: _connectionTimeout,
                allowAdmin: _allowAdmin,
                keyspaceNotificationsEnabled: _enabledKeyspaceNotifications,
                twemproxyEnabled: _useTwemproxy,
                strictCompatibilityModeVersion: _useVersion);

        /// <summary>
        /// Enable the flag to have CacheManager react on keyspace notifications from redis.
        /// CacheManager will listen only for eviction and expiration events (not all events).
        /// Use this feature only if you also have configured Redis correctly: notify-keyspace-events must be set to AT LEAST Exe.
        /// <see href="https://redis.io/topics/notifications#configuration"/>
        /// </summary>
        /// <returns>The builder.</returns>
        public RedisConfigurationBuilder EnableKeyspaceEvents()
        {
            _enabledKeyspaceNotifications = true;
            return this;
        }

        /// <summary>
        /// Can be used to control the available Redis features CacheManager can use. E.g. if set to <c>"2.4"</c>, this would disable all LUA support and would
        /// force CacheManager to use other APIs
        /// </summary>
        /// <param name="version"></param>
        /// <returns>The builder.</returns>
        public RedisConfigurationBuilder UseCompatibilityMode(string version)
        {
            _useVersion = version;
            return this;
        }

        /// <summary>
        /// Enable this in case you are using Redis behind Twemproxy.
        /// </summary>
        /// <returns>The builder.</returns>
        public RedisConfigurationBuilder UseTwemproxy()
        {
            _useTwemproxy = true;
            return this;
        }

        /// <summary>
        /// If set to true, commands which might be risky are enabled, like Clear which will delete
        /// all entries in the redis database.
        /// </summary>
        /// <returns>The builder.</returns>
        public RedisConfigurationBuilder WithAllowAdmin()
        {
            _allowAdmin = true;
            return this;
        }

        /// <summary>
        /// Sets the timeout in milliseconds for connect operations.
        /// </summary>
        /// <param name="timeout">The timeout in milliseconds.</param>
        /// <returns>The builder.</returns>
        public RedisConfigurationBuilder WithConnectionTimeout(int timeout)
        {
            _connectionTimeout = timeout;
            return this;
        }

        /// <summary>
        /// Sets the database.
        /// <para>Maximum number of database depends on the redis server configuration.</para>Default
        /// is <c>0</c>.
        /// </summary>
        /// <param name="databaseIndex">The database index.</param>
        /// <returns>The builder.</returns>
        public RedisConfigurationBuilder WithDatabase(int databaseIndex)
        {
            _database = databaseIndex;
            return this;
        }

        /// <summary>
        /// Adds an endpoint to the connection configuration.
        /// <para>Call this multiple times to add multiple endpoints.</para>
        /// </summary>
        /// <param name="host">The host or IP of the redis server.</param>
        /// <param name="port">The port of the redis server.</param>
        /// <returns>The builder.</returns>
        public RedisConfigurationBuilder WithEndpoint(string host, int port)
        {
            var endpoint = new ServerEndPoint(host, port);
            _endpoints.Add(endpoint);
            return this;
        }

        /// <summary>
        /// Sets the password for the redis server.
        /// </summary>
        /// <param name="serverPassword">The redis server password.</param>
        /// <returns>The builder.</returns>
        public RedisConfigurationBuilder WithPassword(string serverPassword)
        {
            _password = serverPassword;
            return this;
        }

        /// <summary>
        /// Enables SSL encryption.
        /// <para>
        /// If host is specified it will enforce a particular SSL host identity on the server's certificate.
        /// </para>
        /// </summary>
        /// <param name="host">The SSL host.</param>
        /// <returns>The builder.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Using it for configuration data only.")]
        public RedisConfigurationBuilder WithSsl(string host = null)
        {
            _isSsl = true;
            _sslHost = host;
            return this;
        }
    }
}