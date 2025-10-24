namespace Jace.Util;

internal static class JaceEngineFlagsExtensions
{
    public static bool HasFlagFast(this JaceEngineFlags value, JaceEngineFlags flag)
    {
        return (value & flag) != 0;
    }

    public static JaceEngineFlags SetFlag(this JaceEngineFlags value, bool flag, JaceEngineFlags flagToSet)
    {
        return flag
                   ? value | flagToSet
                   : value & ~flagToSet;
    }
}