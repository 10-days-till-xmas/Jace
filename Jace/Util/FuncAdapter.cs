using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ParameterInfo = Jace.Execution.ParameterInfo;

namespace Jace.Util;

/// <summary>
/// An adapter for creating a func wrapper around a func accepting a dictionary. The wrapper
/// can create a func that has an argument for every expected key in the dictionary.
/// </summary>
public class FuncAdapter
{
    /// <summary>
    /// Wrap the parsed the function into a delegate of the specified type. The delegate must accept
    /// the parameters defined in the parameter collection. The order of parameters is respected as defined
    /// in the parameter collection.
    /// <br/>
    /// The function must accept a dictionary of strings and doubles as input. The values passed to the
    /// wrapping function will be passed to the function using the dictionary. The keys in the dictionary
    /// are the parameter names of the wrapping function.
    /// </summary>
    /// <param name="parameters">The required parameters of the wrapping function delegate.</param>
    /// <param name="function">The function that must be wrapped.</param>
    /// <returns>A delegate instance of the required type.</returns>
    public Delegate Wrap(IEnumerable<ParameterInfo> parameters,
                         Func<IDictionary<string, double>, double> function)
    {
        var parameterArray = parameters.ToArray();

        return GenerateDelegate(parameterArray, function);
    }

    // Uncomment for debugging purposes
    //public void CreateDynamicModuleBuilder()
    //{
    //    AssemblyName assemblyName = new AssemblyName("JaceDynamicAssembly");
    //    AppDomain domain = AppDomain.CurrentDomain;
    //    AssemblyBuilder assemblyBuilder = domain.DefineDynamicAssembly(assemblyName,
    //        AssemblyBuilderAccess.RunAndSave);
    //    ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name, "test.dll");

    //    TypeBuilder typeBuilder = moduleBuilder.DefineType("MyTestClass");

    //    MethodBuilder method = typeBuilder.DefineMethod("MyTestMethod", MethodAttributes.Static, typeof(double),
    //       new Type[] { typeof(FuncAdapterArguments), typeof(int), typeof(double) });

    //    ILGenerator generator = method.GetILGenerator();
    //    GenerateMethodBody(generator, new List<Calculator.Execution.ParameterInfo>() {
    //        new Calculator.Execution.ParameterInfo() { Name = "test1", DataType = DataType.Integer },
    //        new Calculator.Execution.ParameterInfo() { Name = "test2", DataType = DataType.FloatingPoint }},
    //        (a) => 0.0);

    //    typeBuilder.CreateType();

    //    assemblyBuilder.Save(@"test.dll");
    //}

    private Delegate GenerateDelegate(ParameterInfo[] parameterArray,
                                      Func<Dictionary<string, double>, double> function)
    {
        var delegateType = GetDelegateType(parameterArray);
        var dictionaryType = typeof(Dictionary<string, double>);

        var dictionaryExpression =
            Expression.Variable(typeof(Dictionary<string, double>), "dictionary");
        var dictionaryAssignExpression =
            Expression.Assign(dictionaryExpression, Expression.New(dictionaryType));

        var parameterExpressions = new ParameterExpression[parameterArray.Length];

        var methodBody = new List<Expression> { dictionaryAssignExpression };

        for (var i = 0; i < parameterArray.Length; i++)
        {
            // Create parameter expression for each func parameter
            var parameterType = parameterArray[i].DataType == DataType.FloatingPoint ? typeof(double) : typeof(int);
            parameterExpressions[i] = Expression.Parameter(parameterType, parameterArray[i].Name);

            methodBody.Add(Expression.Call(dictionaryExpression,
                                           dictionaryType.GetRuntimeMethod("Add", [typeof(string), typeof(double)]),
                                           Expression.Constant(parameterArray[i].Name),
                                           Expression.Convert(parameterExpressions[i], typeof(double)))
                          );
        }

        var invokeExpression = Expression.Invoke(Expression.Constant(function), dictionaryExpression);
        methodBody.Add(invokeExpression);

        var lambdaExpression = Expression.Lambda(delegateType,
                                                 Expression.Block([dictionaryExpression], methodBody),
                                                 parameterExpressions);

        return lambdaExpression.Compile();
    }

    private Type GetDelegateType(ParameterInfo[] parameters)
    {
        var funcTypeName = $"System.Func`{parameters.Length + 1}";
        var funcType = Type.GetType(funcTypeName);

        var typeArguments = new Type[parameters.Length + 1];
        for (var i = 0; i < parameters.Length; i++)
            typeArguments[i] = (parameters[i].DataType == DataType.FloatingPoint) ? typeof(double) : typeof(int);
        typeArguments[typeArguments.Length - 1] = typeof(double);

        return funcType.MakeGenericType(typeArguments);
    }

    private class FuncAdapterArguments
    {
        private readonly Func<Dictionary<string, double>, double> function;

        public FuncAdapterArguments(Func<Dictionary<string, double>, double> function)
        {
            this.function = function;
        }
    }
}