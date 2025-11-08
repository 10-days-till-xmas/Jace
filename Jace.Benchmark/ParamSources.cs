using System.Collections.Generic;
using Jace.Execution;

namespace Jace.Benchmark;

public static class ParamSources
{
    public static IEnumerable<IExecutor> Executors => [
        new Interpreter(),
        new DynamicCompiler()
    ];
}