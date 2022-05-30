using System;
using System.Collections.Generic;

namespace FS.Extends;

public static class SpanExtend
{
    /// <summary>
    /// 替换char字符
    /// </summary>
    public static ReadOnlySpan<char> Replace(this ReadOnlySpan<char> span, char oldValue, char newValue)
    {
        var arr = span.ToArray();
        for (var index = 0; index < arr.Length; index++)
        {
            if (arr[index] == oldValue) arr[index] = newValue;
        }
        return arr;
    }

    public static List<string> Split(this ReadOnlySpan<char> source, string separator)
    {
        var lst           = new List<string>();
        var spanSeparator = separator.AsSpan();
        while (true)
        {
            var index = source.IndexOf(spanSeparator);
            if (index > -1)
            {
                lst.Add(source.Slice(0, index).ToString());
                source = source.Slice(index + spanSeparator.Length);
            }
            else
            {
                lst.Add(source.ToString());
                break;
            }
        }
        return lst;
    }

    public static ReadOnlySpan<T> Concat<T>(this ReadOnlySpan<T> first, ReadOnlySpan<T> second)
    {
        Span<T> span = new T[first.Length + second.Length];
        first.CopyTo(span);
        for (var i = 0; i < second.Length; i++)
        {
            span[i + first.Length] = second[i];
        }
        return span;
    }
}