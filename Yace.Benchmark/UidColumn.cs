using System.Linq;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace Yace.Benchmark;

public sealed class UidColumn : IColumn
{
    public string Id => nameof(UidColumn);
    public string ColumnName => "UID";
    public bool AlwaysShow => true;
    public ColumnCategory Category => ColumnCategory.Job;
    public int PriorityInCategory => 0;
    public bool IsNumeric => false;
    public UnitType UnitType => UnitType.Dimensionless;
    public string Legend => "Unique benchmark identifier (type + method + parameters)";
    public string GetValue(Summary summary, BenchmarkCase benchmarkCase)
    {
        var type = benchmarkCase.Descriptor.Type;
        var method = benchmarkCase.Descriptor.WorkloadMethod;
        var parameters = benchmarkCase.Parameters.Items;
        var paramString = string.Join("; ", parameters.Select(static p => $"{p.Name}={p.Value}"));
        return $"{type.Name}.{method.Name}({paramString})";
    }

    public string GetValue(Summary summary, BenchmarkCase benchmarkCase, SummaryStyle style) =>
        GetValue(summary, benchmarkCase);

    public bool IsDefault(Summary summary, BenchmarkCase benchmarkCase) => false;

    public bool IsAvailable(Summary summary) => true;
}
