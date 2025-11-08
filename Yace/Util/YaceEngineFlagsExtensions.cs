namespace Yace.Util;

internal static class YaceEngineFlagsExtensions
{
    public static bool HasFlagFast(this YaceEngineFlags value, YaceEngineFlags flag)
    {
        return (value & flag) != 0;
    }

    public static YaceEngineFlags SetFlag(this YaceEngineFlags value, bool flag, YaceEngineFlags flagToSet)
    {
        return flag
                   ? value | flagToSet
                   : value & ~flagToSet;
    }
}
