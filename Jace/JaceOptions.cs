using System.Globalization;
using Jace.Execution;

namespace Jace;

public record JaceOptions
{
    private const int DefaultCacheMaximumSize = 500;
    private const int DefaultCacheReductionSize = 50;

    public static JaceOptions Default => new();

    /// The <see cref="CultureInfo"/> required for correctly reading floating point numbers.
    public CultureInfo CultureInfo { get; set; } = CultureInfo.CurrentCulture;

    /// The execution mode that must be used for formula execution.
    public ExecutionMode ExecutionMode { get; set; } = ExecutionMode.Compiled;

    /// Enable or disable caching of mathematical formulas.
    public bool CacheEnabled { get; set; } = true;

    /// Configure the maximum cache size for mathematical formulas.
    public int CacheMaximumSize { get; set; } = DefaultCacheMaximumSize;

    /// Configure the cache reduction size for mathematical formulas.
    public int CacheReductionSize { get; set; } = DefaultCacheReductionSize;

    /// Enable or disable optimizing of formulas.
    public bool OptimizerEnabled { get; set; } = true;

    /// Enable case-sensitive or case-insensitive processing mode.
    public bool CaseSensitive { get; set; } = false;

    /// Enable or disable the default functions.
    public bool DefaultFunctions { get; set; } = true;

    /// Enable or disable the default constants.
    public bool DefaultConstants { get; set; } = true;
}