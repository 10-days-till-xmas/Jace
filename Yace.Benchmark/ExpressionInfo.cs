using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Yace.Interfaces;

#if BENCHJACE
using Jace;
using Jace.Execution;
using Jace.Operations;
using Jace.Tokenizer;
using Jace.Util;
using ParameterInfo = Jace.Execution.ParameterInfo;
using _AstBuilder = Jace.AstBuilder;
using _DataType = Jace.DataType;
using _Optimizer = Jace.Optimizer;
using _FormulaContext = Jace.FormulaContext;
#else
using Yace.Execution;
using Yace.Operations;
using Yace.Tokenizer;
using Yace.Util;
using ParameterInfo = Yace.Execution.ParameterInfo;
using _AstBuilder = Yace.AstBuilder;
using _DataType = Yace.DataType;
using _Optimizer = Yace.Optimizer;
using _FormulaContext = Yace.FormulaContext;
#endif
namespace Yace.Benchmark;

public sealed class ExpressionInfo
#if !BENCHJACE
    : IUsesText
#endif
{
#pragma warning disable CA1822
    public bool CaseSensitive => true;
#pragma warning restore CA1822
    private const int RandomSeed = ':' + '3';

    public string Expression { get; }
    public ParameterInfo[] ParameterInfos { get; }
    public List<Token> Tokens { get; }
    public Operation RootOperation { get; }
    public Operation RootOperation_Optimized { get; }
    public Func<IDictionary<string, double>, double> Raw_CompiledFunction_Interpreted { get; }
    public Func<IDictionary<string, double>, double> Raw_CompiledFunction_Interpreted_Optimized { get; }
    public Delegate CompiledFunction_Interpreted { get; }
    public Delegate CompiledFunction_Interpreted_Optimized { get; }
    public Func<IDictionary<string, double>, double> Raw_CompiledFunction_Dynamic { get; }
    public Func<IDictionary<string, double>, double> Raw_CompiledFunction_Dynamic_Optimized { get; }
    public Delegate CompiledFunction_Dynamic { get; }
    public Delegate CompiledFunction_Dynamic_Optimized { get; }

    #if BENCHJACE
    public FunctionRegistry FunctionRegistry { get; }
    public ConstantRegistry ConstantRegistry { get; }
    #else
    public ReadOnlyFunctionRegistry FunctionRegistry { get; }
    public ReadOnlyConstantRegistry ConstantRegistry { get; }
    #endif
    public _FormulaContext Context { get; }
    #if BENCHJACE
    private static readonly Action<IFunctionRegistry> m_CalculationEngine_RegisterDefaultFunctions = (functionRegistry) =>
    {
        var jaceEngine = new Jace.CalculationEngine(new JaceOptions { DefaultFunctions = true });
        foreach (var fi in jaceEngine.Functions) // this is so hacky...
            functionRegistry.RegisterFunction(fi.FunctionName, fi.Function, fi.IsIdempotent, fi.IsOverWritable);
    };
    private static readonly Action<IConstantRegistry> m_CalculationEngine_RegisterDefaultConstants = (constantRegistry) =>
    {
        var jaceEngine = new Jace.CalculationEngine(new JaceOptions { DefaultConstants = true });
        foreach (var ci in jaceEngine.Constants)
            constantRegistry.Register(ci.ConstantName, ci.Value, ci.IsOverWritable);
    };
    #else
    private static readonly Action<IFunctionRegistry> m_CalculationEngine_RegisterDefaultFunctions =
        typeof(CalculationEngine).GetMethod("RegisterDefaultFunctions", BindingFlags.NonPublic | BindingFlags.Static)!
                                 .CreateDelegate<Action<IFunctionRegistry>>();
    private static readonly Action<IConstantRegistry> m_CalculationEngine_RegisterDefaultConstants =
        typeof(CalculationEngine).GetMethod("RegisterDefaultConstants", BindingFlags.NonPublic | BindingFlags.Static)!
                                 .CreateDelegate<Action<IConstantRegistry>>();
    #endif
    public ExpressionInfo(string expression, params ParameterInfo[] parameterInfos)
    {
        Expression = expression;
        ParameterInfos = parameterInfos;
        var random = new Random(RandomSeed);
        ParameterDictionary = ParameterInfos.ToDictionary(static pi => pi.Name, pi => pi.DataType switch
        {
            _DataType.Integer => new IntDoubleUnion(random.Next()),
            _DataType.FloatingPoint => new IntDoubleUnion(random.NextDouble()),
            _ => throw new NotSupportedException($"Unsupported parameter data type: {pi.DataType}")
        });
        Tokens = new TokenReader(CultureInfo.CurrentCulture).Read(Expression);
        #if BENCHJACE
        FunctionRegistry = CreateDefaultFunctionRegistry(CaseSensitive);
        ConstantRegistry = CreateDefaultConstantRegistry(CaseSensitive);
        Context = new Jace.FormulaContext(SimpleParameterDictionary, FunctionRegistry, ConstantRegistry);
        #else
        FunctionRegistry = new ReadOnlyFunctionRegistry(CreateDefaultFunctionRegistry(CaseSensitive));
        ConstantRegistry = new ReadOnlyConstantRegistry(CreateDefaultConstantRegistry(CaseSensitive));
        Context = new _FormulaContext(FunctionRegistry, ConstantRegistry, SimpleParameterDictionary);
        #endif

        var functionRegistry = CreateDefaultFunctionRegistry(CaseSensitive);

        RootOperation = new _AstBuilder(functionRegistry, CaseSensitive)
           .Build(Tokens);
        #if BENCHJACE
        RootOperation_Optimized = new _Optimizer(new Interpreter())
                                     .Optimize(RootOperation, FunctionRegistry, ConstantRegistry);
        #else
        RootOperation_Optimized = new _Optimizer(new Interpreter())
           .Optimize(RootOperation, Context);
        #endif
        Raw_CompiledFunction_Interpreted = new Interpreter(CaseSensitive)
           .BuildFormula(RootOperation, FunctionRegistry, ConstantRegistry);
        Raw_CompiledFunction_Interpreted_Optimized = new Interpreter(CaseSensitive)
           .BuildFormula(RootOperation_Optimized, FunctionRegistry, ConstantRegistry);
        Raw_CompiledFunction_Dynamic = new DynamicCompiler(CaseSensitive)
           .BuildFormula(RootOperation, FunctionRegistry, ConstantRegistry);
        Raw_CompiledFunction_Dynamic_Optimized = new DynamicCompiler(CaseSensitive)
           .BuildFormula(RootOperation_Optimized, FunctionRegistry, ConstantRegistry);

        #if BENCHJACE
        CompiledFunction_Dynamic = new FuncAdapter().Wrap(ParameterInfos, Raw_CompiledFunction_Dynamic);
        CompiledFunction_Dynamic_Optimized = new FuncAdapter().Wrap(ParameterInfos, Raw_CompiledFunction_Dynamic_Optimized);
        CompiledFunction_Interpreted = new FuncAdapter().Wrap(ParameterInfos, Raw_CompiledFunction_Interpreted);
        CompiledFunction_Interpreted_Optimized = new FuncAdapter().Wrap(ParameterInfos, Raw_CompiledFunction_Interpreted_Optimized);
        #else
        CompiledFunction_Dynamic = FuncAdapter.Wrap(ParameterInfos, Raw_CompiledFunction_Dynamic);
        CompiledFunction_Dynamic_Optimized = FuncAdapter.Wrap(ParameterInfos, Raw_CompiledFunction_Dynamic_Optimized);
        CompiledFunction_Interpreted = FuncAdapter.Wrap(ParameterInfos, Raw_CompiledFunction_Interpreted);
        CompiledFunction_Interpreted_Optimized = FuncAdapter.Wrap(ParameterInfos, Raw_CompiledFunction_Interpreted_Optimized);
        #endif
    }

    private static FunctionRegistry CreateDefaultFunctionRegistry(bool caseSensitive)
    {
        var registry = new FunctionRegistry(caseSensitive);
        m_CalculationEngine_RegisterDefaultFunctions(registry);
        return registry;
    }

    private static ConstantRegistry CreateDefaultConstantRegistry(bool caseSensitive)
    {
            var registry = new ConstantRegistry(caseSensitive);
            m_CalculationEngine_RegisterDefaultConstants(registry);
            return registry;
    }
    public Dictionary<string, IntDoubleUnion> ParameterDictionary { get; }
    public Dictionary<string, double> SimpleParameterDictionary =>
        ParameterDictionary.ToDictionary(static kvp => kvp.Key, static kvp => kvp.Value.Type switch
                                         {
                                             DataType.Integer => kvp.Value.IntValue,
                                             DataType.FloatingPoint => kvp.Value.DoubleValue,
                                             _ => throw new NotSupportedException($"Unsupported parameter data type: {kvp.Value.Type}")
                                         });

    public override string ToString() => Expression;
}
