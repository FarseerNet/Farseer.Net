using System.Collections;

namespace FS.Core
{
    public interface IPageList
    {
        /// <summary>
        ///     总页数
        /// </summary>
        long RecordCount { get; set; }

        IEnumerable GetList();
    }
}