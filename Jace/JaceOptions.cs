using System.Globalization;
using Jace.Execution;
using Jace.Util;
using JetBrains.Annotations;

namespace Jace;

[PublicAPI]
public sealed record JaceOptions
{
    private const int DefaultCacheMaximumSize = 500;
    private const int DefaultCacheReductionSize = 50;

    public JaceOptions()
        : this(CultureInfo.InvariantCulture) {}

    public JaceOptions(CultureInfo cultureInfo, ExecutionMode executionMode = ExecutionMode.Compiled,
        JaceEngineFlags flags = JaceEngineFlags.Default,
        int cacheMaximumSize = DefaultCacheMaximumSize, int cacheReductionSize = DefaultCacheReductionSize)
    {
        CultureInfo = cultureInfo;
        ExecutionMode = executionMode;
        JaceOptionFlags = flags;
        CacheMaximumSize = cacheMaximumSize;
        CacheReductionSize = cacheReductionSize;
    }

    public static JaceOptions Default => new();

    /// The <see cref="CultureInfo"/> required for correctly reading floating point numbers.
    public CultureInfo CultureInfo { get; init; } = CultureInfo.CurrentCulture;

    /// The execution mode that must be used for formula execution.
    public ExecutionMode ExecutionMode { get; init; } = ExecutionMode.Compiled;

    /// Enable or disable caching of mathematical formulas.
    public bool CacheEnabled
    {
        get => JaceOptionFlags.HasFlagFast(JaceEngineFlags.CacheEnabled);
        init => JaceOptionFlags = JaceOptionFlags.SetFlag(value, JaceEngineFlags.CacheEnabled);
    }

    /// Configure the maximum cache size for mathematical formulas.
    public int CacheMaximumSize { get; init; } = DefaultCacheMaximumSize;

    /// Configure the cache reduction size for mathematical formulas.
    public int CacheReductionSize { get; init; } = DefaultCacheReductionSize;

    /// Enable or disable optimizing of formulas.
    public bool OptimizerEnabled
    {
        get => JaceOptionFlags.HasFlagFast(JaceEngineFlags.OptimizerEnabled);
        init => JaceOptionFlags = JaceOptionFlags.SetFlag(value, JaceEngineFlags.OptimizerEnabled);
    }

    /// Enable case-sensitive or case-insensitive processing mode.
    public bool CaseSensitive
    {
        get => JaceOptionFlags.HasFlagFast(JaceEngineFlags.CaseSensitive);
        init => JaceOptionFlags = JaceOptionFlags.SetFlag(value, JaceEngineFlags.CaseSensitive);
    }

    /// Enable or disable the default functions.
    public bool DefaultFunctions
    {
        get => JaceOptionFlags.HasFlagFast(JaceEngineFlags.DefaultFunctions);
        init => JaceOptionFlags = JaceOptionFlags.SetFlag(value, JaceEngineFlags.DefaultFunctions);
    }

    /// Enable or disable the default constants.
    public bool DefaultConstants
    {
        get => JaceOptionFlags.HasFlagFast(JaceEngineFlags.DefaultConstants);
        init => JaceOptionFlags = JaceOptionFlags.SetFlag(value, JaceEngineFlags.DefaultConstants);
    }

    public JaceEngineFlags JaceOptionFlags { get; private init; } = JaceEngineFlags.Default;
}