using System;

namespace FS.Core.Abstract.Data
{
    public interface IDbContext
    {
        /// <summary>
        ///     当事务提交后，会调用该委托
        /// </summary>
        void AddCallback(Action act);
    }
}