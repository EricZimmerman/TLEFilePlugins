using System.IO;
using System.Linq;
using System.Text;
using FluentAssertions;
using NUnit.Framework;
using TLEFileEZTools;
using TLEFileGenericCsv;

namespace TLEFile.Test;

/// <summary>
///     CsvLoader (through plugins that use it) and the generic CSV plugin
/// </summary>
[TestFixture]
public class LoaderTests
{
    private string Write(string name, string text)
    {
        var file = Path.Combine(TestContext.CurrentContext.WorkDirectory, name);
        File.WriteAllText(file, text, new UTF8Encoding(true));
        return file;
    }

    [Test]
    public void AnOlderVersionOfACsvLoads()
    {
        //RBCmd before 2026.10 had no Removed column: the plugin claims both header lines, the new column stays empty
        var p = new RBCmd();
        p.ProcessFile(Write("rb_old.csv", "SourceName,FileType,FileName,FileSize,DeletedOn\r\nC:\\$IABC,File,C:\\x.txt,12,2024-01-02 03:04:05\r\n"));

        var row = (RBCmdData) p.DataList[0];
        row.FileSize.Should().Be(12);
        row.Removed.Should().BeNull();

        p.ProcessFile(Write("rb_new.csv", "SourceName,FileType,FileName,FileSize,DeletedOn,Removed\r\nC:\\$IABC,File,C:\\x.txt,12,2024-01-02 03:04:05,True\r\n"));
        ((RBCmdData) p.DataList[0]).Removed.Should().BeTrue();
    }

    [Test]
    public void EmptyNumbersAndDatesDoNotFailTheFile()
    {
        //one empty value failed the whole file
        var p = new RBCmd();
        p.ProcessFile(Write("rb_empty.csv", "SourceName,FileType,FileName,FileSize,DeletedOn,Removed\r\na,File,b,,,\r\nc,File,d,5,2024-01-02 03:04:05,False\r\n"));

        p.DataList.Count.Should().Be(2);
        ((RBCmdData) p.DataList[0]).FileSize.Should().Be(0);
        ((RBCmdData) p.DataList[1]).FileSize.Should().Be(5);
    }

    [Test]
    public void HeaderCaseDoesNotMatter()
    {
        //TLE matches header lines ignoring case: the columns are matched the same way
        var p = new RBCmd();
        p.ProcessFile(Write("rb_case.csv", "sourcename,filetype,filename,filesize,deletedon\r\na,File,b,7,2024-01-02 03:04:05\r\n"));

        ((RBCmdData) p.DataList[0]).FileSize.Should().Be(7);
    }

    [Test]
    public void TimesAreReadAsWritten()
    {
        var p = new RBCmd();
        p.ProcessFile(Write("rb_time.csv", "SourceName,FileType,FileName,FileSize,DeletedOn\r\na,File,b,7,2024-01-02 03:04:05\r\n"));

        ((RBCmdData) p.DataList[0]).DeletedOn.Should().Be(new System.DateTime(2024, 1, 2, 3, 4, 5));
    }

    [TestCase("a,b,c\r\n1,2,3\r\n", ',')]
    [TestCase("a\tb\tc\r\n1\t2\t3\r\n", '\t')]
    [TestCase("a|b|c\r\n1|2|3\r\n", '|')]
    [TestCase("a;b;c\r\n1;2;3\r\n", ';')]
    [TestCase("#TYPE Selected.System.Object\r\na;b;c\r\n1;2;3\r\n", ';')]
    [TestCase("single\r\nvalue\r\n", ',')]
    public void GenericCsvDetectsTheDelimiter(string text, char expected)
    {
        GenericCsv.DetectDelimiter(Write("delim.txt", text)).Should().Be(expected);
    }

    [Test]
    public void GenericCsvLoadsAnySeparatedFile()
    {
        var p = new GenericCsv();
        p.TaggedLines.Add(2);
        p.ProcessFile(Write("generic.csv", "Name;Value;\r\na;1;\r\nb;2;\r\n"));

        p.DataList.Count.Should().Be(2);
        var rows = p.DataList.Cast<System.Collections.Generic.IDictionary<string, object>>().ToList();
        rows[0]["Name"].Should().Be("a");
        rows[0]["Value"].Should().Be("1");
        rows[1]["Tag"].Should().Be(true);
        rows[0].Keys.Should().NotContain(k => string.IsNullOrWhiteSpace(k), "a column without a name is left out");
    }
}
