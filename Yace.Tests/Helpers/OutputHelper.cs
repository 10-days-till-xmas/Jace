#if !NET5_0
using Xunit;
#endif
namespace Yace.Tests.Helpers;

// ReSharper disable once UnusedType.Global
// Used during debugging
public static class OutputHelper
{
    #if NET5_0
    public static Xunit.Abstractions.ITestOutputHelper Output => null!;
    #else
    public static ITestOutputHelper Output => TestContext.Current.TestOutputHelper!;
    #endif
}
