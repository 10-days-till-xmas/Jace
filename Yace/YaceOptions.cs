using System.Globalization;
using Yace.Util;
using JetBrains.Annotations;
using Yace.Execution;

namespace Yace;

[PublicAPI]
public sealed record YaceOptions
{
    private const int DefaultCacheMaximumSize = 500;
    private const int DefaultCacheReductionSize = 50;

    public YaceOptions()
        : this(CultureInfo.InvariantCulture) {}

    public YaceOptions(CultureInfo cultureInfo, ExecutionMode executionMode = ExecutionMode.Compiled,
        YaceEngineFlags flags = YaceEngineFlags.Default,
        int cacheMaximumSize = DefaultCacheMaximumSize, int cacheReductionSize = DefaultCacheReductionSize)
    {
        CultureInfo = cultureInfo;
        ExecutionMode = executionMode;
        YaceOptionFlags = flags;
        CacheMaximumSize = cacheMaximumSize;
        CacheReductionSize = cacheReductionSize;
    }

    public static YaceOptions Default => new();

    /// The <see cref="CultureInfo"/> required for correctly reading floating point numbers.
    public CultureInfo CultureInfo { get; init; } = CultureInfo.CurrentCulture;

    /// The execution mode that must be used for formula execution.
    public ExecutionMode ExecutionMode { get; init; } = ExecutionMode.Compiled;

    /// Enable or disable caching of mathematical formulas.
    public bool CacheEnabled
    {
        get => YaceOptionFlags.HasFlagFast(YaceEngineFlags.CacheEnabled);
        init => YaceOptionFlags = YaceOptionFlags.SetFlag(value, YaceEngineFlags.CacheEnabled);
    }

    /// Configure the maximum cache size for mathematical formulas.
    public int CacheMaximumSize { get; init; } = DefaultCacheMaximumSize;

    /// Configure the cache reduction size for mathematical formulas.
    public int CacheReductionSize { get; init; } = DefaultCacheReductionSize;

    /// Enable or disable optimizing of formulas.
    public bool OptimizerEnabled
    {
        get => YaceOptionFlags.HasFlagFast(YaceEngineFlags.OptimizerEnabled);
        init => YaceOptionFlags = YaceOptionFlags.SetFlag(value, YaceEngineFlags.OptimizerEnabled);
    }

    /// Enable case-sensitive or case-insensitive processing mode.
    public bool CaseSensitive
    {
        get => YaceOptionFlags.HasFlagFast(YaceEngineFlags.CaseSensitive);
        init => YaceOptionFlags = YaceOptionFlags.SetFlag(value, YaceEngineFlags.CaseSensitive);
    }

    /// Enable or disable the default functions.
    public bool DefaultFunctions
    {
        get => YaceOptionFlags.HasFlagFast(YaceEngineFlags.DefaultFunctions);
        init => YaceOptionFlags = YaceOptionFlags.SetFlag(value, YaceEngineFlags.DefaultFunctions);
    }

    /// Enable or disable the default constants.
    public bool DefaultConstants
    {
        get => YaceOptionFlags.HasFlagFast(YaceEngineFlags.DefaultConstants);
        init => YaceOptionFlags = YaceOptionFlags.SetFlag(value, YaceEngineFlags.DefaultConstants);
    }

    public YaceEngineFlags YaceOptionFlags { get; private init; } = YaceEngineFlags.Default;
}