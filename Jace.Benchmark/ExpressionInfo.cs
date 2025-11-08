using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Jace.Execution;
using Jace.Operations;
using Jace.Tokenizer;
using Jace.Util;
using ParameterInfo = Jace.Execution.ParameterInfo;

namespace Jace.Benchmark;

public sealed class ExpressionInfo : IUsesText
{
    public bool CaseSensitive => true;
    private const int RandomSeed = ':' + '3';

    public string Expression { get; }
    public ParameterInfo[] ParameterInfos { get; }
    public IEnumerable<Token> Tokens { get; }
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

    public ReadOnlyFunctionRegistry FunctionRegistry { get; }
    public ReadOnlyConstantRegistry ConstantRegistry { get; }

    private static readonly Action<IFunctionRegistry> m_CalculationEngine_RegisterDefaultFunctions =
        typeof(CalculationEngine).GetMethod("RegisterDefaultFunctions", BindingFlags.NonPublic | BindingFlags.Static)!
                                 .CreateDelegate<Action<IFunctionRegistry>>();
    public ExpressionInfo(string expression, params ParameterInfo[] parameterInfos)
    {
        Expression = expression;
        ParameterInfos = parameterInfos;
        var random = new Random(RandomSeed);
        ParameterDictionary = ParameterInfos.ToDictionary(static pi => pi.Name, pi => pi.DataType switch
        {
            DataType.Integer => new IntDoubleUnion(random.Next()),
            DataType.FloatingPoint => new IntDoubleUnion(random.NextDouble()),
            _ => throw new NotSupportedException($"Unsupported parameter data type: {pi.DataType}")
        });

        Tokens = new TokenReader(CultureInfo.CurrentCulture)
           .Read(Expression);
        FunctionRegistry = new ReadOnlyFunctionRegistry(CreateDefaultFunctionRegistry(CaseSensitive));
        ConstantRegistry = new ReadOnlyConstantRegistry(CreateDefaultConstantRegistry(CaseSensitive));
        var functionRegistry = CreateDefaultFunctionRegistry(CaseSensitive);
        var constantRegistry = CreateDefaultConstantRegistry(CaseSensitive);
        RootOperation = new AstBuilder(functionRegistry, CaseSensitive)
           .Build(Tokens);
        RootOperation_Optimized = new Optimizer(new Interpreter())
           .Optimize(RootOperation, functionRegistry, constantRegistry);
        Raw_CompiledFunction_Interpreted = new Interpreter(CaseSensitive)
           .BuildFormula(RootOperation, functionRegistry, constantRegistry);
        Raw_CompiledFunction_Interpreted_Optimized = new Interpreter(CaseSensitive)
           .BuildFormula(RootOperation_Optimized, functionRegistry, constantRegistry);
        Raw_CompiledFunction_Dynamic = new DynamicCompiler(CaseSensitive)
           .BuildFormula(RootOperation, functionRegistry, constantRegistry);
        Raw_CompiledFunction_Dynamic_Optimized = new DynamicCompiler(CaseSensitive)
           .BuildFormula(RootOperation_Optimized, functionRegistry, constantRegistry);

        CompiledFunction_Dynamic = FuncAdapter.Wrap(ParameterInfos, Raw_CompiledFunction_Dynamic);
        CompiledFunction_Dynamic_Optimized = FuncAdapter.Wrap(ParameterInfos, Raw_CompiledFunction_Dynamic_Optimized);
        CompiledFunction_Interpreted = FuncAdapter.Wrap(ParameterInfos, Raw_CompiledFunction_Interpreted);
        CompiledFunction_Interpreted_Optimized = FuncAdapter.Wrap(ParameterInfos, Raw_CompiledFunction_Interpreted_Optimized);
    }

    private static FunctionRegistry CreateDefaultFunctionRegistry(bool caseSensitive)
    {
        var registry = new FunctionRegistry(caseSensitive);
        m_CalculationEngine_RegisterDefaultFunctions(registry);
        return registry;
    }

    private static readonly Action<IConstantRegistry> m_CalculationEngine_RegisterDefaultConstants =
        typeof(CalculationEngine).GetMethod("RegisterDefaultConstants", BindingFlags.NonPublic | BindingFlags.Static)!
                                 .CreateDelegate<Action<IConstantRegistry>>();
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