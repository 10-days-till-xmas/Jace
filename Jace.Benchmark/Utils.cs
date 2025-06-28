using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using OfficeOpenXml;

namespace Jace.Benchmark;

public static class Utils
{
    public static TimeSpan Measure(Action action)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            action();
        }
        finally
        {
            stopwatch.Stop();
        }

        return stopwatch.Elapsed;
    }

    public static void AddBenchmarkRecord(this DataTable table, string engine, bool caseSensitive, string formula,
        int? iterationsPerRandom, int totalIterations, TimeSpan duration)
    {
        table.Rows.Add(engine, caseSensitive, formula, iterationsPerRandom, totalIterations,
            duration.ToString(@"mm\:ss\.fffffff"));
    }
    
    public static void WriteToExcelFile(DataTable table, string fileName)
    {
        using var excel = new ExcelPackage();
        var worksheet = excel.Workbook.Worksheets.Add("Results");
        worksheet.Cells.LoadFromDataTable(table, true);

        var endColumnLetter = (char)(('A' + table.Columns.Count) - 1);

        worksheet.Cells[$"A1:{endColumnLetter}1"].Style.Font.Bold = true;

        for (var i = 0; i < table.Columns.Count; i++)
            worksheet.Column(i + 1).AutoFit();

        excel.SaveAs(new FileInfo(fileName));
    }
}