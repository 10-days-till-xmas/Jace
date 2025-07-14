using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;


namespace Jace.Util;

internal static class CollectionExtensions
{
    public static T[] Slice<T>(this T[] source, int start, int length)
    {
        var result = new T[length];
        Array.Copy(source, start, result, 0, length);
        return result;
    }
    
    public static T Dequeue<T>(this T[] array, ref int index)
    {
        return array[index++];
    }
    
    public static bool TryDequeue<T>(this T[] array, ref int index, out T value)
    {
        if (index < array.Length)
        {
            value = array[index];
            index++;
            return true;
        }

        value = default!;
        return false;
    }
    
    public static bool DequeueIf<T>(this T[] array, ref int index, out T value, Func<T, bool> predicate)
    {
        if (!array.TryPeek(index, out value)) return false;
        if (!predicate(value)) return false;
        index++;
        return true;
    }
    
    [Pure]
    public static T Peek<T>(this T[] array, int index)
    {
        return array[index];
    }
    
    public static bool TryPeek<T>(this T[] array, int index, out T value)
    {
        if (index < array.Length)
        {
            value = array[index];
            return true;
        }

        value = default!;
        return false;
    }
}