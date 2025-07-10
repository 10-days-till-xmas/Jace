using Jace.Execution;
using JetBrains.Annotations;

namespace Jace.Tests;

[UsedImplicitly(ImplicitUseTargetFlags.WithInheritors, 
    Reason = "This class is used as a base class for tests.")]
public abstract partial class CalculationEngineTests(ExecutionMode executionMode);
public class InterpretedEngineTests() : CalculationEngineTests(ExecutionMode.Interpreted);
public class CompiledEngineTests() : CalculationEngineTests(ExecutionMode.Compiled);
