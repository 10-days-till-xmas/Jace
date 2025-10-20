﻿using System;

namespace Jace.Execution;

public sealed record FunctionInfo(
    string FunctionName,
    int NumberOfParameters,
    bool IsIdempotent,
    bool IsOverWritable,
    bool IsDynamicFunc,
    Delegate Function);