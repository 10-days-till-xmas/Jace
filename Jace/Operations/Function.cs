using System.Collections.Generic;
using System.Linq;

namespace Jace.Operations;

public sealed class Function(DataType dataType, string functionName, IList<Operation> arguments, bool isIdempotent)
    : Operation(dataType, arguments.Any(static o => o.DependsOnVariables),
                isIdempotent && arguments.All(static o => o.IsIdempotent))
{
    private IList<Operation> arguments = arguments;

    public string FunctionName { get; private set; } = functionName;

    public IList<Operation> Arguments {
        get => arguments;
        internal set
        {
            arguments = value;
            DependsOnVariables = arguments.FirstOrDefault(o => o.DependsOnVariables) != null;
        }
    }
}