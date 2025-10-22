using System;
using System.Collections.Generic;
using System.Linq;
using Jace.Util;

namespace Jace.Execution;

public class FormulaBuilder
{
    private readonly CalculationEngine engine;

    private string formulaText;
    private bool caseSensitive;
    private DataType? resultDataType;
    private List<ParameterInfo> parameters;
    private IDictionary<string, double> constants;

    /// <summary>
    /// Creates a new instance of the FormulaBuilder class.
    /// </summary>
    internal FormulaBuilder(string formulaText, bool caseSensitive, CalculationEngine engine)
    {
        parameters = [];
        constants = new Dictionary<string, double>();
        this.formulaText = formulaText;
        this.engine = engine;
        this.caseSensitive = caseSensitive;
    }

    /// <summary>
    /// Add a new parameter to the formula being constructed. Parameters are
    /// added in the order of which they're defined.
    /// </summary>
    /// <param name="name">The name of the parameter.</param>
    /// <param name="dataType">The date type of the parameter.</param>
    /// <returns>The <see cref="FormulaBuilder"/> instance.</returns>
    public FormulaBuilder Parameter(string name, DataType dataType)
    {
        if (string.IsNullOrEmpty(name))
            throw new ArgumentNullException(nameof(name));

        if (engine.FunctionRegistry.ContainsFunctionName(name))
            throw new ArgumentException($"The name \"{name}\" is a function name. Parameters cannot have this name.", nameof(name));

        if (parameters.Any(p => p.Name == name))
            throw new ArgumentException($"A parameter with the name \"{name}\" was already defined.", nameof(name));

        parameters.Add(new ParameterInfo {Name = name, DataType = dataType});
        return this;
    }

    /// <summary>
    /// Add a new constant to the formula being constructed.
    /// </summary>
    /// <param name="name">The name of the constant.</param>
    /// <param name="constantValue">The value of the constant. Variables for which a constant value is defined will be replaced at pre-compilation time.</param>
    /// <returns>The <see cref="FormulaBuilder"/> instance.</returns>
    public FormulaBuilder Constant(string name, int constantValue)
    {
        return Constant(name, (double)constantValue);
    }

    /// <summary>
    /// Add a new constant to the formula being constructed. The
    /// </summary>
    /// <param name="name">The name of the constant.</param>
    /// <param name="constantValue">The value of the constant.</param>
    /// <returns>The <see cref="FormulaBuilder"/> instance.</returns>
    public FormulaBuilder Constant(string name, double constantValue)
    {
        if (string.IsNullOrEmpty(name))
            throw new ArgumentNullException(nameof(name));

        if (constants.Any(p => p.Key == name))
            throw new ArgumentException($"A constant with the name \"{name}\" was already defined.", nameof(name));

        constants[name] = constantValue;
        return this;
    }

    /// <summary>
    /// Define the result data type for the formula.
    /// </summary>
    /// <param name="dataType">The result data type for the formula.</param>
    /// <returns>The <see cref="FormulaBuilder"/> instance.</returns>
    public FormulaBuilder Result(DataType dataType)
    {
        if (resultDataType.HasValue)
            throw new InvalidOperationException("The result can only be defined once for a given formula.");

        resultDataType = dataType;
        return this;
    }

    /// <summary>
    /// Build the formula defined. This will create a func delegate matching with the parameters
    /// and the return type specified.
    /// </summary>
    /// <returns>The func delegate for the defined formula.</returns>
    public Delegate Build()
    {
        if (!resultDataType.HasValue)
            throw new Exception("Please define a result data type for the formula.");

        var formula = engine.Build(formulaText, constants);

        var adapter = new FuncAdapter();
        var constantRegistry = new ReadOnlyConstantRegistry(engine.ConstantRegistry);
        return adapter.Wrap(parameters, variables => {

            if(!caseSensitive)
                variables = EngineUtil.ConvertVariableNamesToLowerCase(variables);

            engine.VerifyVariableNames(variables);

            // Add the reserved variables to the dictionary
            foreach (var constant in constantRegistry)
                variables.Add(constant.ConstantName, constant.Value);

            return formula(variables);
        });
    }
}