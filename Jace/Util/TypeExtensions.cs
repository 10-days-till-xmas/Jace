using System;
using System.Linq;

namespace Jace.Util;

public static class TypeExtensions
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
}