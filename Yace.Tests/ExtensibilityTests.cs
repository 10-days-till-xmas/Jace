using Xunit;
using Yace.Execution;
using Yace.Operations;
using Yace.Tests.Mocks;

namespace Yace.Tests;

public class ExtensibilityTests
{
    [Fact]
    public void OperationsAreExtensible()
    {
        // sqrt(16 + 9) = 5
        var expr = new Sqrt(
            new Addition(
                DataType.FloatingPoint,
                new IntegerConstant(16),
                new IntegerConstant(9)));
        var interpreter = new Interpreter();
        var result = interpreter.Execute(
            expr,
            null!);
        Assert.Equal(5.0, result);
    }
}
