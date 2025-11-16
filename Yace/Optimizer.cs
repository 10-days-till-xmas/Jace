using System.Linq;
using Yace.Interfaces;
using Yace.Operations;

namespace Yace;

public sealed class Optimizer(IExecutor executor) : IOptimizer
{
    public Operation Optimize(Operation operation, FormulaContext context)
    {
        // TODO: Consider adding more optimization rules here.
        // TODO: Refactor to make it non-recursive
        switch (operation)
        {
            case { DependsOnVariables: false, IsIdempotent: true } and (not IntegerConstant or FloatingPointConstant):
                var result = executor.Execute(operation, context);
                return new FloatingPointConstant(result);
            case Function function:
                function.Arguments = function.Arguments
                                             .Select(arg => Optimize(arg, context))
                                             .ToList();
                break;
            case Multiplication multiplication:
                multiplication.Argument1 = Optimize(multiplication.Argument1, context);
                multiplication.Argument2 = Optimize(multiplication.Argument2, context);

                if (multiplication.Argument1 is Constant { DoubleValue: 0.0 }
                 || multiplication.Argument2 is Constant { DoubleValue: 0.0 })
                    return new FloatingPointConstant(0.0);
                break;
            case UnaryOperation unaryOperation:
                unaryOperation.Argument = Optimize(unaryOperation.Argument, context);
                break;
            case BinaryOperation binaryOperation:
                binaryOperation.Argument1 = Optimize(binaryOperation.Argument1, context);
                binaryOperation.Argument2 = Optimize(binaryOperation.Argument2, context);
                break;
        }
        return operation;
    }
}
