using Yace.Operations;

namespace Yace.Interfaces;

public interface IOptimizer
{
    Operation Optimize(Operation operation, FormulaContext context);
}
