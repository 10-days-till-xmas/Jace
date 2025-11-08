using System;

namespace Jace;

public sealed partial class CalculationEngine
{
    /// <summary>
    /// Add a function to the calculation engine.
    /// </summary>
    /// <param name="functionName">The name of the function. This name can be used in mathematical formulas.</param>
    /// <param name="function">The implementation of the function.</param>
    /// <param name="isIdempotent">Whether the function provides the same result when it is executed multiple times.</param>
    public void AddFunction(string functionName, Func<double> function, bool isIdempotent = true)
    {
        FunctionRegistry.RegisterFunction(functionName, function, isIdempotent, true);
    }

    /// <inheritdoc cref="CalculationEngine.AddFunction(string, Func{double}, bool)"/>
    public void AddFunction(string functionName, Func<double, double> function, bool isIdempotent = true)
    {
        FunctionRegistry.RegisterFunction(functionName, function, isIdempotent, true);
    }

    /// <inheritdoc cref="CalculationEngine.AddFunction(string, Func{double}, bool)"/>
    public void AddFunction(string functionName, Func<double, double, double> function, bool isIdempotent = true)
    {
        FunctionRegistry.RegisterFunction(functionName, function, isIdempotent, true);
    }

    /// <inheritdoc cref="CalculationEngine.AddFunction(string, Func{double}, bool)"/>
    public void AddFunction(string functionName, Func<double, double, double, double> function,
        bool isIdempotent = true)
    {
        FunctionRegistry.RegisterFunction(functionName, function, isIdempotent, true);
    }

    /// <inheritdoc cref="CalculationEngine.AddFunction(string, Func{double}, bool)"/>
    public void AddFunction(string functionName, Func<double, double, double, double, double> function,
        bool isIdempotent = true)
    {
        FunctionRegistry.RegisterFunction(functionName, function, isIdempotent, true);
    }

    /// <inheritdoc cref="CalculationEngine.AddFunction(string, Func{double}, bool)"/>
    public void AddFunction(string functionName, Func<double, double, double, double, double, double> function,
        bool isIdempotent = true)
    {
        FunctionRegistry.RegisterFunction(functionName, function, isIdempotent, true);
    }

    /// <inheritdoc cref="CalculationEngine.AddFunction(string, Func{double}, bool)"/>
    public void AddFunction(string functionName, Func<double, double, double, double, double, double, double> function,
        bool isIdempotent = true)
    {
        FunctionRegistry.RegisterFunction(functionName, function, isIdempotent, true);
    }
    
    /// <inheritdoc cref="CalculationEngine.AddFunction(string, Func{double}, bool)"/>
    public void AddFunction(string functionName,
        Func<double, double, double, double, double, double, double, double> function, bool isIdempotent = true)
    {
        FunctionRegistry.RegisterFunction(functionName, function, isIdempotent, true);
    }

    /// <inheritdoc cref="CalculationEngine.AddFunction(string, Func{double}, bool)"/>
    public void AddFunction(string functionName,
        Func<double, double, double, double, double, double, double, double, double> function, bool isIdempotent = true)
    {
        FunctionRegistry.RegisterFunction(functionName, function, isIdempotent, true);
    }

    /// <inheritdoc cref="CalculationEngine.AddFunction(string, Func{double}, bool)"/>
    public void AddFunction(string functionName,
        Func<double, double, double, double, double, double, double, double, double, double> function,
        bool isIdempotent = true)
    {
        FunctionRegistry.RegisterFunction(functionName, function, isIdempotent, true);
    }

    /// <inheritdoc cref="CalculationEngine.AddFunction(string, Func{double}, bool)"/>
    public void AddFunction(string functionName,
        Func<double, double, double, double, double, double, double, double, double, double, double> function,
        bool isIdempotent = true)
    {
        FunctionRegistry.RegisterFunction(functionName, function, isIdempotent, true);
    }

    /// <inheritdoc cref="CalculationEngine.AddFunction(string, Func{double}, bool)"/>
    public void AddFunction(string functionName,
        Func<double, double, double, double, double, double, double, double, double, double, double, double> function,
        bool isIdempotent = true)
    {
        FunctionRegistry.RegisterFunction(functionName, function, isIdempotent, true);
    }

    /// <inheritdoc cref="CalculationEngine.AddFunction(string, Func{double}, bool)"/>
    public void AddFunction(string functionName,
        Func<double, double, double, double, double, double, double, double, double, double, double, double, double>
            function, bool isIdempotent = true)
    {
        FunctionRegistry.RegisterFunction(functionName, function, isIdempotent, true);
    }

    /// <inheritdoc cref="CalculationEngine.AddFunction(string, Func{double}, bool)"/>
    public void AddFunction(string functionName,
        Func<double, double, double, double, double, double, double, double, double, double, double, double, double,
            double> function, bool isIdempotent = true)
    {
        FunctionRegistry.RegisterFunction(functionName, function, isIdempotent, true);
    }

    /// <inheritdoc cref="CalculationEngine.AddFunction(string, Func{double}, bool)"/>
    public void AddFunction(string functionName,
        Func<double, double, double, double, double, double, double, double, double, double, double, double, double,
            double, double> function, bool isIdempotent = true)
    {
        FunctionRegistry.RegisterFunction(functionName, function, isIdempotent, true);
    }

    /// <inheritdoc cref="CalculationEngine.AddFunction(string, Func{double}, bool)"/>
    public void AddFunction(string functionName,
        Func<double, double, double, double, double, double, double, double, double, double, double, double, double,
            double, double, double> function, bool isIdempotent = true)
    {
        FunctionRegistry.RegisterFunction(functionName, function, isIdempotent, true);
    }

    /// <inheritdoc cref="CalculationEngine.AddFunction(string, Func{double}, bool)"/>
    public void AddFunction(string functionName,
        Func<double, double, double, double, double, double, double, double, double, double, double, double, double,
            double, double, double, double> function, bool isIdempotent = true)
    {
        FunctionRegistry.RegisterFunction(functionName, function, isIdempotent, true);
    }

    /// <inheritdoc cref="CalculationEngine.AddFunction(string, Func{double}, bool)"/>
    public void AddFunction(string functionName, DynamicFunc<double, double> functionDelegate, bool isIdempotent = true)
    {
        FunctionRegistry.RegisterFunction(functionName, functionDelegate, isIdempotent, true);
    }
}