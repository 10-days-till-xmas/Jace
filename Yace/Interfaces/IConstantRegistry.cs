using JetBrains.Annotations;
using Yace.Execution;

namespace Yace.Interfaces;

[PublicAPI]
public interface IConstantRegistry : IRegistry<ConstantInfo>, IUsesText
{

    void Register(string constantName, double value, bool isOverWritable = true);
}

