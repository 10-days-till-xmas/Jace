using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace Yace.Benchmark;

public sealed class JobIdColumn : IColumn
{
    public string GetValue(Summary summary, BenchmarkCase benchmarkCase) => benchmarkCase.Job.Id;

    public string GetValue(Summary summary, BenchmarkCase benchmarkCase, SummaryStyle style) => GetValue(summary, benchmarkCase);

    public bool IsDefault(Summary summary, BenchmarkCase benchmarkCase) => benchmarkCase.Job.Id == Job.Default.Id;

    public bool IsAvailable(Summary summary) => true;

    public string Id => nameof(JobIdColumn);
    public string ColumnName => "Job Id";
    public bool AlwaysShow => true;
    public ColumnCategory Category => ColumnCategory.Job;
    public int PriorityInCategory => 0;
    public bool IsNumeric => false;
    public UnitType UnitType => UnitType.Dimensionless;
    public string Legend => "Identifier of the Job associated with the benchmark case";
}
