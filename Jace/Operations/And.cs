namespace Jace.Operations;

public sealed class And(DataType dataType, Operation argument1, Operation argument2)
    : BinaryOperation(dataType, argument1, argument2);