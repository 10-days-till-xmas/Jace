using System.Collections.Generic;
using Yace.Operations;
using Yace.Tokenizer;

namespace Yace.Interfaces;

public interface IAstBuilder : IUsesText
{
    Operation Build(IEnumerable<Token> tokens);
}
