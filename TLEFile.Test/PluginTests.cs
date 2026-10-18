using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using CsvHelper;
using FluentAssertions;
using ITLEFileSpec;
using NUnit.Framework;

namespace TLEFile.Test;

/// <summary>
///     Every plugin, found as Timeline Explorer finds them (IFileSpec types in the plugin assemblies), and checked against
///     its own header lines, the current tools' header lines and the layouts
/// </summary>
[TestFixture]
public class PluginTests
{
    private static readonly Assembly[] PluginAssemblies =
    {
        typeof(TLEFileEZTools.Boot).Assembly, typeof(TLEFileMisc.DensityScout).Assembly, typeof(TLEFileKAPE.KapeTriage).Assembly,
        typeof(TLEFileMicrosoft.Autoruns).Assembly, typeof(TLEFileTimelines.MacTime).Assembly, typeof(TLEFileTzWorks.Wisp).Assembly,
        typeof(TLEFileGenericCsv.GenericCsv).Assembly
    };

    //plugins that parse their files by hand (fixed formats, pipes, line prefixes...): a CSV with a header and typed
    //values says nothing about them
    private static readonly HashSet<string> HandParsed = new(StringComparer.Ordinal)
    {
        "TLEFileMisc.DensityScout", "TLEFileTzWorks.PeScan", "TLEFileTzWorks.Wisp", "TLEFileTimelines.PsortTimeline",
        "TLEFileTimelines.SuperTimeline", "TLEFileTimelines.MacTime", "TLEFileTimelines.ForensicTimeline", "TLEFileTimelines.KapeMiniTimeline",
        "TLEFileMisc.AnalyzeMft", "TLEFileMisc.ShimcacheParser", "TLEFileMisc.ShimcacheVolatility", "TLEFileMisc.ConvertPSHistoryToCSV",
        "TLEFileMicrosoft.Sigcheck", "TLEFileMicrosoft.SigcheckTroy", "TLEFileMicrosoft.Autoruns"
    };

    internal static IReadOnlyList<IFileSpec> Plugins() =>
        PluginAssemblies.SelectMany(a => a.GetTypes())
            .Where(t => !t.IsAbstract && typeof(IFileSpec).IsAssignableFrom(t))
            .Select(t => (IFileSpec) Activator.CreateInstance(t))
            .ToList();

    /// <summary>
    ///     What Timeline Explorer does: the first plugin whose ExpectedHeaders has the first line (ignoring case)
    /// </summary>
    internal static IFileSpec Match(string firstLine) =>
        Plugins().FirstOrDefault(p => p.ExpectedHeaders.Contains(firstLine));

    private static Type DataType(IFileSpec p) => p.DataList.GetType().GetGenericArguments().FirstOrDefault();

    public static IEnumerable<TestCaseData> HeaderLines() =>
        Plugins().Where(p => !HandParsed.Contains(p.GetType().FullName) && DataType(p) != null)
            .SelectMany(p => p.ExpectedHeaders.Select((h, i) => new TestCaseData(p.GetType().FullName, i).SetName($"Loads {p.GetType().FullName} header {i + 1}")));

    [Test]
    public void InternalGuidsAreUnique()
    {
        Plugins().GroupBy(p => p.InternalGuid).Where(g => g.Count() > 1).Select(g => g.Key).Should().BeEmpty();
    }

    [Test]
    public void NoHeaderLineIsClaimedTwice()
    {
        //TLE would always pick the first one
        Plugins().SelectMany(p => p.ExpectedHeaders.Select(h => (Header: h.ToLowerInvariant(), Plugin: p.GetType().FullName)))
            .GroupBy(x => x.Header).Where(g => g.Count() > 1).Select(g => string.Join(", ", g.Select(x => x.Plugin)))
            .Should().BeEmpty();
    }

    /// <summary>
    ///     A file with each header line a plugin claims, and three rows of typed values (empty for columns the data class
    ///     doesn't have by that name), loads all three rows and tags the second
    /// </summary>
    [TestCaseSource(nameof(HeaderLines))]
    public void EveryHeaderLineLoads(string plugin, int index)
    {
        var p = Plugins().Single(x => x.GetType().FullName == plugin);
        var header = p.ExpectedHeaders.ElementAt(index);
        var file = Path.Combine(TestContext.CurrentContext.WorkDirectory, $"{plugin}_{index}.csv");
        File.WriteAllText(file, Synthesize(DataType(p), header), new UTF8Encoding(true));

        Match(header).Should().NotBeNull().And.Subject.GetType().FullName.Should().Be(plugin);

        p.TaggedLines.Add(2);
        p.ProcessFile(file);

        p.DataList.Count.Should().Be(3);
        ((IFileSpecData) p.DataList[1]).Tag.Should().BeTrue();
        ((IFileSpecData) p.DataList[0]).Line.Should().Be(1);
    }

    /// <summary>
    ///     The header line of every CSV the 2026.10 tools write goes to a plugin, not the generic CSV loader. A tool that
    ///     changes its CSV should change TestFiles/CurrentHeaders.tsv, and this shows which plugin to update
    /// </summary>
    [Test]
    public void CurrentToolHeadersHaveAPlugin()
    {
        var unmatched = new List<string>();
        foreach (var line in File.ReadAllLines(Path.Combine(TestContext.CurrentContext.TestDirectory, "TestFiles", "CurrentHeaders.tsv"), Encoding.UTF8))
        {
            if (line.StartsWith('#') || line.Length == 0)
            {
                continue;
            }

            var parts = line.Split('\t', 3);
            if (Match(parts[2]) == null)
            {
                unmatched.Add($"{parts[0]} {parts[1]}");
            }
        }

        unmatched.Should().BeEmpty();
    }

    /// <summary>
    ///     TLE finds a layout by the plugin's full class name (in Layouts/Default). Misspelt names were never used
    /// </summary>
    [Test]
    public void EveryLayoutBelongsToAPlugin()
    {
        var names = Plugins().Select(p => p.GetType().FullName).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var layouts = Directory.GetFiles(LayoutsDir(), "*.layout").Select(f => Path.GetFileNameWithoutExtension(f)).ToList();

        layouts.Should().NotBeEmpty();
        layouts.Where(l => !names.Contains(l)).Should().BeEmpty();

        //every EZ Tools plugin has one
        names.Where(n => n.StartsWith("TLEFileEZTools.")).Where(n => !layouts.Contains(n, StringComparer.OrdinalIgnoreCase)).Should().BeEmpty();
    }

    [Test]
    public void LayoutsHaveEveryColumn()
    {
        //TLE runs on Windows, where file names ignore case (AppCompatcache.layout is AppCompatCache's); the tests also
        //run on Linux
        var files = Directory.GetFiles(LayoutsDir(), "*.layout")
            .ToDictionary(f => Path.GetFileNameWithoutExtension(f), StringComparer.OrdinalIgnoreCase);

        foreach (var p in Plugins().Where(p => p.GetType().FullName.StartsWith("TLEFileEZTools.")))
        {
            var layout = File.ReadAllText(files[p.GetType().FullName]);
            var props = DataType(p).GetProperties().Select(x => x.Name);
            props.Where(n => !layout.Contains($"<property name=\"Name\">col{n}</property>", StringComparison.OrdinalIgnoreCase))
                .Should().BeEmpty($"{p.GetType().FullName}.layout should have every column");
        }
    }

    private static string LayoutsDir()
    {
        for (var d = new DirectoryInfo(TestContext.CurrentContext.TestDirectory); d != null; d = d.Parent)
        {
            var candidate = Path.Combine(d.FullName, "Layouts", "Default");
            if (Directory.Exists(candidate))
            {
                return candidate;
            }
        }

        throw new DirectoryNotFoundException("Layouts/Default not found above the test directory");
    }

    /// <summary>
    ///     The tools' outputs in the folder named by TLE_CSV_DIR (real data: not in the repository): each goes to a
    ///     plugin and every row loads
    /// </summary>
    [Test]
    [Explicit]
    public void RealOutputs()
    {
        var dir = Environment.GetEnvironmentVariable("TLE_CSV_DIR");
        if (string.IsNullOrEmpty(dir))
        {
            Assert.Ignore("TLE_CSV_DIR not set");
        }

        foreach (var file in Directory.GetFiles(dir, "*.csv", SearchOption.AllDirectories))
        {
            var first = File.ReadLines(file).First();
            var p = Match(first);
            if (p == null)
            {
                TestContext.Out.WriteLine($"generic CSV: {Path.GetFileName(file)}");
                continue;
            }

            p.ProcessFile(file);
            p.DataList.Count.Should().Be(CountRows(file), Path.GetFileName(file));
            TestContext.Out.WriteLine($"{p.GetType().FullName}: {p.DataList.Count:N0} rows from {Path.GetFileName(file)}");
        }
    }

    private static int CountRows(string file)
    {
        using var reader = new StreamReader(file);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        csv.Read();
        csv.ReadHeader();
        var n = 0;
        while (csv.Read())
        {
            n++;
        }

        return n;
    }

    internal static string Synthesize(Type data, string header)
    {
        var delimiter = header.Contains('\t') ? "\t" : ",";
        string[] cols;
        using (var r = new StringReader(header))
        using (var parser = new CsvParser(r, new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture) {Delimiter = delimiter}))
        {
            parser.Read();
            cols = parser.Record;
        }

        var sb = new StringBuilder(header).Append("\r\n");
        for (var row = 0; row < 3; row++)
        {
            sb.Append(string.Join(delimiter, cols.Select(c => Value(data, c, row)))).Append("\r\n");
        }

        return sb.ToString();
    }

    private static string Value(Type data, string col, int row)
    {
        var prop = data.GetProperties().FirstOrDefault(x => x.Name.Equals(col.Trim(), StringComparison.OrdinalIgnoreCase));
        if (prop == null)
        {
            //a column the plugin maps under another name (or not at all): empty gives the member its default
            return string.Empty;
        }

        var t = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
        if (t == typeof(string)) return $"v{row}";
        if (t == typeof(bool)) return row % 2 == 0 ? "True" : "False";
        if (t == typeof(DateTime) || t == typeof(DateTimeOffset)) return $"2024-01-0{row + 1} 03:04:05";
        if (t == typeof(Guid)) return Guid.Empty.ToString();
        if (t == typeof(double) || t == typeof(float) || t == typeof(decimal)) return $"{row}.5";
        if (t.IsEnum) return Enum.GetNames(t)[0];
        return $"{row + 1}";
    }
}
