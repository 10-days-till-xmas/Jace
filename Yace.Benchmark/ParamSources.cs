using System.Collections.Generic;
using Yace.Execution;

namespace Yace.Benchmark;

public static class ParamSources
{
    public static IEnumerable<IExecutor> Executors => [
        new Interpreter(),
        new DynamicCompiler()
    ];
}