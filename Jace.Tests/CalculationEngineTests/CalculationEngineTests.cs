using Jace.Execution;
using JetBrains.Annotations;

namespace Jace.Tests;

[UsedImplicitly]
public partial class CalculationEngineTests
{
    public abstract partial class TestExecutionModeBase(ExecutionMode executionMode);
    [UsedImplicitly]
    public class TestCalculationEngine_Interpreted() : TestExecutionModeBase(ExecutionMode.Interpreted);
    [UsedImplicitly]
    public class TestCalculationEngine_Compiled() : TestExecutionModeBase(ExecutionMode.Compiled);
}