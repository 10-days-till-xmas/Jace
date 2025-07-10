using System;
using System.Linq;
using System.Reflection;

namespace Jace.Util;

public static class DelegateExtensions
{
    /// <summary>
    /// Throws an exception if the function type is not valid.
    /// </summary>
    /// <remarks>
    /// For Jace, only functions of type <see cref="System.Func{T,T}"/> or <see cref="Jace.DynamicFunc{T, T}"/>
    /// where <c>T</c> and <c>TResult</c> are <see cref="double"/> are supported.
    /// </remarks>
    /// <param name="function"></param>
    /// <exception cref="ArgumentException">Thrown if the function is invalid</exception>
    internal static void AssertIsValidFunctionType(this Delegate function)
    {
        const string SystemFuncPrefix = $"{nameof(System)}.{nameof(Func<double>)}";
        const string DynamicFuncName = $"{nameof(Jace)}.{nameof(DynamicFunc<double, double>)}";
        if (function is DynamicFunc<double, double>)
            return;
        var functionType = function.GetType();
        
        if (functionType.GenericTypeArguments.Any(arg => arg != typeof(double))) 
            throw new ArgumentException("Only doubles are supported as function arguments.", nameof(function));
        
        var funcTypeName = functionType.FullName;
        
        if (!funcTypeName!.StartsWith(SystemFuncPrefix))
        {
            throw new ArgumentException(
                $"Only {SystemFuncPrefix} and {DynamicFuncName} delegates are permitted.", nameof(function));
        }
    }

    public static double FastInvoke(this Delegate function, double[] arguments)
    {
        // DynamicInvoke is slow, so first try to convert it to a Func
        return function switch
        {
            Func<double> Func0 
                => Func0(),
            Func<double, double> Func1 
                => Func1(arguments[0]),
            Func<double, double, double> Func2 
                => Func2(arguments[0], arguments[1]),
            Func<double, double, double, double> Func3 
                => Func3(arguments[0], arguments[1], arguments[2]),
            Func<double, double, double, double, double> Func4 
                => Func4(arguments[0], arguments[1], arguments[2], arguments[3]),
            Func<double, double, double, double, double, double> Func5 
                => Func5(arguments[0], arguments[1], arguments[2], arguments[3], arguments[4]),
            Func<double, double, double, double, double, double, double> Func6 
                => Func6(arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], arguments[5]),
            Func<double, double, double, double, double, double, double, double> Func7 
                => Func7(arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], arguments[5], arguments[6]),
            Func<double, double, double, double, double, double, double, double, double> Func8 
                => Func8(arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], arguments[5], arguments[6], arguments[7]),
            Func<double, double, double, double, double, double, double, double, double, double> Func9 
                => Func9(arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], arguments[5], arguments[6], arguments[7], arguments[8]),
            Func<double, double, double, double, double, double, double, double, double, double, double> Func10 
                => Func10(arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], arguments[5], arguments[6], arguments[7], arguments[8], arguments[9]),
            Func<double, double, double, double, double, double, double, double, double, double, double, double> Func11 
                => Func11(arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], arguments[5], arguments[6], arguments[7], arguments[8], arguments[9], arguments[10]),
            Func<double, double, double, double, double, double, double, double, double, double, double, double, double> Func12 
                => Func12(arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], arguments[5], arguments[6], arguments[7], arguments[8], arguments[9], arguments[10], arguments[11]),
            Func<double, double, double, double, double, double, double, double, double, double, double, double, double, double> Func13 
                => Func13(arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], arguments[5], arguments[6], arguments[7], arguments[8], arguments[9], arguments[10], arguments[11], arguments[12]),
            Func<double, double, double, double, double, double, double, double, double, double, double, double, double, double, double> Func14 
                => Func14(arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], arguments[5], arguments[6], arguments[7], arguments[8], arguments[9], arguments[10], arguments[11], arguments[12], arguments[13]),
            Func<double, double, double, double, double, double, double, double, double, double, double, double, double, double, double, double> Func15 
                => Func15(arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], arguments[5], arguments[6], arguments[7], arguments[8], arguments[9], arguments[10], arguments[11], arguments[12], arguments[13], arguments[14]),
            Func<double, double, double, double, double, double, double, double, double, double, double, double, double, double, double, double, double> Func16 
                => Func16(arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], arguments[5], arguments[6], arguments[7], arguments[8], arguments[9], arguments[10], arguments[11], arguments[12], arguments[13], arguments[14], arguments[15]),
            DynamicFunc<double, double> dynamicFunc 
                => dynamicFunc(arguments),
            _ => (double)function.DynamicInvoke(arguments.Cast<object>().ToArray())!
        };
    }
    
    // ReSharper disable once RedundantSuppressNullableWarningExpression
    
    // The lower func reside in mscorlib, the higher ones in another assembly.
    // This is an easy cross-platform way to have this AssemblyQualifiedName.
    private static readonly string FuncAssemblyQualifiedName = typeof(Func<double, double, double, double, double, 
            double, double, double, double, double>)
        .GetTypeInfo().Assembly.FullName!;

    public static Type GetFuncType(int numberOfParameters)
    {
        var funcTypeName = numberOfParameters < 9 
            ? $"System.Func`{numberOfParameters + 1}" 
            : $"System.Func`{numberOfParameters + 1}, {FuncAssemblyQualifiedName}";
        var funcType = Type.GetType(funcTypeName) 
                       ?? throw new InvalidOperationException($"Could not find type {funcTypeName}.");

        var typeArguments = new Type[numberOfParameters + 1];
        #if NET5_0_OR_GREATER
        Array.Fill(typeArguments, typeof(double)); // slightly more efficient
        #else
        for (var i = 0; i < numberOfParameters + 1; i++)
            typeArguments[i] = typeof(double);
        #endif
            
        return funcType.MakeGenericType(typeArguments);
    }
}