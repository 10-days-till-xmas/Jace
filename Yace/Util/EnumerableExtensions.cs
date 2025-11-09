using System;
using System.Collections.Generic;

namespace Yace.Util;

internal static class EnumerableExtensions
{
    /// <summary>
    /// Pops multiple items from the stack. The items are returned in the order they were popped.
    /// </summary>
    /// <returns>The popped items</returns>
    public static T[] PopMany<T>(this Stack<T> stack, int count)
    {
        var arr = new T[count];
        for (var i = count - 1; i >= 0; i--)
            arr[i] = stack.Pop();
        return arr;
    }
    #if !NETSTANDARD
    /// <summary>
    /// Pops multiple items from the stack. The items are returned in the order they were popped.
    /// </summary>
    /// <returns>The popped items</returns>
    public static ReadOnlySpan<T> PopManyToSpan<T>(this Stack<T> stack, int count)
    {
        var arr = new T[count];
        for (var i = count - 1; i >= 0; i--)
            arr[i] = stack.Pop();
        return arr;
    }
    #endif
}
