using System;
using System.Collections.Generic;

namespace Yace.Util;

internal static class ExecutorHelpers
{
    public static double InvokeFunction(Stack<double> valueStack, Delegate function, int argCount)
    {
        var argValues = valueStack.PopMany(argCount);
        return FuncUtil.Invoke(function, argValues);
    }
}
