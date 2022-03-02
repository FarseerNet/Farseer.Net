using System;
using System.Data;

namespace FS.Core.Data
{
    public interface IDbContext
    {
        /// <summary>
        ///     当事务提交后，会调用该委托
        /// </summary>
        void AddCallback(Action act);
        /// <summary>
        ///     保存修改
        /// </summary>
        void SaveChanges();
        /// <summary>
        ///     回滚事务
        /// </summary>
        void Rollback();
        /// <summary>
        ///     改变事务级别
        /// </summary>
        /// <param name="tranLevel"> 事务方式 </param>
        void ChangeTransaction(IsolationLevel tranLevel);
    }
}