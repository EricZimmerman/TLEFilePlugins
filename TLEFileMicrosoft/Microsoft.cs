using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using CsvHelper;
using ITLEFileSpec;
using TLEFilePlugins.Shared;
using Serilog;
using Serilog.Events;

namespace TLEFileMicrosoft
{
    /// <summary>Sigcheck's Verified column: true for "Signed"</summary>
    internal sealed class SignedConverter : ICsvConverter<bool>
    {
        public bool Read(CsvReader reader, int index) => reader.GetString(index) == "Signed";

        public void Write(CsvWriter writer, bool value) => writer.WriteField(value ? "Signed" : "Unsigned");
    }

    /// <summary>Autoruns' Enabled column: true for "enabled"</summary>
    internal sealed class EnabledConverter : ICsvConverter<bool>
    {
        public bool Read(CsvReader reader, int index) => reader.GetString(index) == "enabled";

        public void Write(CsvWriter writer, bool value) => writer.WriteField(value ? "enabled" : "disabled");
    }

    [CsvRecord]
    public partial class SigcheckData : IFileSpecData
    {
        public string Path { get; set; }
        [CsvConverter(typeof(SignedConverter))] public bool Verified { get; set; }
        [CsvColumn("Date")] public DateTime Timestamp { get; set; }

        public string Publisher { get; set; }
        public string Company { get; set; }
        public string Description { get; set; }
        public string Product { get; set; }
        [CsvColumn("Product Version")] public string ProductVersion { get; set; }
        [CsvColumn("File Version")] public string FileVersion { get; set; }
        [CsvColumn("Machine Type")] public string MachineType { get; set; }
        public string MD5 { get; set; }
        public string SHA1 { get; set; }
        public string PESHA1 { get; set; }
        public string PESHA256 { get; set; }
        public string SHA256 { get; set; }
        public string IMP { get; set; }
        [CsvColumn("VT detection")] public string VTDetection { get; set; }
        [CsvColumn("VT link")] public string VTLink { get; set; }

        [CsvIgnore] public int Line { get; set; }

        [CsvIgnore] public bool Tag { get; set; }

        public override string ToString()
        {
            return
                $"{Path} {Verified} {Timestamp} {Publisher} {Company} {Description} {Product} {ProductVersion} {FileVersion} {MachineType} {MD5} {SHA1} {PESHA1} {PESHA256} {SHA256} {IMP} {VTDetection} {VTLink}";
        }
    }


    public class Sigcheck : IFileSpec
    {
        public Sigcheck()
        {
            //Initialize collections here, one for TaggedLines TLE can add values to, and the collection that TLE will display
            TaggedLines = new List<int>();

            DataList = new BindingList<SigcheckData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "path,verified,date,publisher,company,description,product,product version,file version,machine type,md5,sha1,pesha1,pesha256,sha256,imp,vt detection,vt link"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from SysInternals Sigcheck";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "40dd7405-16cf-4612-a221-9a050d0a9952";

        public void ProcessFile(string filename)
        {
            //(TaggedLines.Contains was a list scan for every row)
            var tagged = new HashSet<int>(TaggedLines);

            DataList.Clear();

            //Windows-1252 (a byte order mark still wins)
            using var fileReader = new StreamReader(filename, CodePagesEncodingProvider.Instance.GetEncoding(1252));
            //the columns are matched ignoring case, as TLE matches header lines (a lower-case header failed); bad quoting is recovered from
            using var csv = CsvReader.Create(fileReader, CsvLoader.Options);

            var records = csv.GetRecords<SigcheckData>();

            var ln = 1;
            //(RawRecord was built for every row even with debug messages off)
            var debug = Log.IsEnabled(LogEventLevel.Debug);

            //one reset at the end instead of a ListChanged event for every row
            using var quiet = CsvLoader.QuietList<SigcheckData>(DataList);

            foreach (var sc in records)
            {
                if (debug)
                {
                    Log.Debug("Line # {Line}, Record: {RawRecord}", ln, csv.Current.ToString());
                }

                sc.Line = ln;
                sc.Tag = tagged.Contains(ln);
                DataList.Add(sc);
                ln += 1;
            }
        }
    }

    [CsvRecord]
    public partial class SigcheckTroyData : IFileSpecData
    {
        public string Path { get; set; }
        [CsvConverter(typeof(SignedConverter))] public bool Verified { get; set; }
        [CsvColumn("Date")] public DateTime Timestamp { get; set; }

        public string Publisher { get; set; }
        public string Company { get; set; }
        public string Description { get; set; }
        public string Product { get; set; }
        [CsvColumn("Product Version")] public string ProductVersion { get; set; }
        [CsvColumn("File Version")] public string FileVersion { get; set; }
        [CsvColumn("Machine Type")] public string MachineType { get; set; }
        [CsvColumn("Binary Version")] public string BinaryVersion { get; set; }
        [CsvColumn("Original Name")] public string OriginalName { get; set; }
        [CsvColumn("Internal Name")] public string InternalName { get; set; }
        public string Copyright { get; set; }
        public string Comments { get; set; }
        public string Entropy { get; set; }
        public string MD5 { get; set; }
        public string SHA1 { get; set; }
        public string PESHA1 { get; set; }
        public string PESHA256 { get; set; }
        public string SHA256 { get; set; }
        public string IMP { get; set; }


        [CsvIgnore] public int Line { get; set; }

        [CsvIgnore] public bool Tag { get; set; }

        public override string ToString()
        {
            return
                $"{Path} {Verified} {Timestamp} {Publisher} {Company} {Description} {Product} {ProductVersion} {FileVersion} {MachineType} {BinaryVersion} {OriginalName} {InternalName} {Copyright} {Comments} {Entropy} {MD5} {SHA1} {PESHA1} {PESHA256} {SHA256} {IMP}";
        }
    }


    public class SigcheckTroy : IFileSpec
    {
        public SigcheckTroy()
        {
            //Initialize collections here, one for TaggedLines TLE can add values to, and the collection that TLE will display
            TaggedLines = new List<int>();

            DataList = new BindingList<SigcheckTroyData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "path,verified,date,publisher,company,description,product,product version,file version,machine type,binary version,original name,internal name,copyright,comments,entropy,md5,sha1,pesha1,pesha256,sha256,imp"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from customized Sigcheck";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "40dd7405-16cf-4612-a480-1a010d0a1912";

        public void ProcessFile(string filename)
        {
            //(TaggedLines.Contains was a list scan for every row)
            var tagged = new HashSet<int>(TaggedLines);

            DataList.Clear();

            //Windows-1252 (a byte order mark still wins)
            using var fileReader = new StreamReader(filename, CodePagesEncodingProvider.Instance.GetEncoding(1252));
            //the columns are matched ignoring case, as TLE matches header lines (a lower-case header failed); bad quoting is recovered from
            using var csv = CsvReader.Create(fileReader, CsvLoader.Options);

            var records = csv.GetRecords<SigcheckTroyData>();

            var ln = 1;
            //(RawRecord was built for every row even with debug messages off)
            var debug = Log.IsEnabled(LogEventLevel.Debug);

            //one reset at the end instead of a ListChanged event for every row
            using var quiet = CsvLoader.QuietList<SigcheckTroyData>(DataList);

            foreach (var sc in records)
            {
                if (debug)
                {
                    Log.Debug("Line # {Line}, Record: {RawRecord}", ln, csv.Current.ToString());
                }

                sc.Line = ln;
                sc.Tag = tagged.Contains(ln);
                DataList.Add(sc);
                ln += 1;
            }
        }
    }

    [CsvRecord]
    public partial class AutorunsData : IFileSpecData
    {
        public DateTime? Time { get; set; }
        [CsvColumn("Entry Location")] public string EntryLocation { get; set; }
        public string Entry { get; set; }
        [CsvColumn(Index = 3), CsvConverter(typeof(EnabledConverter))] public bool Enabled { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string Signer { get; set; }
        public string Company { get; set; }
        public string Version { get; set; }
        [CsvColumn("Image Path")] public string ImagePath { get; set; }
        [CsvColumn("Launch String")] public string LaunchString { get; set; }
        public string MD5 { get; set; }
        [CsvColumn("SHA-1")] public string SHA1 { get; set; }
        [CsvColumn("PESHA-1")] public string PESHA1 { get; set; }
        [CsvColumn("SHA-256")] public string SHA256 { get; set; }
        [CsvColumn("PESHA-256")] public string PESHA256 { get; set; }
        [CsvColumn("IMP")] public string Imp { get; set; }
        public string PSComputerName { get; set; }
        public string RunspaceId { get; set; }
        public string PSShowComputerName { get; set; }

        [CsvIgnore] public int Line { get; set; }
        [CsvIgnore] public bool Tag { get; set; }

        public override string ToString()
        {
            return
                $"{Time} {EntryLocation} {Entry} {Category} {Description}  {Signer}  {Company}  {Version}  {ImagePath} {LaunchString}  {MD5}  {SHA1}  {SHA256}  {PESHA1}  {PESHA256}  {PSComputerName}  {RunspaceId} {PSShowComputerName} ";
        }
    }


    public class Autoruns : IFileSpec
    {
        public Autoruns()
        {
            //Initialize collections here, one for TaggedLines TLE can add values to, and the collection that TLE will display
            TaggedLines = new List<int>();

            DataList = new BindingList<AutorunsData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "\"time\",\"entry location\",\"entry\",\"enabled\",\"category\",\"profile\",\"description\",\"signer\",\"company\",\"image path\",\"version\",\"launch string\",\"md5\",\"sha-1\",\"pesha-1\",\"pesha-256\",\"sha-256\",\"imp\",\"pscomputername\",\"runspaceid\",\"psshowcomputername\""
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from SysInternals Autoruns";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "42dd2304-16cf-4612-a480-9a050d0a9952";

        public void ProcessFile(string filename)
        {
            //(TaggedLines.Contains was a list scan for every row)
            var tagged = new HashSet<int>(TaggedLines);

            DataList.Clear();

            //the columns are matched ignoring case, as TLE matches header lines (a lower-case header failed)
            using var csv = CsvReader.FromFile(filename, CsvLoader.Options);

            var records = csv.GetRecords<AutorunsData>();

            var ln = 1;
            //(RawRecord was built for every row even with debug messages off)
            var debug = Log.IsEnabled(LogEventLevel.Debug);

            //one reset at the end instead of a ListChanged event for every row
            using var quiet = CsvLoader.QuietList<AutorunsData>(DataList);

            foreach (var autorunsEntry in records)
            {
                if (debug)
                {
                    Log.Debug("Line # {Line}, Record: {RawRecord}", ln, csv.Current.ToString());
                }

                autorunsEntry.Line = ln;
                autorunsEntry.Tag = tagged.Contains(ln);
                DataList.Add(autorunsEntry);
                ln += 1;
            }
        }
    }

    [CsvRecord]
    public partial class MsftMftData : IFileSpecData
    {
        [CsvColumn(Optional = true)] public long SegmentNumber { get; set; }
        [CsvColumn(Optional = true)] public bool InUse { get; set; }
        [CsvColumn(Optional = true)] public int ReferenceCount { get; set; }
        [CsvColumn(Optional = true)] public bool IsDirectory { get; set; }
        [CsvColumn(Optional = true)] public bool IsStream { get; set; }
        [CsvColumn(Optional = true)] public string FileName { get; set; }
        [CsvColumn(Optional = true)] public long? FileSize { get; set; }
        [CsvColumn(Optional = true)] public string Extension { get; set; }
        [CsvColumn(Optional = true), CsvConverter(typeof(NullableNoZParseTimeConverter))] public DateTime? CreationTime0x10 { get; set; }
        [CsvColumn(Optional = true), CsvConverter(typeof(NullableNoZParseTimeConverter))] public DateTime? LastModificationTime0x10 { get; set; }
        [CsvColumn(Optional = true), CsvConverter(typeof(NullableNoZParseTimeConverter))] public DateTime? LastChangeTime0x10 { get; set; }
        [CsvColumn(Optional = true), CsvConverter(typeof(NullableNoZParseTimeConverter))] public DateTime? LastAccessTime0x10 { get; set; }
        [CsvColumn(Optional = true), CsvConverter(typeof(NullableNoZParseTimeConverter))] public DateTime? LastModificationTime0x30 { get; set; }
        [CsvColumn(Optional = true), CsvConverter(typeof(NullableNoZParseTimeConverter))] public DateTime? CreationTime0x30 { get; set; }
        [CsvColumn(Optional = true), CsvConverter(typeof(NullableNoZParseTimeConverter))] public DateTime? LastChangeTime0x30 { get; set; }
        [CsvColumn(Optional = true), CsvConverter(typeof(NullableNoZParseTimeConverter))] public DateTime? LastAccessTime0x30 { get; set; }
        [CsvColumn(Optional = true)] public string Path { get; set; }
        [CsvColumn(Optional = true)] public int ParentFRS { get; set; }
        [CsvColumn(Optional = true)] public int? OwnerID { get; set; }
        [CsvColumn(Optional = true)] public int? SecurityID { get; set; }
        [CsvColumn(Optional = true)] public bool HasEA { get; set; }
        [CsvColumn(Optional = true)] public bool IsReadOnly { get; set; }
        [CsvColumn(Optional = true)] public bool IsHidden { get; set; }
        [CsvColumn(Optional = true)] public bool IsSystem { get; set; }
        [CsvColumn(Optional = true)] public bool IsArchive { get; set; }
        [CsvColumn(Optional = true)] public bool IsNormal { get; set; }
        [CsvColumn(Optional = true)] public bool IsTemporary { get; set; }
        [CsvColumn(Optional = true)] public bool IsSparse { get; set; }
        [CsvColumn(Optional = true)] public bool IsReparsePoint { get; set; }
        [CsvColumn(Optional = true)] public bool IsCompressed { get; set; }
        [CsvColumn(Optional = true)] public bool IsOffline { get; set; }
        [CsvColumn(Optional = true)] public bool IsNotContentIndexed { get; set; }
        [CsvColumn(Optional = true)] public bool IsEncrypted { get; set; }
        [CsvColumn(Optional = true)] public bool IsIntegrityStream { get; set; }
        [CsvColumn(Optional = true)] public bool IsVirtual { get; set; }
        [CsvColumn(Optional = true)] public bool IsNoScrubData { get; set; }
        [CsvColumn(Optional = true)] public bool IsEA { get; set; }
        [CsvColumn(Optional = true)] public string LoggedUtilityStream { get; set; }
        [CsvColumn(Optional = true)] public int SequenceNumber { get; set; }
        [CsvColumn(Optional = true)] public string ParseErrorMsg { get; set; }

        [CsvIgnore] public int Line { get; set; }

        [CsvIgnore] public bool Tag { get; set; }


        public override string ToString()
        {
            return
                $"{SegmentNumber} {InUse} {ReferenceCount} {IsDirectory} {IsStream} {FileName} {FileSize} {Extension} {CreationTime0x10} {LastModificationTime0x10} {LastChangeTime0x10} {LastAccessTime0x10} {CreationTime0x30} {LastModificationTime0x30} {LastChangeTime0x30} {LastAccessTime0x30} {Path} {ParentFRS} {OwnerID} {SecurityID}" +
                $" {HasEA} {IsHidden} {IsSystem} {IsArchive} {IsNormal} {IsTemporary} {IsSparse} {IsReparsePoint} {IsCompressed} {IsOffline} {IsNotContentIndexed} {IsEncrypted} {IsIntegrityStream} {IsVirtual} {IsNoScrubData} {IsEA} {IsReadOnly} {LoggedUtilityStream} {SequenceNumber} {ParseErrorMsg}";
        }
    }


    public class MsftMft : IFileSpec
    {
        public MsftMft()
        {
            //Initialize collections here, one for TaggedLines TLE can add values to, and the collection that TLE will display
            TaggedLines = new List<int>();

            DataList = new BindingList<MsftMftData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "segmentnumber	inuse	referencecount	isdirectory	isstream	filename	filesize	extension	creationtime0x10	lastmodificationtime0x10	lastchangetime0x10	lastaccesstime0x10	lastmodificationtime0x30	creationtime0x30	lastchangetime0x30	lastaccesstime0x30	path	parentfrs	ownerid	securityid	hasea	isreadonly	ishidden	issystem	isarchive	isnormal	istemporary	issparse	isreparsepoint	iscompressed	isoffline	isnotcontentindexed	isencrypted	isintegritystream	isvirtual	isnoscrubdata	isea	loggedutilitystream	sequencenumber	parseerrormsg"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from internal Microsoft tool";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "40dd7405-16cf-4612-c580-9a050d0c1252";

        public void ProcessFile(string filename)
        {
            //tab separated (with the default comma, no file ever loaded)
            CsvLoader.Load<MsftMftData>(filename, DataList, TaggedLines, CsvLoader.Options.With((nameof(CsvReaderOptions.Separator), '\t')));
        }
    }
}