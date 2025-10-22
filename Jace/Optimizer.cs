﻿using System.Linq;
using Jace.Execution;
using Jace.Operations;

namespace Jace;

public sealed class Optimizer(IExecutor executor)
{
    public Operation Optimize(Operation operation, IFunctionRegistry functionRegistry, IConstantRegistry constantRegistry)
    {
        // TODO: Consider adding more optimization rules here.
        // TODO: Refactor to make it non-recursive
        switch (operation)
        {
            case { DependsOnVariables: false, IsIdempotent: true } and (not IntegerConstant or FloatingPointConstant):
                var result = executor.Execute(operation, functionRegistry, constantRegistry);
                return new FloatingPointConstant(result);
            case Function function:
                function.Arguments = function.Arguments
                                             .Select(arg => Optimize(arg, functionRegistry, constantRegistry))
                                             .ToList();
                break;
            case Multiplication multiplication:
                multiplication.Argument1 = Optimize(multiplication.Argument1, functionRegistry, constantRegistry);
                multiplication.Argument2 = Optimize(multiplication.Argument2, functionRegistry, constantRegistry);

                if (multiplication.Argument1 is Constant { DoubleValue: 0.0 }
                 || multiplication.Argument2 is Constant { DoubleValue: 0.0 })
                    return new FloatingPointConstant(0.0);
                break;
            case UnaryOperation unaryOperation:
                unaryOperation.Argument = Optimize(unaryOperation.Argument, functionRegistry, constantRegistry);
                break;
            case BinaryOperation binaryOperation:
                binaryOperation.Argument1 = Optimize(binaryOperation.Argument1, functionRegistry, constantRegistry);
                binaryOperation.Argument2 = Optimize(binaryOperation.Argument2, functionRegistry, constantRegistry);
                break;
        }
        return operation;
    }
}