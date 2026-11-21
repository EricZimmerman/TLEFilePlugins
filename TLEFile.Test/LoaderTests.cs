using System.IO;
using System.Linq;
using System.Text;
using TLEFileTimelines;
using TLEFileMisc;
using TLEFileMicrosoft;
using System;
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
    public void BadQuotingAndTimesInUtcLoad()
    {
        //CsvHelper 33 threw on a stray quote (the file stopped there) and converted a time ending in Z to local time
        var p = new RBCmd();
        p.ProcessFile(Write("rb_quote.csv", "SourceName,FileType,FileName,FileSize,DeletedOn\r\na\"b,File,c,1,2024-01-02T03:04:05Z\r\nd,File,e,2,2024-01-02 03:04:05\r\n"));

        p.DataList.Count.Should().Be(2);
        var row = (RBCmdData) p.DataList[0];
        row.SourceName.Should().Be("a\"b");
        row.DeletedOn.Should().Be(new DateTime(2024, 1, 2, 3, 4, 5, DateTimeKind.Utc));
        row.DeletedOn.Kind.Should().Be(DateTimeKind.Utc);
        ((RBCmdData) p.DataList[1]).Line.Should().Be(2);
    }

    [Test]
    public void RepeatedValuesShareOneString()
    {
        //one string for a value repeated in many rows (TLE keeps every row), also in a column where most values are new
        var sb = new StringBuilder("SourceName,FileType,FileName,FileSize,DeletedOn\r\n");
        for (var i = 0; i < 5000; i++)
        {
            sb.Append($"C:\\$I{i},File,C:\\Users\\name\\file{i % 2000}.txt,{i},2024-01-02 03:04:05\r\n");
        }

        var p = new RBCmd();
        p.ProcessFile(Write("rb_pool.csv", sb.ToString()));

        var rows = p.DataList.Cast<RBCmdData>().ToList();
        rows.Should().HaveCount(5000);
        ReferenceEquals(rows[10].FileName, rows[2010].FileName).Should().BeTrue();
        ReferenceEquals(rows[10].FileType, rows[11].FileType).Should().BeTrue();
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

    [TestCase("SuperTimelineLowerCase.csv")]
    [TestCase("SuperTimelinePlaso.csv")]
    public void SuperTimelineHeaderCaseDoesNotMatter(string name)
    {
        //TLE matched the lower-case header line, but the plugin read the MACB column by its plaso name only, so the
        //first row failed ("Object reference not set")
        var file = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestFiles", name);
        var p = new SuperTimeline();
        p.ExpectedHeaders.Should().Contain(File.ReadLines(file).First());

        p.ProcessFile(file);

        p.DataList.Count.Should().Be(3);
        var row = (SuperTimelineData) p.DataList[0];
        row.Timestamp.Should().Be(new DateTime(2020, 1, 2, 3, 4, 5, DateTimeKind.Utc));
        row.Macb.Should().Be("m...");
        row.SourceName.Should().Be("FILE");
        row.LongDescription.Should().Be("C:/Users/a/x.lnk");
        row.Inode.Should().Be(123);
        row.Color.Should().Be("FileOpening");
        ((SuperTimelineData) p.DataList[1]).Color.Should().Be("WebHistory");
        ((SuperTimelineData) p.DataList[2]).Color.Should().Be("LogFile");
    }

    [Test]
    public void HandParsedPluginsMatchHeadersIgnoringCase()
    {
        //the plugins that don't use CsvLoader matched their columns by exact case: a header line TLE matched (it
        //ignores case) could fail or leave columns empty
        var mac = new MacTime();
        mac.ProcessFile(Write("mactime_lower.csv", "date,size,type,mode,uid,gid,meta,file name\r\nMon Apr 02 2012 00:46:02,1938,m...,r/rrwxrwxrwx,0,0,51206-128-4,C:/x.pf\r\n"));
        var m = (MacTimeData) mac.DataList[0];
        m.FileSize.Should().Be(1938);
        m.Meta.Should().Be("51206-128-4");
        m.FileName.Should().Be("C:/x.pf");

        var shim = new ShimcacheParser();
        shim.ProcessFile(Write("shim_lower.csv", "last modified,last update,path,file size,exec flag\r\n2020-01-02 03:04:05,N/A,C:\\x.exe,12,True\r\n"));
        var sh = (ShimcacheParserData) shim.DataList[0];
        sh.FilePath.Should().Be(@"C:\x.exe");
        sh.Executed.Should().BeTrue();

        var sig = new Sigcheck();
        sig.ProcessFile(Write("sig_lower.csv", "path,verified,date,publisher,company,description,product,product version,file version,machine type,md5,sha1,pesha1,pesha256,sha256,imp,vt detection,vt link\r\nC:\\x.exe,Signed,10:00 AM 1/2/2020,MS,MS,d,p,1.0,2.0,64-bit,a,b,c,d,e,f,0|70,http://x\r\n"));
        var sg = (SigcheckData) sig.DataList[0];
        sg.Verified.Should().BeTrue();
        sg.FileVersion.Should().Be("2.0");
    }
}
