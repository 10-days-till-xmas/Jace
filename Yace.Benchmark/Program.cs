using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using BenchmarkDotNet.Analysers;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Diagnostics.dotTrace;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Exporters.Json;
using BenchmarkDotNet.Exporters.Xml;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using Perfolizer.Horology;
using Perfolizer.Metrology;
using Yace.Benchmark;

CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;

// dotnet run -c Release --project Yace.Benchmark

if (args is ["profile"])
{
    // Run with profiling. No BenchmarkDotNet features.
    Profiler.RunClean();
    Console.WriteLine("Profiling complete.");
    return;
}

var assembly = Assembly.GetEntryAssembly()
            ?? throw new InvalidOperationException("Entry assembly not found.");

IColumn[] columns = [
    new UidColumn(),
    new JobIdColumn(),
    StatisticColumn.Mean,
    StatisticColumn.Error,
    StatisticColumn.StdDev,
    StatisticColumn.Min,
    StatisticColumn.Q1,
    StatisticColumn.Median,
    StatisticColumn.Q3,
    StatisticColumn.Max,
    StatisticColumn.OperationsPerSecond,
    BaselineRatioColumn.RatioMean,
    BaselineRatioColumn.RatioStdDev,
    BaselineAllocationRatioColumn.RatioMean,
    BaselineColumn.Default
];
var jobs = new List<Job> { Job.Default
                              .WithId("Yace")
                              .WithCustomBuildConfiguration("Release") };
#if BENCHJACE
jobs.Add(Job.Default
            .WithId("Jace")
            .WithCustomBuildConfiguration("BenchJace")
            .AsBaseline());
#endif
IConfig config = ManualConfig.CreateEmpty()
                             .AddColumnProvider(DefaultColumnProviders.Descriptor,
                                                DefaultColumnProviders.Params,
                                                DefaultColumnProviders.Metrics)
                             .AddColumn(columns)
                             .WithSummaryStyle(SummaryStyle.Default
                                                           .WithMaxParameterColumnWidth(50))
                             .AddLogger(ConsoleLogger.Default)
                             .AddDiagnoser(MemoryDiagnoser.Default,
                                           ThreadingDiagnoser.Default,
                                           new DotTraceDiagnoser())
                             .AddExporter(HtmlExporter.Default,
                                          MarkdownExporter.GitHub,
                                          XmlExporter.Default,
                                          JsonExporter.Default,
                                          new CsvExporter(CsvSeparator.Comma,
                                              new SummaryStyle(cultureInfo: null,
                                                  printUnitsInHeader: true,
                                                  SizeUnit.B, TimeUnit.Nanosecond,
                                                  printUnitsInContent: false, printZeroValuesInContent: true, maxParameterColumnWidth: 50)))
                             .WithOption(ConfigOptions.JoinSummary, true)
                             .AddAnalyser(EnvironmentAnalyser.Default)
                             .AddJob(jobs.ToArray())
                             .WithWakeLock(WakeLockType.Display);

BenchmarkRunner.Run(assembly, config, args);
