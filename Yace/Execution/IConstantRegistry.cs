using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace Yace.Execution;

[PublicAPI]
public interface IConstantRegistry : IEnumerable<ConstantInfo>, IUsesText
{
    StringComparer Comparer { get; }
    ConstantInfo GetConstantInfo(string constantName);
    bool TryGetConstantInfo(string constantName, [NotNullWhen(true)] out ConstantInfo? constantInfo);
    bool ContainsConstantName(string constantName);

    void RegisterConstant(string constantName, double value, bool isOverWritable = true);

    void RegisterConstant(ConstantInfo constantInfo);
}
