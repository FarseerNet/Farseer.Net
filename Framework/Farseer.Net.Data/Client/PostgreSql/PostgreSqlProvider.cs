﻿using System;
using System.Data.Common;
using System.Reflection;
using System.Text;
using FS.Cache;
using FS.Data.Internal;
using FS.Data.Map;

namespace FS.Data.Client.PostgreSql
{
    /// <summary>
    ///     MySql 数据库提供者（不同数据库的特性）
    /// </summary>
    public class PostgreSqlProvider : AbsDbProvider
    {
        private static readonly FieldInfo           DbProviderFactoryType = Assembly.Load(assemblyString: "Npgsql").GetType(name: "Npgsql.NpgsqlFactory").GetField(name: "Instance");
        public override         DbProviderFactory   DbProviderFactory    => (DbProviderFactory)StaticFieldGetCacheManger.Cache(key: DbProviderFactoryType);
        public override         AbsFunctionProvider FunctionProvider     => new PostgreSqlFunctionProvider();
        public override         AbsDbParam          DbParam              => new PostgreSqlParam(DbProviderFactory);
        public override         AbsConnectionString ConnectionString     => new PostgreSqlConnectionString();
        public override         bool                IsSupportTransaction => true;

        public override string KeywordAegis(string fieldName) =>
        //if (Regex.IsMatch(fieldName, "[\\(\\)\\,\\[\\]\\+\\= ]+")) { return fieldName; }
        $"\"{fieldName}\"";

        internal override AbsSqlBuilder CreateSqlBuilder(ExpressionBuilder expBuilder, SetDataMap setMap) => new PostgreSqlBuilder(dbProvider: this, expBuilder: expBuilder, setMap);
    }
}