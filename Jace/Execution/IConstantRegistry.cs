using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace Jace.Execution;

[PublicAPI]
public interface IConstantRegistry : IEnumerable<ConstantInfo>
{
    ConstantInfo GetConstantInfo(string constantName);
    bool TryGetConstantInfo(string constantName, [NotNullWhen(true)] out ConstantInfo? constantInfo);
    bool ContainsConstantName(string constantName);

    void RegisterConstant(string constantName, double value, bool isOverWritable = true);
}