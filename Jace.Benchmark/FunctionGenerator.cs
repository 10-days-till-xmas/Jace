using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Jace.Benchmark;

public class FunctionGenerator(CultureInfo cultureInfo)
{
    private const int NumberOfVariables = 3;

    private readonly Random random = new();

    public FunctionGenerator()
        : this(CultureInfo.CurrentCulture)
    {
    }

    public string Next()
    {
        var variables = new Queue<string>();
        for (var i = 0; i < NumberOfVariables; i++)
            variables.Enqueue("var" + (i + 1));

        var sb = new StringBuilder();
        Generate(sb, variables);

        return sb.ToString();
    }

    private void Generate(StringBuilder result, Queue<string> variables)
    {
        while (true)
        {
            var value = random.NextDouble();

            switch (value)
            {
                case < 0.35:
                    result.Append(variables.Dequeue());
                    result.Append(GetRandomOperator());

                    if (variables.Count > 0)
                        continue;
                    break;
                case < 0.8:
                    result.Append(GetRandomValue());
                    result.Append(GetRandomOperator());

                    if (variables.Count > 0)
                        continue;
                    break;
                default:
                    if (variables.Count > 0)
                    {
                        result.Append('(');
                        Generate(result, variables);
                        result.Append(')');
                        result.Append(GetRandomOperator());

                        if (variables.Count > 0)
                            continue;
                    }
                    break;
            }
            result.Append(GetRandomValue());
            break;
        }
    }

    private char GetRandomOperator()
    {
        var value = random.NextDouble();

        return value switch
        {
            < 0.2 => '+',
            < 0.4 => '*',
            < 0.6 => '/',
            < 0.8 => '^',
            _ => '-'
        };
    }

    private string GetRandomValue()
    {
        var value = random.NextDouble();

        return (value < 0.6
            ? random.Next()
            : (random.Next() * random.NextDouble()))
            .ToString(cultureInfo);
    }
}