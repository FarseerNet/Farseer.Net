using System;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace FS.Extends;

public static class IListExtend
{
    /// <summary>
    /// IList不支持RemoveAll，这里简单实时RemoveAll
    /// </summary>
    public static int RemoveAll<TEntity>(this IList<TEntity> lst, Predicate<TEntity> match)
    {
        var count = 0;
        for (int index = 0; index < lst.Count; index++)
        {
            // 判断是否匹配
            if (!match(lst[index])) continue;
            
            lst.RemoveAt(index);
            count++;
            index--;
        }
        return count;
    }
}