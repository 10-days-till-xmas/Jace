using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace Yace.Execution;

[PublicAPI]
public interface IConstantRegistry : IRegistry<ConstantInfo>, IUsesText
{

    void Register(string constantName, double value, bool isOverWritable = true);
}

