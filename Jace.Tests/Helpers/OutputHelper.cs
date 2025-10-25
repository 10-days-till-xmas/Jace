#if !NET5_0
using Xunit;
#endif
namespace Jace.Tests.Helpers;

public static class OutputHelper
{
    #if NET5_0
    public static Xunit.Abstractions.ITestOutputHelper Output => null!;
    #else
    public static ITestOutputHelper Output => TestContext.Current.TestOutputHelper!;
    #endif
}