using System.Collections.Generic;
using System.Linq;
using Jace.Operations;
using Jace.Execution;

namespace Jace;

public sealed class Optimizer(IExecutor executor)
{
    public Operation Optimize(Operation operation, IFunctionRegistry functionRegistry, IConstantRegistry constantRegistry)
    {
        // TODO: have Operation have a method for this to call to contain logic
        // and avoid the need for a switch statement
        
        switch (operation)
        {
            case not IntegerConstant and not FloatingPointConstant and { DependsOnVariables: false, IsIdempotent: true }:
                var result = executor.Execute(operation, functionRegistry, constantRegistry);
                return new FloatingPointConstant(result);
            case Multiplication multiplication when multiplication.Argument1 is FloatingPointConstant { Value: 0 } 
                                                    || multiplication.Argument2 is FloatingPointConstant { Value: 0 }:
                multiplication.Argument1 = Optimize(multiplication.Argument1, functionRegistry, constantRegistry);
                multiplication.Argument2 = Optimize(multiplication.Argument2, functionRegistry, constantRegistry);
                return new FloatingPointConstant(0.0);
            case BinaryOperation binaryOperation:
                binaryOperation.Argument1 = Optimize(binaryOperation.Argument1, functionRegistry, constantRegistry);
                binaryOperation.Argument2 = Optimize(binaryOperation.Argument2, functionRegistry, constantRegistry);
                break;
            case UnaryOperation unaryOperation:
                unaryOperation.Argument = Optimize(unaryOperation.Argument, functionRegistry, constantRegistry);
                break;
            case Function function:
                IList<Operation> arguments = function.Arguments.Select(a => Optimize(a, functionRegistry, constantRegistry)).ToList();
                function.Arguments = arguments;
                break;
        }

        return operation;
    }
}