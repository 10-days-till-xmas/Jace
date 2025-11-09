using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using JetBrains.Annotations;
using Yace.Execution;
using Yace.Operations;
using Yace.Tokenizer;
using Yace.Util;

namespace Yace;

public delegate TResult DynamicFunc<in T, out TResult>(params T[] values);

/// <summary>
/// The CalculationEngine class is the main class of Yace.NET to convert strings containing
/// mathematical formulas into .NET Delegates and to calculate the result.
/// It can be configured to run in a number of modes based on the constructor parameters chosen.
/// </summary>
[PublicAPI]
public sealed partial class CalculationEngine : IUsesText
{
    private readonly IExecutor executor;
    private readonly Optimizer optimizer;
    private readonly CultureInfo cultureInfo;
    private readonly MemoryCache<string, Func<IDictionary<string, double>, double>> executionFormulaCache;
    private readonly bool cacheEnabled;
    private readonly bool optimizerEnabled;
    public bool CaseSensitive { get; }

    private static readonly Random random = new();

    /// <summary>
    /// Creates a new instance of the <see cref="CalculationEngine"/> class with
    /// default parameters.
    /// </summary>
    public CalculationEngine()
        : this(new YaceOptions())
    {
    }

    /// <summary>
    /// Creates a new instance of the <see cref="CalculationEngine"/> class.
    /// The dynamic compiler is used for formula execution, and the optimizer and cache are enabled.
    /// </summary>
    /// <param name="cultureInfo">
    /// The <see cref="CultureInfo"/> required for correctly reading floating point numbers.
    /// </param>
    public CalculationEngine(CultureInfo cultureInfo)
        : this(new YaceOptions { CultureInfo = cultureInfo })
    {
    }

    /// <summary>
    /// Creates a new instance of the <see cref="CalculationEngine"/> class. The optimizer and
    /// cache are enabled.
    /// </summary>
    /// <param name="cultureInfo">
    /// The <see cref="CultureInfo"/> required for correctly reading floating point numbers.
    /// </param>
    /// <param name="executionMode">The execution mode that must be used for formula execution.</param>
    public CalculationEngine(CultureInfo cultureInfo, ExecutionMode executionMode)
        : this (new YaceOptions { CultureInfo = cultureInfo, ExecutionMode = executionMode })
    {
    }

    /// <summary>
    /// Creates a new instance of the <see cref="CalculationEngine"/> class.
    /// </summary>
    /// <param name="options">The <see cref="YaceOptions"/> to configure the behaviour of the engine.</param>
    public CalculationEngine(YaceOptions options)
    {
        executionFormulaCache = new MemoryCache<string, Func<IDictionary<string, double>, double>>(options.CacheMaximumSize, options.CacheReductionSize);

        cultureInfo = options.CultureInfo;
        cacheEnabled = options.CacheEnabled;
        optimizerEnabled = options.OptimizerEnabled;
        CaseSensitive = options.CaseSensitive;
        FunctionRegistry = new FunctionRegistry(CaseSensitive);
        ConstantRegistry = new ConstantRegistry(CaseSensitive);

        executor = options.ExecutionMode switch
        {
            ExecutionMode.Interpreted => new Interpreter(CaseSensitive),
            ExecutionMode.Compiled    => new DynamicCompiler(CaseSensitive),
            _ => throw new ArgumentException($"Unsupported execution mode \"{options.ExecutionMode}\".",
                                             nameof(options))
        };

        optimizer = new Optimizer(new Interpreter()); // We run the optimizer with the interpreter
        // why the interpreter? Maybe write optimizers for each executor?

        if (options.DefaultConstants)
            RegisterDefaultConstants(ConstantRegistry);
        if (options.DefaultFunctions)
            RegisterDefaultFunctions(FunctionRegistry);
    }

    internal IFunctionRegistry FunctionRegistry { get; }

    internal IConstantRegistry ConstantRegistry { get; }

    public IEnumerable<FunctionInfo> Functions => FunctionRegistry;

    public IEnumerable<ConstantInfo> Constants => ConstantRegistry;

    public double Calculate(string formulaText, IDictionary<string, double>? variables = null)
    {
        if (string.IsNullOrWhiteSpace(formulaText))
            throw new ArgumentNullException(nameof(formulaText));
        switch ((variables, CaseSensitive))
        {
            case (null, true):
                variables = new Dictionary<string, double>();
                break;
            case (null, false):
                variables = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
                break;
            case (not null, true):
                variables = new Dictionary<string, double>(variables);
                break;
            case (not null, false):
                variables = new Dictionary<string, double>(variables, StringComparer.OrdinalIgnoreCase);
                break;
        }
        VerifyVariableNames_Throws(variables);

        // Add the reserved variables to the dictionary
        var compiledConstants = new ReadOnlyConstantRegistry(ConstantRegistry);
        var compiledFunctions = new ReadOnlyFunctionRegistry(FunctionRegistry);
        foreach (var constant in ConstantRegistry)
            variables.Add(constant.ConstantName, constant.Value);

        var function = GetCachedFormulaOrBuild(formulaText, compiledConstants, compiledFunctions);
        return function(variables);
    }

    public FormulaBuilder Formula(string formulaText)
    {
        if (string.IsNullOrWhiteSpace(formulaText))
            throw new ArgumentNullException(nameof(formulaText));

        return new FormulaBuilder(formulaText, this);
    }

    /// <summary>
    /// Build a .NET func for the provided formula.
    /// </summary>
    /// <param name="formulaText">The formula that must be converted into a .NET func.</param>
    /// <returns>A .NET func for the provided formula.</returns>
    public Func<IDictionary<string, double>, double> Build(string formulaText)
    {
        if (string.IsNullOrEmpty(formulaText))
            throw new ArgumentNullException(nameof(formulaText));

        var compiledFunctions = new ReadOnlyFunctionRegistry(FunctionRegistry);
        var compiledConstants = new ReadOnlyConstantRegistry(ConstantRegistry);

        return GetCachedFormulaOrBuild(formulaText, compiledConstants, compiledFunctions);
    }

    /// <summary>
    /// Build a .NET func for the provided formula.
    /// </summary>
    /// <param name="formulaText">The formula that must be converted into a .NET func.</param>
    /// <param name="constants">Constant values for variables defined into the formula. They variables will be replaced by the constant value at pre-compilation time.</param>
    /// <returns>A .NET func for the provided formula.</returns>
    [Obsolete("Please register the constants instead using CalculationEngine.AddConstant, or use the overload that takes an IConstantRegistry.")]
    public Func<IDictionary<string, double>, double> Build(string formulaText, IDictionary<string, double>? constants)
    {
        if (string.IsNullOrEmpty(formulaText))
            throw new ArgumentNullException(nameof(formulaText));


        var compiledConstants = new ConstantRegistry(ConstantRegistry);
        var compiledFunctions = new ReadOnlyFunctionRegistry(FunctionRegistry);
        if (constants != null)
            foreach (var constant in constants)
                compiledConstants.RegisterConstant(constant.Key, constant.Value);
        var readonlyConstants = new ReadOnlyConstantRegistry(compiledConstants);

        return GetCachedFormulaOrBuild(formulaText, readonlyConstants, compiledFunctions);
    }


    /// <summary>
    /// Build a .NET func for the provided formula.
    /// </summary>
    /// <param name="formulaText">The formula that must be converted into a .NET func.</param>
    /// <param name="constantRegistryOverride">Overrides the Engine's ConstantRegistry</param>
    /// <returns>A .NET func for the provided formula.</returns>
    internal Func<IDictionary<string, double>, double> Build(string formulaText, IConstantRegistry constantRegistryOverride)
    {
        if (string.IsNullOrEmpty(formulaText))
            throw new ArgumentNullException(nameof(formulaText));


        var compiledConstants = new ReadOnlyConstantRegistry(constantRegistryOverride);
        var compiledFunctions = new ReadOnlyFunctionRegistry(FunctionRegistry);

        return GetCachedFormulaOrBuild(formulaText, compiledConstants, compiledFunctions);
    }

    /// <summary>
    /// Add a constant to the calculation engine.
    /// </summary>
    /// <param name="constantName">The name of the constant. This name can be used in mathematical formulas.</param>
    /// <param name="value">The value of the constant.</param>
    public void AddConstant(string constantName, double value)
    {
        ConstantRegistry.RegisterConstant(constantName, value);
    }

    private static void RegisterDefaultFunctions(IFunctionRegistry functionRegistry)
    {
        functionRegistry.RegisterFunction("sin", (Func<double, double>)Math.Sin, true, false);
        functionRegistry.RegisterFunction("cos", (Func<double, double>)Math.Cos, true, false);
        functionRegistry.RegisterFunction("csc", (Func<double, double>)MathUtil.Csc, true, false);
        functionRegistry.RegisterFunction("sec", (Func<double, double>)MathUtil.Sec, true, false);
        functionRegistry.RegisterFunction("asin", (Func<double, double>)Math.Asin, true, false);
        functionRegistry.RegisterFunction("acos", (Func<double, double>)Math.Acos, true, false);
        functionRegistry.RegisterFunction("tan", (Func<double, double>)Math.Tan, true, false);
        functionRegistry.RegisterFunction("cot", (Func<double, double>)MathUtil.Cot, true, false);
        functionRegistry.RegisterFunction("atan", (Func<double, double>)Math.Atan, true, false);
        functionRegistry.RegisterFunction("acot", (Func<double, double>)MathUtil.Acot, true, false);
        functionRegistry.RegisterFunction("loge", (Func<double, double>)Math.Log, true, false);
        functionRegistry.RegisterFunction("log10", (Func<double, double>)Math.Log10, true, false);
        functionRegistry.RegisterFunction("logn", (Func<double, double, double>)(Math.Log), true, false);
        functionRegistry.RegisterFunction("sqrt", (Func<double, double>)Math.Sqrt, true, false);
        functionRegistry.RegisterFunction("abs", (Func<double, double>)Math.Abs, true, false);
        functionRegistry.RegisterFunction("if", (Func<double, double, double, double>)((a, b, c) => (a != 0.0 ? b : c)), true, false);
        functionRegistry.RegisterFunction("ifless", (Func<double, double, double, double, double>)((a, b, c, d) => (a < b ? c : d)), true, false);
        functionRegistry.RegisterFunction("ifmore", (Func<double, double, double, double, double>)((a, b, c, d) => (a > b ? c : d)), true, false);
        functionRegistry.RegisterFunction("ifequal", (Func<double, double, double, double, double>)((a, b, c, d) => (a.Equals(b) ? c : d)), true, false);
        functionRegistry.RegisterFunction("ceiling", (Func<double, double>)Math.Ceiling, true, false);
        functionRegistry.RegisterFunction("floor", (Func<double, double>)Math.Floor, true, false);
        functionRegistry.RegisterFunction("truncate", (Func<double, double>)Math.Truncate, true, false);
        functionRegistry.RegisterFunction("round", (Func<double, double>)Math.Round, true, false);

        // Dynamic-based arguments Functions
        functionRegistry.RegisterFunction("max", (DynamicFunc<double, double>)(a => a.Max()), true, false);
        functionRegistry.RegisterFunction("min", (DynamicFunc<double, double>)(a => a.Min()), true, false);
        functionRegistry.RegisterFunction("avg", (DynamicFunc<double, double>)(a => a.Average()), true, false);
        functionRegistry.RegisterFunction("median", (DynamicFunc<double, double>)(a => a.Median()), true, false);

        // Non Idempotent Functions
        functionRegistry.RegisterFunction("random", (Func<double>)random.NextDouble, false, false);
    }

    private static void RegisterDefaultConstants(IConstantRegistry constantRegistry)
    {
        constantRegistry.RegisterConstant("e", Math.E, false);
        constantRegistry.RegisterConstant("pi", Math.PI, false);
    }

    /// <summary>
    /// Build the abstract syntax tree for a given formula. The formula string will
    /// be first tokenized.
    /// </summary>
    /// <param name="formulaText">A string containing the mathematical formula that must be converted
    /// into an abstract syntax tree.</param>
    /// /// <param name="functions">A registry containing the functions that should be used during compilation of the formula.</param>
    /// <param name="constants">A registry containing the constant values that should be used during compilation of the formula.</param>
    /// <returns>The abstract syntax tree of the formula.</returns>
    private Operation BuildAbstractSyntaxTree(string formulaText, IFunctionRegistry? functions, IConstantRegistry? constants)
    {
        var tokenReader = new TokenReader(cultureInfo);
        var tokens = tokenReader.Read(formulaText);

        var astBuilder = new AstBuilder(functions, CaseSensitive, constants);
        var operation = astBuilder.Build(tokens);

        return optimizerEnabled
                   ? optimizer.Optimize(operation, functions, constants)
                   : operation;
    }

    private Func<IDictionary<string, double>, double> GetCachedFormulaOrBuild(string formulaText,
        IConstantRegistry? compiledConstants,
        IFunctionRegistry? functionRegistry)
    {
        return executionFormulaCache.GetOrAdd(
            GenerateFormulaCacheKey(formulaText, compiledConstants), _ =>
            {
                var operation = BuildAbstractSyntaxTree(formulaText, functionRegistry, compiledConstants);
                return executor.BuildFormula(operation, FunctionRegistry, compiledConstants);
            })!;
    }

    private bool TryGetFromFormulaCache(string formulaText, IConstantRegistry? compiledConstants, [NotNullWhen(true)] out Func<IDictionary<string, double>, double>? function)
    {
        function = null;
        return cacheEnabled && executionFormulaCache.TryGetValue(GenerateFormulaCacheKey(formulaText, compiledConstants), out function);
    }

    private static string GenerateFormulaCacheKey(string formulaText, IConstantRegistry? compiledConstants)
    {
        return compiledConstants?.Any() == true
                   ? $"{formulaText}@{string.Join(",", compiledConstants.Select(x => $"{x.ConstantName}:{x.Value}"))}"
                   : formulaText;
    }

    /// <summary>
    /// Verify a collection of variables to ensure that all the variable names are valid.
    /// Users aren't allowed to overwrite reserved variables or use function names as variables.
    /// If an invalid variable is detected, an exception is thrown.
    /// </summary>
    /// <param name="variables">The collection of variables that must be verified.</param>
    internal void VerifyVariableNames_Throws(IDictionary<string, double> variables)
    {
        foreach (var variableName in variables.Keys)
        {
            if (ConstantRegistry.TryGetConstantInfo(variableName, out var constantInfo) && !constantInfo.IsOverWritable)
                throw new ArgumentException($"The name \"{variableName}\" is a reserved variable name that cannot be overwritten.", nameof(variables));

            if (FunctionRegistry.ContainsFunctionName(variableName))
                throw new ArgumentException($"The name \"{variableName}\" is a function name. Parameters cannot have this name.", nameof(variables));
        }
    }
}
