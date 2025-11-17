using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Diagnostics.dotTrace;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using Yace.Execution;



namespace Yace.Benchmark.Benchmarks;
[DotTraceDiagnoser]
[DisassemblyDiagnoser]
public abstract class YaceBenchmarkBase
{
    private static readonly (string expr, ParameterInfo[] parameters)[] _expressionData =
    [
        ("logn(var1, (2+3) * 500)",
            [ new ParameterInfo { Name="var1", DataType = DataType.FloatingPoint } ]),
        ("(var1 + var2 * 3)/(2+3) - something",
            [
                new ParameterInfo { Name="var1", DataType = DataType.FloatingPoint },
                new ParameterInfo { Name="var2", DataType = DataType.FloatingPoint },
                new ParameterInfo { Name="something", DataType = DataType.FloatingPoint }
            ])
    ];
    private static readonly ExpressionInfo[] _expressions = _expressionData.Select(static ed => new ExpressionInfo(ed.expr,
            #if BENCHJACE
            Library.Jace,
            #else
            Library.Yace,
            #endif
            ed.parameters)).ToArray();
    

#pragma warning disable CA1822
    public ExpressionInfo[] Expressions => _expressions;
#pragma warning restore CA1822
}
