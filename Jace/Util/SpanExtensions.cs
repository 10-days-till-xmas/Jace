using System;

namespace Jace.Util;

internal static class SpanExtensions
{
#if NET5_0_OR_GREATER
    public static T Dequeue<T>(this ReadOnlySpan<T> span, ref int index)
    {
        return span[index++];
    }

    public static bool TryDequeue<T>(this ReadOnlySpan<T> span, ref int index, out T value)
    {
        if (index < span.Length)
        {
            value = span[index];
            index++;
            return true;
        }

        value = default!;
        return false;
    }

    public static bool DequeueIf<T>(this ReadOnlySpan<T> array, ref int index, out T value, Func<T, bool> predicate)
    {
        if (index < array.Length && predicate(array[index]))
        {
            value = array[index];
            index++;
            return true;
        }

        value = default!;
        return false;
    }

    public static T Peek<T>(this ReadOnlySpan<T> span, int index)
    {
        return span[index];
    }
    
    public static bool TryPeek<T>(this ReadOnlySpan<T> span, int index, out T value)
    {
        if (index < span.Length)
        {
            value = span[index];
            return true;
        }

        value = default!;
        return false;
    }
#endif
}