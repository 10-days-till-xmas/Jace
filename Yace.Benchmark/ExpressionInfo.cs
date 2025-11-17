using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Jace;
using Yace.Interfaces;

using System.Reflection;
using Yace.Execution;
using ParameterInfo = Yace.Execution.ParameterInfo;

namespace Yace.Benchmark;

public sealed class ExpressionInfo : IUsesText
{
    #region Shared Properties
    public bool CaseSensitive => true;
    private const int RandomSeed = ':' + '3';
    public string Expression { get; }
    public Library Library { get; }
    public Func<IDictionary<string, double>, double> Raw_CompiledFunction_Interpreted { get; }
    public Func<IDictionary<string, double>, double> Raw_CompiledFunction_Interpreted_Optimized { get; }
    public Delegate CompiledFunction_Interpreted { get; }
    public Delegate CompiledFunction_Interpreted_Optimized { get; }
    public Func<IDictionary<string, double>, double> Raw_CompiledFunction_Dynamic { get; }
    public Func<IDictionary<string, double>, double> Raw_CompiledFunction_Dynamic_Optimized { get; }
    public Delegate CompiledFunction_Dynamic { get; }
    public Delegate CompiledFunction_Dynamic_Optimized { get; }
    #endregion

    #region Yace Properties
    public ParameterInfo[] ParameterInfos { get; }
    public List<Tokenizer.Token> Tokens { get; }
    public Operations.Operation RootOperation { get; }
    public Operations.Operation RootOperation_Optimized { get; }
    public Execution.ReadOnlyFunctionRegistry FunctionRegistry { get; }
    public Execution.ReadOnlyConstantRegistry ConstantRegistry { get; }
    public FormulaContext Context { get; }
    #endregion

    #region Jace Properties
    public Jace.Execution.ParameterInfo[] ParameterInfos_Jace { get; }

    public List<Jace.Tokenizer.Token> Tokens_Jace { get; }

    public Jace.Operations.Operation RootOperation_Jace { get; }
    public Jace.Operations.Operation RootOperation_Optimized_Jace { get; }

    public Jace.Execution.FunctionRegistry FunctionRegistry_Jace { get; }
    public Jace.Execution.ConstantRegistry ConstantRegistry_Jace { get; }
    public Jace.FormulaContext Context_Jace { get; }
    #endregion


    private static readonly Action<Jace.Execution.IFunctionRegistry> m_CalculationEngine_RegisterDefaultFunctions_Jace = (functionRegistry) =>
    {
        var jaceEngine = new Jace.CalculationEngine(new JaceOptions { DefaultFunctions = true });
        foreach (var fi in jaceEngine.Functions) // this is so hacky...
            functionRegistry.RegisterFunction(fi.FunctionName, fi.Function, fi.IsIdempotent, fi.IsOverWritable);
    };
    private static readonly Action<Jace.Execution.IConstantRegistry> m_CalculationEngine_RegisterDefaultConstants_Jace = (constantRegistry) =>
    {
        var jaceEngine = new Jace.CalculationEngine(new JaceOptions { DefaultConstants = true });
        foreach (var ci in jaceEngine.Constants)
            constantRegistry.RegisterConstant(ci.ConstantName, ci.Value, ci.IsOverWritable);
    };

    private static readonly Action<IFunctionRegistry> m_CalculationEngine_RegisterDefaultFunctions =
        typeof(CalculationEngine).GetMethod("RegisterDefaultFunctions", BindingFlags.NonPublic | BindingFlags.Static)!
                                 .CreateDelegate<Action<IFunctionRegistry>>();
    private static readonly Action<IConstantRegistry> m_CalculationEngine_RegisterDefaultConstants =
        typeof(CalculationEngine).GetMethod("RegisterDefaultConstants", BindingFlags.NonPublic | BindingFlags.Static)!
                                 .CreateDelegate<Action<IConstantRegistry>>();

    public ExpressionInfo(string expression, Library lib, params ParameterInfo[] parameterInfos)
    {
        Expression = expression;
        ParameterInfos = parameterInfos;
        ParameterInfos_Jace = parameterInfos.Select(static pi => new Jace.Execution.ParameterInfo
                                             {
                                                 Name = pi.Name,
                                                 DataType = (Jace.DataType)(int)pi.DataType
                                             })
                                            .ToArray();
        Library = lib;
        var random = new Random(RandomSeed);
        _parameterDictionary = ParameterInfos.ToDictionary(static pi => pi.Name, pi => pi.DataType switch
        {
            DataType.Integer => new IntDoubleUnion(random.Next()),
            DataType.FloatingPoint => new IntDoubleUnion(random.NextDouble()),
            _ => throw new NotSupportedException($"Unsupported parameter data type: {pi.DataType}")
        });
        Tokens = new Tokenizer.TokenReader(CultureInfo.CurrentCulture).Read(Expression);
        Tokens_Jace = new Jace.Tokenizer.TokenReader(CultureInfo.CurrentCulture).Read(Expression);

        FunctionRegistry_Jace = CreateDefaultFunctionRegistry_Jace(CaseSensitive);
        ConstantRegistry_Jace = CreateDefaultConstantRegistry_Jace(CaseSensitive);
        Context_Jace = new Jace.FormulaContext(SimpleParameterDictionary, FunctionRegistry_Jace, ConstantRegistry_Jace);

        FunctionRegistry = new ReadOnlyFunctionRegistry(CreateDefaultFunctionRegistry(CaseSensitive));
        ConstantRegistry = new ReadOnlyConstantRegistry(CreateDefaultConstantRegistry(CaseSensitive));
        Context = new FormulaContext(FunctionRegistry, ConstantRegistry, SimpleParameterDictionary);

        RootOperation = new AstBuilder(FunctionRegistry, CaseSensitive)
           .Build(Tokens);
        RootOperation_Jace = new Jace.AstBuilder(FunctionRegistry_Jace, CaseSensitive)
           .Build(Tokens_Jace);

        RootOperation_Optimized_Jace = new Jace.Optimizer(new Jace.Execution.Interpreter())
           .Optimize(RootOperation_Jace, FunctionRegistry_Jace, ConstantRegistry_Jace);

        RootOperation_Optimized = new Optimizer(new Interpreter())
           .Optimize(RootOperation, Context);

        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        switch (lib)
        {
            case Library.Jace:
                Raw_CompiledFunction_Interpreted = new Jace.Execution.Interpreter(CaseSensitive)
                   .BuildFormula(RootOperation_Jace, FunctionRegistry_Jace, ConstantRegistry_Jace);
                Raw_CompiledFunction_Interpreted_Optimized = new Jace.Execution.Interpreter(CaseSensitive)
                   .BuildFormula(RootOperation_Optimized_Jace, FunctionRegistry_Jace, ConstantRegistry_Jace);
                Raw_CompiledFunction_Dynamic = new Jace.Execution.DynamicCompiler(CaseSensitive)
                   .BuildFormula(RootOperation_Jace, FunctionRegistry_Jace, ConstantRegistry_Jace);
                Raw_CompiledFunction_Dynamic_Optimized = new Jace.Execution.DynamicCompiler(CaseSensitive)
                   .BuildFormula(RootOperation_Optimized_Jace, FunctionRegistry_Jace, ConstantRegistry_Jace);

                CompiledFunction_Dynamic = new Jace.Util.FuncAdapter().Wrap(ParameterInfos_Jace, Raw_CompiledFunction_Dynamic);
                CompiledFunction_Dynamic_Optimized = new Jace.Util.FuncAdapter().Wrap(ParameterInfos_Jace, Raw_CompiledFunction_Dynamic_Optimized);
                CompiledFunction_Interpreted = new Jace.Util.FuncAdapter().Wrap(ParameterInfos_Jace, Raw_CompiledFunction_Interpreted);
                CompiledFunction_Interpreted_Optimized = new Jace.Util.FuncAdapter().Wrap(ParameterInfos_Jace, Raw_CompiledFunction_Interpreted_Optimized);
                break;
            case Library.Yace:
                Raw_CompiledFunction_Interpreted = new Interpreter(CaseSensitive)
                   .BuildFormula(RootOperation, FunctionRegistry, ConstantRegistry);
                Raw_CompiledFunction_Interpreted_Optimized = new Interpreter(CaseSensitive)
                   .BuildFormula(RootOperation_Optimized, FunctionRegistry, ConstantRegistry);
                Raw_CompiledFunction_Dynamic = new DynamicCompiler(CaseSensitive)
                   .BuildFormula(RootOperation, FunctionRegistry, ConstantRegistry);
                Raw_CompiledFunction_Dynamic_Optimized = new DynamicCompiler(CaseSensitive)
                   .BuildFormula(RootOperation_Optimized, FunctionRegistry, ConstantRegistry);

                CompiledFunction_Dynamic = Util.FuncAdapter.Wrap(ParameterInfos, Raw_CompiledFunction_Dynamic);
                CompiledFunction_Dynamic_Optimized = Util.FuncAdapter.Wrap(ParameterInfos, Raw_CompiledFunction_Dynamic_Optimized);
                CompiledFunction_Interpreted = Util.FuncAdapter.Wrap(ParameterInfos, Raw_CompiledFunction_Interpreted);
                CompiledFunction_Interpreted_Optimized = Util.FuncAdapter.Wrap(ParameterInfos, Raw_CompiledFunction_Interpreted_Optimized);
                break;
            default:
                throw new NotSupportedException($"Unsupported library: {lib}");
        }
    }

    private static FunctionRegistry CreateDefaultFunctionRegistry(bool caseSensitive)
    {
        var registry = new FunctionRegistry(caseSensitive);
        m_CalculationEngine_RegisterDefaultFunctions(registry);
        return registry;
    }
    private static Jace.Execution.FunctionRegistry CreateDefaultFunctionRegistry_Jace(bool caseSensitive)
    {
        var registry = new Jace.Execution.FunctionRegistry(caseSensitive);
        m_CalculationEngine_RegisterDefaultFunctions_Jace(registry);
        return registry;
    }

    private static ConstantRegistry CreateDefaultConstantRegistry(bool caseSensitive)
    {
            var registry = new ConstantRegistry(caseSensitive);
            m_CalculationEngine_RegisterDefaultConstants(registry);
            return registry;
    }
    private static Jace.Execution.ConstantRegistry CreateDefaultConstantRegistry_Jace(bool caseSensitive)
    {
        var registry = new Jace.Execution.ConstantRegistry(caseSensitive);
        m_CalculationEngine_RegisterDefaultConstants_Jace(registry);
        return registry;
    }

    private readonly Dictionary<string, IntDoubleUnion> _parameterDictionary;
    public Dictionary<string, double> SimpleParameterDictionary =>
        _parameterDictionary.ToDictionary(static kvp => kvp.Key, static kvp => kvp.Value.Type switch
                                         {
                                             DataType.Integer => kvp.Value.IntValue,
                                             DataType.FloatingPoint => kvp.Value.DoubleValue,
                                             _ => throw new NotSupportedException($"Unsupported parameter data type: {kvp.Value.Type}")
                                         });

    public override string ToString() => $"{Library}-{Expression}";
}
