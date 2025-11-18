# Yace.NET
![Build status](https://github.com/10-days-till-xmas/Yace/actions/workflows/build.yml/badge.svg) 
![Build status](https://github.com/10-days-till-xmas/Yace/actions/workflows/test.yml/badge.svg)

![.NET](https://img.shields.io/badge/.NET%20Standard-2.0-blue?logo=.net)
![.NET](https://img.shields.io/badge/.NET-6.0-blue?logo=.net)
![.NET](https://img.shields.io/badge/.NET-9.0-blue?logo=.net)

Yace.NET is a high-performance calculation engine for the .NET platform. It stands for "Yet Another Calculation Engine".

It builds upon Jace.NET by improving the extensibility and safety of the codebase while maintaining high performance, 
along with taking advantage of the latest .NET features.

## What does it do?
Yace.NET can interpret and execute strings containing mathematical formulas. These formulas can rely on variables. 
If variables are used, values can be provided for these variables when executing the mathematical formula.

Yace can execute formulas in two modes: in interpreted mode and in a dynamic compilation mode. 
If dynamic compilation mode is used, Yace will create a dynamic method at runtime and will generate the 
necessary MSIL opcodes for native execution of the formula. If a formula is re-executed with other variables, 
Yace will take the dynamically generated method from its cache. 
It is recommended to use Yace in dynamic compilation mode in scenarios where the same formula will be repeated multiple times.

## Wiki
For detailed information on how to use Yace.NET, please consult the [Jace wiki](https://github.com/pieterderycke/Jace/wiki)

A wiki specifically for Yace.NET is currently being built.

## Architecture
Yace.NET follows a design similar to most of the modern compilers. Interpretation and execution are done in a number of phases:

1. **Tokenization**
    - The string is converted into the different kinds of tokens: variables, operators and constants.
2. **Abstract Syntax Tree Creation**
    - The tokenized input is converted into a hierarchical tree representing the mathematical formula.
3. **Optimization**
    - The abstract syntax tree is optimized by executing the lower operations when determined to be constant and then replacing that tree node with the result.
4. **Interpreted Execution/Dynamic Compilation**
    - The abstract syntax tree is executed in either interpreted mode or in dynamic compilation mode (using Linq.Expressions) by calculating values in the tree bottom-up.

## Examples
Yace.NET can be used in a couple of ways:

To directly execute a given mathematical formula using the provided variables:
```csharp
var variables = new Dictionary<string, double>
{
    { "var1", 2.5 },
    { "var2", 3.4 }
};
var engine = new CalculationEngine();
var result = engine.Calculate("var1*var2", variables);
```

To build a .NET Func accepting a dictionary as input containing the values for each variable:
```csharp
var engine = new CalculationEngine();
var formula = engine.Build("var1+2/(3*otherVariable)");

var variables = new Dictionary<string, double>
{
    { "var1", 2 },
    { "otherVariable", 4.2 }
};

var result = formula(variables);
```

To build a typed .NET Func:
```csharp
var engine = new CalculationEngine();
var formula = (Func<int, double, double>)engine.Formula("var1+2/(3*otherVariable)")
                                               .Parameter("var1", DataType.Integer)
                                               .Parameter("otherVariable", DataType.FloatingPoint)
                                               .Result(DataType.FloatingPoint)
                                               .Build();

var result = formula(2, 4.2);
```

Functions can be used inside the mathematical formulas. Yace.NET currently offers the following functions:
<details>
    <summary>Click to view</summary>

```
sin(a)
cos(x)
csc(x)
sec(x)
asin(x)
acos(x)
tan(x)
cot(x)
atan(x)
acot(x)
loge(x)
log10(x)
logn(x, newBase)
sqrt(x)
abs(x)
if(a, x, y) // if a != 0 then x else y
ifless(x, y, a, b) // if x < y then a else b
ifmore(x, y, a, b) // if x > y then a else b
ifequal(x, y, a, b) // if x == y then a else b
ceiling(x)
floor(x)
truncate(x)
round(x)

// Dynamic-based arguments Functions
max(params x)
min(params x)
avg(params x)
median(params x)

// Non Idempotent Functions
random()
```
</details>

```csharp
var variables = new Dictionary<string, double>
{
    { "var1", 2.5 },
    { "var2", 3.4 }
};

var engine = new CalculationEngine();
var result = engine.Calculate("logn(var1,var2)+4", variables);
```

## Performance
Below you can find the results of Yace.NET benchmark that demonstrate its capabilities as a high-performance calculation engine.
Additionally, feel free to run the benchmarks yourself by cloning the repository and executing the following command in the solution directory:
```
dotnet run -c Release --project Yace.Benchmark
```
<details>
    <summary>Click to view</summary>

```
BenchmarkDotNet v0.15.2, Windows 11 (10.0.26200.7171)
AMD Ryzen 7 4800H with Radeon Graphics 2.90GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.100-rc.2.25502.107
  [Host] : .NET 9.0.10 (9.0.1025.47515), X64 RyuJIT AVX2
  Jace   : .NET 9.0.10 (9.0.1025.47515), X64 RyuJIT AVX2
  Yace   : .NET 9.0.10 (9.0.1025.47515), X64 RyuJIT AVX2
```

| Type                        | Method                                   | expression                          |   Mean (Yace) |   Mean (Jace) | Ratio | Allocated (Yace) | Allocated (Jace) | Alloc Ratio |
|-----------------------------|------------------------------------------|-------------------------------------|--------------:|--------------:|------:|-----------------:|-----------------:|------------:|
| AstBuilderBenchmarks        | BenchBuildAst                            | (var1 + var2 * 3)/(2+3) - something |     759.58 ns |     808.69 ns |  0.94 |           1504 B |           1608 B |        0.94 |
| AstBuilderBenchmarks        | BenchBuildAst                            | logn(var1, (2+3) * 500)             |     689.10 ns |     710.90 ns |  0.97 |           1496 B |           1592 B |        0.94 |
|                             |                                          |                                     |               |               |       |                  |                  |             |
| CalculationEngineBenchmarks | Calculate_DynamicCompiler                | (var1 + var2 * 3)/(2+3) - something | 109,090.29 ns | 172,827.97 ns |  0.63 |          20531 B |          33186 B |        0.62 |
| CalculationEngineBenchmarks | Calculate_Interpreter                    | (var1 + var2 * 3)/(2+3) - something |  23,444.89 ns |  13,013.53 ns |  1.80 |          14510 B |          20143 B |        0.72 |
| CalculationEngineBenchmarks | Calculate_DynamicCompiler                | logn(var1, (2+3) * 500)             | 216,317.30 ns | 273,300.99 ns |  0.79 |          22674 B |          32749 B |        0.69 |
| CalculationEngineBenchmarks | Calculate_Interpreter                    | logn(var1, (2+3) * 500)             |  23,177.50 ns |  12,573.76 ns |  1.84 |          14070 B |          19021 B |        0.74 |
|                             |                                          |                                     |               |               |       |                  |                  |             |
| OptimizerBenchmarks         | OptimizeOperation_Interpreter            | (var1 + var2 * 3)/(2+3) - something |     116.34 ns |     100.11 ns |  1.16 |            368 B |            408 B |        0.90 |
| OptimizerBenchmarks         | OptimizeOperation_Interpreter            | logn(var1, (2+3) * 500)             |      80.90 ns |      87.60 ns |  0.92 |            384 B |            376 B |        1.02 |
|                             |                                          |                                     |               |               |       |                  |                  |             |
| TokenReaderBenchmarks       | Tokenize                                 | (var1 + var2 * 3)/(2+3) - something |     458.66 ns |     475.72 ns |  0.96 |           1904 B |           2464 B |        0.77 |
| TokenReaderBenchmarks       | Tokenize                                 | logn(var1, (2+3) * 500)             |     348.34 ns |     359.87 ns |  0.97 |           1528 B |           1896 B |        0.81 |
|                             |                                          |                                     |               |               |       |                  |                  |             |
| ExecutorBenchmarks          | BuildFormula_Optimized_Interpreter       | (var1 + var2 * 3)/(2+3) - something |      16.17 ns |      15.82 ns |  1.02 |            136 B |            136 B |        1.00 |
| ExecutorBenchmarks          | BuildFormula_Optimized_DynamicCompiler   | (var1 + var2 * 3)/(2+3) - something |  69,493.79 ns |  70,468.53 ns |  0.99 |           6129 B |           6641 B |        0.92 |
| ExecutorBenchmarks          | BuildFormula_Optimized_Interpreter       | logn(var1, (2+3) * 500)             |      15.99 ns |      16.00 ns |  1.00 |            136 B |            136 B |        1.00 |
| ExecutorBenchmarks          | BuildFormula_Optimized_DynamicCompiler   | logn(var1, (2+3) * 500)             | 173,026.68 ns | 119,673.35 ns |  1.45 |           8620 B |           6976 B |        1.24 |
|                             |                                          |                                     |               |               |       |                  |                  |             |
|                             |                                          |                                     |               |               |       |                  |                  |             |
| FunctionExecutionBenchmarks | Execute_Dynamic_Optimized                | (var1 + var2 * 3)/(2+3) - something |     104.09 ns |      99.47 ns |  1.05 |            256 B |            256 B |        1.00 |
| FunctionExecutionBenchmarks | Execute_Interpreted_Optimized            | (var1 + var2 * 3)/(2+3) - something |     108.66 ns |     135.35 ns |  0.80 |            256 B |            216 B |        1.19 |
| FunctionExecutionBenchmarks | Execute_Dynamic_Optimized                | logn(var1, (2+3) * 500)             |     100.07 ns |      89.41 ns |  1.12 |            296 B |            256 B |        1.16 |
| FunctionExecutionBenchmarks | Execute_Interpreted_Optimized            | logn(var1, (2+3) * 500)             |     137.84 ns |     113.98 ns |  1.21 |            456 B |            256 B |        1.78 |

#### Note
Where the code slowed down in comparison to Jace.NET, this is mostly due to favoring improved extensibility and maintainability of the codebase. 
Yace.NET offers a more modular architecture that allows easier addition of new features and functions, 
which may introduce slight overhead in certain scenarios. Regardless, improvements are continuously being made to 
enhance performance without compromising the design principles.

</details>

## Migration from Jace.NET
Most of the API of Yace.NET is similar to Jace.NET, but there are some differences due to the improved architecture and extensibility of Yace.NET.
Here are some of the key changes to be aware of when migrating from Jace.NET to Yace.NET:
- Many methods that previously accepted `IFunctionRegistry`/`IConstantRegistry` parameters have been changed 
to accept `ReadOnlyFunctionRegistry`/`ReadOnlyConstantRegistry` parameters instead. To continue using IFunctionRegistry/ConstantRegistry,
you can either construct a new registry from the existing one, or use the `-Capturing(...)` overload instead. Note that this may open up potential bugs.
- Most of the classes now implement an IUsesText interface to allow better tracking of case sensitivity.
- The registry interfaces now both implement the `IRegistry<T>` interface to enforce better extensibility. Additionally,
the names of methods in these interfaces have been changed to be more consistent.
- Building formulae now doesn't capture the registries by default. This includes the `CalculationEngine` and `FormulaBuilder` methods. 
You can use the `-Capturing(...)` overloads and manually execute to continue using this behavior.
However, plans to add it as an option are being considered.
- For more detailed information, please refer to the source code for the new method signatures and class structures.
No type names have been changed.

## Limitations
If you're using Yace.NET inside a Unity project using IL2CPP, you must use Yace.NET in interpreted mode due to limitations of IL2CPP with dynamic code generation.

## More Information
For more information on the Jace.NET library, you can read the following articles:
* http://pieterderycke.wordpress.com/2012/11/04/jace-net-just-another-calculation-engine-for-net/
* http://www.codeproject.com/Articles/682589/Jace-NET-Just-another-calculation-engine-for-NET
