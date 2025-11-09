using System;

namespace Yace.Util;

internal static class FuncUtil
{
    internal static double Invoke(Delegate function, double[] args)
    {
        return function switch
        {
            // DynamicInvoke is slow, so we first try to convert it to a Func
            Func<double> func0
                => func0.Invoke(),
            Func<double, double> func1
                => func1.Invoke(args[0]),
            Func<double, double, double> func2
                => func2.Invoke(args[0], args[1]),
            Func<double, double, double, double> func3
                => func3.Invoke(args[0], args[1], args[2]),
            Func<double, double, double, double, double> func4
                => func4.Invoke(args[0], args[1], args[2], args[3]),
            Func<double, double, double, double, double, double> func5
                => func5.Invoke(args[0], args[1], args[2], args[3], args[4]),
            Func<double, double, double, double, double, double, double> func6
                => func6.Invoke(args[0], args[1], args[2], args[3], args[4], args[5]),
            Func<double, double, double, double, double, double, double, double> func7
                => func7.Invoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6]),
            Func<double, double, double, double, double, double, double, double, double> func8 =>
                func8.Invoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7]),
            Func<double, double, double, double, double, double, double, double, double, double> func9 =>
                func9.Invoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8]),
            Func<double, double, double, double, double, double, double, double, double, double, double> func10 =>
                func10.Invoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9]),
            Func<double, double, double, double, double, double, double, double, double, double, double, double> func11
                => func11.Invoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10]),
            Func<double, double, double, double, double, double, double, double, double, double, double, double, double> func12
                => func12.Invoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10], args[11]),
            Func<double, double, double, double, double, double, double, double, double, double, double, double, double, double> func13
                => func13.Invoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10], args[11], args[12]),
            Func<double, double, double, double, double, double, double, double, double, double, double, double, double, double, double> func14
                => func14.Invoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10], args[11], args[12], args[13]),
            Func<double, double, double, double, double, double, double, double, double, double, double, double, double, double, double, double> func15
                => func15.Invoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10], args[11], args[12], args[13], args[14]),
            Func<double, double, double, double, double, double, double, double, double, double, double, double, double, double, double, double, double> func16
                => func16.Invoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10], args[11], args[12], args[13], args[14], args[15]),
            DynamicFunc<double, double> dynamicFunc => dynamicFunc.Invoke(args),
            _ => (double)function.DynamicInvoke(args)!
        };
    }
    #if !NETSTANDARD2_0
    internal static double Invoke(Delegate function, ReadOnlySpan<double> args)
    {
        return function switch
        {
            // DynamicInvoke is slow, so we first try to convert it to a Func
            Func<double> func0
                => func0.Invoke(),
            Func<double, double> func1
                => func1.Invoke(args[0]),
            Func<double, double, double> func2
                => func2.Invoke(args[0], args[1]),
            Func<double, double, double, double> func3
                => func3.Invoke(args[0], args[1], args[2]),
            Func<double, double, double, double, double> func4
                => func4.Invoke(args[0], args[1], args[2], args[3]),
            Func<double, double, double, double, double, double> func5
                => func5.Invoke(args[0], args[1], args[2], args[3], args[4]),
            Func<double, double, double, double, double, double, double> func6
                => func6.Invoke(args[0], args[1], args[2], args[3], args[4], args[5]),
            Func<double, double, double, double, double, double, double, double> func7
                => func7.Invoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6]),
            Func<double, double, double, double, double, double, double, double, double> func8 =>
                func8.Invoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7]),
            Func<double, double, double, double, double, double, double, double, double, double> func9 =>
                func9.Invoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8]),
            Func<double, double, double, double, double, double, double, double, double, double, double> func10 =>
                func10.Invoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9]),
            Func<double, double, double, double, double, double, double, double, double, double, double, double> func11
                => func11.Invoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10]),
            Func<double, double, double, double, double, double, double, double, double, double, double, double, double> func12
                => func12.Invoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10], args[11]),
            Func<double, double, double, double, double, double, double, double, double, double, double, double, double, double> func13
                => func13.Invoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10], args[11], args[12]),
            Func<double, double, double, double, double, double, double, double, double, double, double, double, double, double, double> func14
                => func14.Invoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10], args[11], args[12], args[13]),
            Func<double, double, double, double, double, double, double, double, double, double, double, double, double, double, double, double> func15
                => func15.Invoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10], args[11], args[12], args[13], args[14]),
            Func<double, double, double, double, double, double, double, double, double, double, double, double, double, double, double, double, double> func16
                => func16.Invoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10], args[11], args[12], args[13], args[14], args[15]),
            DynamicFunc<double, double> dynamicFunc => dynamicFunc.Invoke(args.ToArray()),
            _ => (double)function.DynamicInvoke(args.ToArray())!
        };
    }
    #endif
}
