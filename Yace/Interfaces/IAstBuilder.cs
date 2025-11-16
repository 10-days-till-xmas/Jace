using System.Collections.Generic;
using Yace.Execution;
using Yace.Operations;
using Yace.Tokenizer;

namespace Yace;

public interface IAstBuilder : IUsesText
{
    Operation Build(IEnumerable<Token> tokens);
}
