using FsCheck;
using FsCheck.Fluent;

namespace Jace.Tests.Helpers;

public static class PropertyFilterHelpers
{
    public static bool IsInvalid(this double d)
    {
        return double.IsNaN(d) || double.IsInfinity(d);
    }
}