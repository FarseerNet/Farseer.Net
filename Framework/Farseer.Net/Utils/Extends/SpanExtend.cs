using System;
using Collections.Pooled;

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

    /// <summary>
    /// 替换char字符
    /// </summary>
    public static ReadOnlySpan<char> Replace(this ReadOnlySpan<char> span, string oldValue, string newValue)
    {
        var oldValSpan    = oldValue.AsSpan();
        var newValSpan    = newValue.AsSpan();
        var oldValueIndex = span.IndexOf(oldValSpan);

        while (oldValueIndex > -1)
        {
            var headSpan = span.Slice(0, oldValueIndex);
            var tailSpan = span.Slice(oldValueIndex + oldValue.Length);
            span          = Concat(headSpan, newValSpan, tailSpan);
            oldValueIndex = span.IndexOf(oldValSpan);
        }
        return span;
    }

    public static PooledList<string> Split(this ReadOnlySpan<char> source, string separator)
    {
        var lst           = new PooledList<string>();
        var       spanSeparator = separator.AsSpan();
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
        Span<T> span = new T[first.Length + second.Length]; //stackalloc
        first.CopyTo(span);
        for (var i = 0; i < second.Length; i++)
        {
            span[i + first.Length] = second[i];
        }
        return span;
    }

    public static ReadOnlySpan<T> Concat<T>(this ReadOnlySpan<T> first, ReadOnlySpan<T> second, ReadOnlySpan<T> third)
    {
        Span<T> span = new T[first.Length + second.Length + third.Length]; //stackalloc
        first.CopyTo(span);
        // 合并第二个
        for (var i = 0; i < second.Length; i++)
        {
            span[i + first.Length] = second[i];
        }

        // 合并第三个
        for (var i = 0; i < third.Length; i++)
        {
            span[i + first.Length + second.Length] = third[i];
        }
        return span;
    }
}