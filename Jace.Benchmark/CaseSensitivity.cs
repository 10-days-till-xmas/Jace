using System;

namespace Jace.Benchmark;

[Flags]
public enum CaseSensitivity
{
    CaseSensitive = 1,
    CaseInSensitive = 2,
    All = CaseSensitive | CaseInSensitive
}