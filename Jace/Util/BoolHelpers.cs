namespace Jace.Util;

internal static class BoolHelpers
{
    public static bool AsBool(this double value) => value != 0;
    
    public static double AsDouble(this bool value) => value ? 1 : 0;
}