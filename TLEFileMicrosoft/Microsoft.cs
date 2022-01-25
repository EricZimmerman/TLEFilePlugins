using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using ITLEFileSpec;
using Serilog;

namespace TLEFileMicrosoft
{
    public class SigcheckData : IFileSpecData
    {
        public string Path { get; set; }
        public bool Verified { get; set; }
        public DateTime Timestamp { get; set; }

        public string Publisher { get; set; }
        public string Company { get; set; }
        public string Description { get; set; }
        public string Product { get; set; }
        public string ProductVersion { get; set; }
        public string FileVersion { get; set; }
        public string MachineType { get; set; }
        public string MD5 { get; set; }
        public string SHA1 { get; set; }
        public string PESHA1 { get; set; }
        public string PESHA256 { get; set; }
        public string SHA256 { get; set; }
        public string IMP { get; set; }
        public string VTDetection { get; set; }
        public string VTLink { get; set; }

        public int Line { get; set; }

        public bool Tag { get; set; }

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
            DataList.Clear();

            using var fileReader = new StreamReader(filename, Encoding.GetEncoding(1252));
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                BadDataFound = null,
            };

            var csv = new CsvReader(fileReader, config);
                
            var o = new TypeConverterOptions
            {
                DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal
            };
            csv.Context.TypeConverterOptionsCache.AddOptions<SigcheckData>(o);

            var foo = csv.Context.AutoMap<SigcheckData>();

            foo.Map(t => t.Line).Ignore();
            foo.Map(t => t.Tag).Ignore();

            foo.Map(t => t.Verified).Convert(t => t.Row.GetField("Verified") == "Signed");

            foo.Map(t => t.Timestamp).Name("Date");
            foo.Map(t => t.ProductVersion).Name("Product Version");
            foo.Map(t => t.FileVersion).Name("File Version");
            foo.Map(t => t.MachineType).Name("Machine Type");
            foo.Map(t => t.VTDetection).Name("VT detection");
            foo.Map(t => t.VTLink).Name("VT link");

            csv.Context.RegisterClassMap(foo);

                

            var records = csv.GetRecords<SigcheckData>();

            var ln = 1;
            foreach (var sc in records)
            {
                Log.Debug("Line # {Line}, Record: {RawRecord}",ln,csv.Context.Parser.RawRecord);

                sc.Line = ln;
                sc.Tag = TaggedLines.Contains(ln);
                DataList.Add(sc);
                ln += 1;
            }
        }
    }

    public class SigcheckTroyData : IFileSpecData
    {
        public string Path { get; set; }
        public bool Verified { get; set; }
        public DateTime Timestamp { get; set; }

        public string Publisher { get; set; }
        public string Company { get; set; }
        public string Description { get; set; }
        public string Product { get; set; }
        public string ProductVersion { get; set; }
        public string FileVersion { get; set; }
        public string MachineType { get; set; }
        public string BinaryVersion { get; set; }
        public string OriginalName { get; set; }
        public string InternalName { get; set; }
        public string Copyright { get; set; }
        public string Comments { get; set; }
        public string Entropy { get; set; }
        public string MD5 { get; set; }
        public string SHA1 { get; set; }
        public string PESHA1 { get; set; }
        public string PESHA256 { get; set; }
        public string SHA256 { get; set; }
        public string IMP { get; set; }


        public int Line { get; set; }

        public bool Tag { get; set; }

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
            DataList.Clear();

            using var fileReader = new StreamReader(filename, Encoding.GetEncoding(1252));
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                BadDataFound = null,
            };

            var csv = new CsvReader(fileReader, config);
                
            

            var o = new TypeConverterOptions
            {
                DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal
            };
            csv.Context.TypeConverterOptionsCache.AddOptions<SigcheckTroyData>(o);

            var foo = csv.Context.AutoMap<SigcheckTroyData>();

            //path,verified,date,publisher,company,description,product,product version,file version,machine type,binary version,original name,internal name,copyright,comments,entropy,md5,sha1,pesha1,pesha256,sha256,imp

            foo.Map(t => t.Line).Ignore();
            foo.Map(t => t.Tag).Ignore();

            foo.Map(t => t.Verified).Convert(t => t.Row.GetField("Verified") == "Signed");

            foo.Map(t => t.Timestamp).Name("Date");
            foo.Map(t => t.ProductVersion).Name("Product Version");
            foo.Map(t => t.FileVersion).Name("File Version");
            foo.Map(t => t.MachineType).Name("Machine Type");
            foo.Map(t => t.BinaryVersion).Name("Binary Version");
            foo.Map(t => t.OriginalName).Name("Original Name");
            foo.Map(t => t.InternalName).Name("Internal Name");


            csv.Context.RegisterClassMap(foo);

                

            var records = csv.GetRecords<SigcheckTroyData>();

            var ln = 1;
            foreach (var sc in records)
            {
                Log.Debug("Line # {Line}, Record: {RawRecord}",ln,csv.Context.Parser.RawRecord);

                sc.Line = ln;
                sc.Tag = TaggedLines.Contains(ln);
                DataList.Add(sc);
                ln += 1;
            }
        }
    }

    public class AutorunsData : IFileSpecData
    {
        public DateTime? Time { get; set; }
        public string EntryLocation { get; set; }
        public string Entry { get; set; }
        public bool Enabled { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string Signer { get; set; }
        public string Company { get; set; }
        public string Version { get; set; }
        public string ImagePath { get; set; }
        public string LaunchString { get; set; }
        public string MD5 { get; set; }
        public string SHA1 { get; set; }
        public string PESHA1 { get; set; }
        public string SHA256 { get; set; }
        public string PESHA256 { get; set; }
        public string Imp { get; set; }
        public string PSComputerName { get; set; }
        public string RunspaceId { get; set; }
        public string PSShowComputerName { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

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
            DataList.Clear();

            using var fileReader = File.OpenText(filename);
            var csv = new CsvReader(fileReader, CultureInfo.InvariantCulture);
                
            var foo = csv.Context.AutoMap<AutorunsData>();

            //"Time","Entry Location","Entry","Enabled","Category","Profile","Description","Signer","Company",
            //"Image Path","Version","Launch String","MD5","SHA-1","PESHA-1","PESHA-256","SHA-256","IMP",
            //"PSComputerName","RunspaceId","PSShowComputerName"

            foo.Map(t => t.Line).Ignore();
            foo.Map(t => t.Tag).Ignore();

            foo.Map(t => t.EntryLocation).Name("Entry Location");
            foo.Map(t => t.Enabled).Convert(t => t.Row[3] == "enabled");
            foo.Map(t => t.ImagePath).Name("Image Path");
            foo.Map(t => t.LaunchString).Name("Launch String");
            foo.Map(t => t.SHA1).Name("SHA-1");
            foo.Map(t => t.PESHA1).Name("PESHA-1");
            foo.Map(t => t.SHA256).Name("SHA-256");
            foo.Map(t => t.PESHA256).Name("PESHA-256");
            foo.Map(t => t.Imp).Name("IMP");

            csv.Context.RegisterClassMap(foo);

                

            var records = csv.GetRecords<AutorunsData>();

            var ln = 1;
            foreach (var autorunsEntry in records)
            {
                Log.Debug("Line # {Line}, Record: {RawRecord}",ln,csv.Context.Parser.RawRecord);

                autorunsEntry.Line = ln;
                autorunsEntry.Tag = TaggedLines.Contains(ln);
                DataList.Add(autorunsEntry);
                ln += 1;
            }
        }
    }

    public class MsftMftData : IFileSpecData
    {
        public long SegmentNumber { get; set; }
        public bool InUse { get; set; }
        public int ReferenceCount { get; set; }
        public bool IsDirectory { get; set; }
        public bool IsStream { get; set; }
        public string FileName { get; set; }
        public long? FileSize { get; set; }
        public string Extension { get; set; }
        public DateTime? CreationTime0x10 { get; set; }
        public DateTime? LastModificationTime0x10 { get; set; }
        public DateTime? LastChangeTime0x10 { get; set; }
        public DateTime? LastAccessTime0x10 { get; set; }
        public DateTime? LastModificationTime0x30 { get; set; }
        public DateTime? CreationTime0x30 { get; set; }
        public DateTime? LastChangeTime0x30 { get; set; }
        public DateTime? LastAccessTime0x30 { get; set; }
        public string Path { get; set; }
        public int ParentFRS { get; set; }
        public int? OwnerID { get; set; }
        public int? SecurityID { get; set; }
        public bool HasEA { get; set; }
        public bool IsReadOnly { get; set; }
        public bool IsHidden { get; set; }
        public bool IsSystem { get; set; }
        public bool IsArchive { get; set; }
        public bool IsNormal { get; set; }
        public bool IsTemporary { get; set; }
        public bool IsSparse { get; set; }
        public bool IsReparsePoint { get; set; }
        public bool IsCompressed { get; set; }
        public bool IsOffline { get; set; }
        public bool IsNotContentIndexed { get; set; }
        public bool IsEncrypted { get; set; }
        public bool IsIntegrityStream { get; set; }
        public bool IsVirtual { get; set; }
        public bool IsNoScrubData { get; set; }
        public bool IsEA { get; set; }
        public string LoggedUtilityStream { get; set; }
        public int SequenceNumber { get; set; }
        public string ParseErrorMsg { get; set; }

        public int Line { get; set; }

        public bool Tag { get; set; }


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
            DataList.Clear();

            using var fileReader = File.OpenText(filename);
            var csv = new CsvReader(fileReader, CultureInfo.InvariantCulture);
                


            var foo = csv.Context.AutoMap<MsftMftData>();

            var o = new TypeConverterOptions
            {
                DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal
            };

            csv.Context.TypeConverterOptionsCache.AddOptions<MsftMftData>(o);

            foo.Map(t => t.Line).Ignore();
            foo.Map(t => t.Tag).Ignore();

            foo.Map(m => m.CreationTime0x10).Convert(row =>
                DateTime.Parse(row.Row.GetField<string>("CreationTime0x10").Replace("Z", "")));
            foo.Map(m => m.CreationTime0x30).Convert(row =>
                DateTime.Parse(row.Row.GetField<string>("CreationTime0x30").Replace("Z", "")));
            foo.Map(m => m.LastModificationTime0x10).Convert(row =>
                DateTime.Parse(row.Row.GetField<string>("LastModificationTime0x10").Replace("Z", "")));
            foo.Map(m => m.LastModificationTime0x30).Convert(row =>
                DateTime.Parse(row.Row.GetField<string>("LastModificationTime0x30").Replace("Z", "")));
            foo.Map(m => m.LastChangeTime0x10).Convert(row =>
                DateTime.Parse(row.Row.GetField<string>("LastChangeTime0x10").Replace("Z", "")));
            foo.Map(m => m.LastChangeTime0x30).Convert(row =>
                DateTime.Parse(row.Row.GetField<string>("LastChangeTime0x30").Replace("Z", "")));
            foo.Map(m => m.LastAccessTime0x10).Convert(row =>
                DateTime.Parse(row.Row.GetField<string>("LastAccessTime0x10").Replace("Z", "")));
            foo.Map(m => m.LastAccessTime0x30).Convert(row =>
                DateTime.Parse(row.Row.GetField<string>("LastAccessTime0x30").Replace("Z", "")));


            csv.Context.RegisterClassMap(foo);

                

            var records = csv.GetRecords<MsftMftData>();

            var ln = 1;
            foreach (var record in records)
            {
                Log.Debug("Line # {Line}, Record: {RawRecord}",ln,csv.Context.Parser.RawRecord);
                record.Line = ln;
                record.Tag = TaggedLines.Contains(ln);
                DataList.Add(record);

                ln += 1;
            }
        }
    }
}