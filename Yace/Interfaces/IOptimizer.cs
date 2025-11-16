using Yace.Operations;

namespace Yace;

public interface IOptimizer
{
    Operation Optimize(Operation operation, FormulaContext context);
}
