using System;

namespace Yace;

[Flags]
public enum YaceEngineFlags
{
    None = 0,
    /// Enable or disable caching of mathematical formulas.
    CacheEnabled = 1 << 0,
    /// Enable or disable optimizing of formulas.
    OptimizerEnabled = 1 << 1,
    /// Enable case-sensitive or case-insensitive processing mode.
    CaseSensitive = 1 << 2,
    /// Enable or disable the default functions.
    DefaultFunctions = 1 << 3,
    /// Enable or disable the default constants.
    DefaultConstants = 1 << 4,
    Default = CacheEnabled | OptimizerEnabled | DefaultFunctions | DefaultConstants
}