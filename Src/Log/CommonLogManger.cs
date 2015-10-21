using System;
using System.Collections.Generic;
using System.Threading;
using FS.Utils.Common;

namespace FS.Log
{
    /// <summary>
    ///     通用的日志管理器
    /// </summary>
    /// <typeparam name="TEntity">要保存的实体</typeparam>
    public class CommonLogManger<TEntity>
    {
        private readonly string _filePath;
        private readonly string _fileName;
        private readonly int _lazyTime;

        /// <summary>
        ///     线程锁
        /// </summary>
        public static readonly object LockObject = new object();

        /// <summary> SQL日志保存定时器/// </summary>
        private static readonly Dictionary<Type, Timer> SaveSqlRecord = new Dictionary<Type, Timer>();

        /// <summary> SQL执行记录列表 </summary>
        public static readonly List<TEntity> SqlLogList = new List<TEntity>();

        public CommonLogManger(string filePath, string fileName, int lazyTime)
        {
            _lazyTime = lazyTime;
            _filePath = filePath;
            _fileName = fileName;
        }

        /// <summary>
        ///     指定延迟时间写入数据
        /// </summary>
        public void Write(TEntity entity)
        {
            SqlLogList.Add(entity);
            // 已创建延迟时，退出
            if (SaveSqlRecord.ContainsKey(typeof (TEntity)) && SaveSqlRecord[typeof (TEntity)] != null) { return; }
            lock (LockObject)
            {
                if (SaveSqlRecord.ContainsKey(typeof (TEntity)) && SaveSqlRecord[typeof (TEntity)] != null) { return; }

                // 创建延迟执行
                var timer = new Timer(o =>
                {
                    lock (LockObject)
                    {
                        // 读取本地日志文件
                        var lst = Serialize.Load<List<TEntity>>(_filePath, _fileName, true) ?? new List<TEntity>();
                        var count = SqlLogList.Count;
                        lst.AddRange(SqlLogList);
                        SqlLogList.RemoveRange(0, count);
                        Serialize.Save(lst, _filePath, _fileName);

                        if (SqlLogList.Count == 0 && SaveSqlRecord.ContainsKey(typeof (TEntity)) && SaveSqlRecord[typeof (TEntity)] != null)
                        {
                            SaveSqlRecord[typeof (TEntity)].Dispose();
                            SaveSqlRecord[typeof (TEntity)] = null;
                            SaveSqlRecord.Remove(typeof (TEntity));
                        }
                    }
                }, null, 1000*_lazyTime, 1000*_lazyTime);
                SaveSqlRecord[typeof (TEntity)] = timer;
            }
        }
    }
}