using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using ITLEFileSpec;
using Serilog;
using Serilog.Events;
using TLEFilePlugins.Shared;

namespace TLEFileTimelines
{
    internal static class CsvReaderExtensions
    {
        /// <summary>
        ///     The field of the column called <paramref name="name" /> (matched ignoring case), or null when the header or
        ///     the record doesn't have it (CsvHelper 33's GetField with MissingFieldFound off)
        /// </summary>
        public static string GetOrNull(this CsvReader csv, string name) =>
            csv.TryGetFieldIndex(name, out var i) && i < csv.FieldCount ? csv.GetString(i) : null;
    }

    /// <summary>
    ///     mactime's Date (the first column): parsed with the invariant culture, a time without a zone taken as UTC, as
    ///     UTC; null when it doesn't parse
    /// </summary>
    internal sealed class MacTimeTimestampConverter : ICsvConverter<DateTime?>
    {
        public DateTime? Read(CsvReader reader, int index) =>
            DateTime.TryParse(reader.GetString(index), CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var outDate)
                ? outDate.ToUniversalTime()
                : null;

        public void Write(CsvWriter writer, DateTime? value) => writer.WriteField(value?.ToString("O", CultureInfo.InvariantCulture) ?? string.Empty);
    }

    /// <summary>
    ///     DateTime.Parse with the invariant culture and AssumeUniversal (a time without a zone is UTC, given as the local
    ///     time), as CsvHelper 33 read the KAPE mini timeline's Time with that style
    /// </summary>
    internal sealed class AssumeUniversalTimeConverter : ICsvConverter<DateTime>
    {
        public DateTime Read(CsvReader reader, int index) => DateTime.Parse(reader.GetString(index), CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);

        public void Write(CsvWriter writer, DateTime value) => writer.WriteField(value.ToString("O", CultureInfo.InvariantCulture));
    }

    [CsvRecord]
    public partial class ForensicTimelineData : IFileSpecData
    {
        public DateTime DateTime { get; set; }
        public string TimestampInfo { get; set; }
        public string ArtifactName { get; set; }
        public string Tool { get; set; }
        public string Description { get; set; }
        public string DataDetails { get; set; }
        public string DataPath { get; set; }
        public string FileExtension { get; set; }
        public string EvidencePath { get; set; }
        public string EventId { get; set; }
        public string User { get; set; }
        public string Computer { get; set; }
        public long? FileSize { get; set; }
        public string IPAddress { get; set; }
        public string SourceAddress { get; set; }
        public string DestinationAddress { get; set; }
        public string SHA1 { get; set; }
        public string Count { get; set; }
        public string RawData { get; set; }

        [CsvIgnore] public int Line { get; set; }
        [CsvIgnore] public bool Tag { get; set; }
        [CsvIgnore] public string Color { get; set; }

        public override string ToString()
        {
            return $"{DateTime} {TimestampInfo} {ArtifactName} {Tool} {Description} {DataDetails} {DataPath} {FileExtension} {EvidencePath} {EventId} {User} {Computer} {FileSize} {IPAddress} {SourceAddress} {DestinationAddress} {SHA1} {Count} {RawData}";
        }
    }

    public class ForensicTimeline : IFileSpec
    {

        public ForensicTimeline()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<ForensicTimelineData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "DateTime,TimestampInfo,ArtifactName,Tool,Description,DataDetails,DataPath,FileExtension,EventId,User,Computer,FileSize,IPAddress,SourceAddress,DestinationAddress,SHA1,Count,EvidencePath,RawData"
        };

        }

        public string Author => "Brad Roughan (@acquiredsecurity)";
        public string FileDescription => "CSV generated for ForensicTimeliner";
        public HashSet<string> ExpectedHeaders { get; }
        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }
        public string InternalGuid => "40ef1341-42af-4612-a480-9a021e2a3353";

        public void ProcessFile(string filename)
        {
            //(TaggedLines.Contains was a list scan for every row)
            var tagged = new HashSet<int>(TaggedLines);

            var ln = 1;
            //the columns are matched ignoring case, as TLE matches header lines (a lower-case header failed); Color is set here
            using var csv = CsvReader.FromFile(filename, CsvLoader.Options);

            try
            {
                DataList.Clear();

                var records = csv.GetRecords<ForensicTimelineData>();

                //(RawRecord was built for every row even with debug messages off)
                var debug = Log.IsEnabled(LogEventLevel.Debug);

                //one reset at the end instead of a ListChanged event for every row
                using var quiet = CsvLoader.QuietList<ForensicTimelineData>(DataList);

                foreach (var record in records)
                {
                    if (debug)
                    {
                        Log.Debug("Line # {Line}, Record: {RawRecord}", ln, csv.Current.ToString());
                    }

                    record.Line = ln;
                    record.Tag = tagged.Contains(ln);

                    // SIMPLE COLOR LOGIC
                    if (record.ArtifactName != null)
                    {
                        if (record.ArtifactName.Contains("LNK") ||
                            record.ArtifactName.Contains("JumpLists") ||
                            record.ArtifactName.Contains("WindowsTimelineActivity") ||
                            record.ArtifactName.Contains("Shellbags"))
                        {
                            record.Color = "FileFolderOpening";
                        }
                        else if (record.ArtifactName.Contains("FileDeletion"))
                        {
                            record.Color = "DeletedData";
                        }
                        else if (record.ArtifactName.Contains("AppCompatCache") ||
                                 record.ArtifactName.Contains("Prefetch") ||
                                 record.ArtifactName.Contains("Amcache") ||
                                 record.ArtifactName.Contains("MFT") ||
                                 record.ArtifactName.Contains("Registry"))
                        {
                            record.Color = "Execution";
                        }
                        else if (record.ArtifactName.Contains("Web History"))
                        {
                            record.Color = "WebHistory";
                        }
                        else if (record.ArtifactName.Contains("Event Logs"))
                        {
                            record.Color = "LogFile";
                        }
                    }


                    DataList.Add(record);
                    ln++;
                }
            }
            catch (Exception e)
            {
                throw new Exception(
                    $"Error loading data on line '{ln}': {e.Message}. Line: {csv.Current.ToString()}", e);
            }
        }
    }

    public class PsortTimelineData : IFileSpecData
    {
        public PsortTimelineData(int line, string timestamp, string timestampDescription, string source, string sourceLong, string message, string parser, string displayName, string tagInfo)
        {
            Line = line;
            TimestampDescription = timestampDescription;
            Source = source;
            SourceLong = sourceLong;
            Message = message;
            Parser = parser;
            DisplayName = displayName;
            TagInfo = tagInfo;

            //(an exception for every row without a time was slow)
            Timestamp = DateTime.TryParse(timestamp, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var ts)
                ? ts.ToUniversalTime()
                : DateTime.MinValue;

            TimestampDescription = timestampDescription;
            Source = source;
            SourceLong = sourceLong;
            Message = message;
            Parser = parser;
            DisplayName = displayName;
            TagInfo = tagInfo;

            if (sourceLong == null)
            {
                return;
            }

            if (sourceLong.Contains("RECYCLE") ||
                sourceLong.Contains("DELETED") ||
                sourceLong.Contains("Deleted Registry") ||
                sourceLong.Contains("$Recycle.Bin"))
            {
                Color = "DeletedData";
            }

            if (sourceLong.Contains("Expiration Time") ||
                sourceLong.Contains("Cookie") ||
                sourceLong.Contains("Visited") ||
                sourceLong.Contains("URL") && source.Contains("FILE") == false ||
                sourceLong.Contains("Flash Cookie") ||
                sourceLong.Contains("LSO") && source.Contains("REG") == false ||
                sourceLong.IndexOf("http://", StringComparison.OrdinalIgnoreCase) >= 0 ||
                sourceLong.IndexOf("https://", StringComparison.OrdinalIgnoreCase) >= 0 ||
                sourceLong.Contains("Location:") ||
                sourceLong.Contains("time(s) [HTTP") ||
                sourceLong.Contains("Last Visited Time") ||
                source.StartsWith("WEBHIST", StringComparison.Ordinal))
            {
                Color = "WebHistory";
            }


            if (sourceLong.Contains("lnk/shell_items") ||
                source.Contains("File entry shell item") ||
                sourceLong.Contains("BagMRU") ||
                sourceLong.Contains("ShellNoRoam/Bags"))
            {
                Color = "FolderOpening";
            }

            if (sourceLong.Contains("visited file://") ||
                sourceLong.Contains("CreateDate") ||
                sourceLong.Contains("URL:file://") ||
                sourceLong.Contains("File Opened") ||
                sourceLong.Contains("Folder opened") ||
                sourceLong.Contains("Shortcut LNK") ||
                sourceLong.Contains("RecentDocs key") ||
                sourceLong.Contains("Link target:") ||
                sourceLong.Contains("File attribute flags") ||
                sourceLong.Contains("Birth droid volume identifier:") ||
                sourceLong.Contains("UserAssist entry") ||
                sourceLong.EndsWith(".lnk", StringComparison.OrdinalIgnoreCase) ||
                sourceLong.Contains("Recently opened file") ||
                sourceLong.Contains("file of extension") ||
                sourceLong.Contains("Recently") && source.Contains("Firefox") == false ||
                sourceLong.EndsWith("LNK", StringComparison.Ordinal) ||
                sourceLong.Contains("file://") && source.Contains("Firefox") == false ||
                sourceLong.Contains("RecentDocs"))
            {
                Color = "FileOpening";
            }

            if (sourceLong.Contains("MountPoints2") ||
                sourceLong.Contains("volume mounted") ||
                sourceLong.Contains("USB") ||
                sourceLong.Contains("/USB/Vid_") ||
                sourceLong.Contains("Enum/USBSTOR/Disk") ||
                sourceLong.Contains("RemovableMedia") ||
                sourceLong.Contains("STORAGE/RemovableMedia") ||
                sourceLong.Contains("drive mounted") ||
                sourceLong.Contains("Drive last mounted") ||
                sourceLong.Contains("SetupAPI Log"))
            {
                Color = "Device|USBUsage";
            }

            if (sourceLong.Contains("EVT") ||
                sourceLong.Contains("XP Firewall Log") ||
                sourceLong.Contains("Event Level:") ||
                source.StartsWith("EVT", StringComparison.Ordinal)
            )
            {
                Color = "LogFile";
            }

            if (sourceLong.Contains("Prefetch {") ||
                sourceLong.Contains("AppCompatCache") ||
                sourceLong.Contains(@"\Software\Sysinternals") ||
                sourceLong.Contains("typed the following cmd") ||
                sourceLong.Contains("CMD typed") ||
                sourceLong.Contains("Last run") ||
                sourceLong.Contains("RunMRU") ||
                sourceLong.Contains("MUICache") ||
                sourceLong.Contains("UserAssist key") ||
                sourceLong.Contains("Time of Launch") ||
                sourceLong.Contains("Prefetch") ||
                sourceLong.Contains("SHIMCACHE") ||
                sourceLong.Contains("Scheduled") ||
                sourceLong.EndsWith(".pf", StringComparison.OrdinalIgnoreCase) ||
                sourceLong.Contains("was run") ||
                sourceLong.Contains("UEME_") ||
                sourceLong.StartsWith("[PROCESS]", StringComparison.Ordinal)
            )
            {
                Color = "Execution";
            }
        }


        public DateTime Timestamp { get; }
        public string TimestampDescription { get; }
        public string Source { get; }
        public string SourceLong { get; }
        public string Message { get; }
        public string Parser { get; }
        public string DisplayName { get; }
        public string TagInfo { get; }

        public string Color { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return
                $"{Timestamp} {TimestampDescription} {Source} {SourceLong} {Message} {Parser} {DisplayName} {TagInfo}";
        }
    }





    public class PsortTimeline : IFileSpec
    {
        public PsortTimeline()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<PsortTimelineData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "datetime,timestamp_desc,source,source_long,message,parser,display_name,tag"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from Plaso in psort format";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "40ee3432-12eb-4612-a480-9a021e2a3353";

        public void ProcessFile(string filename)
        {
            //(TaggedLines.Contains was a list scan for every row)
            var tagged = new HashSet<int>(TaggedLines);

            DataList.Clear();

            //columns by name, matched ignoring case as TLE matches header lines (a lower-case header failed). No quotes: a
            //double quote escapes the character after it (CsvHelper 33's Escape mode with its default escape character)
            using var csv = CsvReader.FromFile(filename, CsvLoader.Options.With(
                (nameof(CsvReaderOptions.Mode), CsvMode.Escape),
                (nameof(CsvReaderOptions.Escape), '"'),
                (nameof(CsvReaderOptions.OnMalformedRecord), (Action<CsvMalformedRecord>) (record =>
                    Log.Warning("Bad data found ({Reason}) in record {Record}! Raw data: {RawRecord}", record.Reason, record.RecordNumber, record.RawRecord)))));



            //one string for a value repeated in many rows (PoolStrings alone stops pooling most columns of a large file)
            var pool = new StringPool();

            var ln = 1;

            //(RawRecord was built for every row even with debug messages off)
            var debug = Log.IsEnabled(LogEventLevel.Debug);

            //one reset at the end instead of a ListChanged event for every row
            using var quiet = CsvLoader.QuietList<PsortTimelineData>(DataList);

            try
            {
                while (csv.Read())
                {
                    if (debug)
                    {
                        Log.Debug("Line # {Line}, Record: {RawRecord}", ln, csv.Current.ToString());
                    }

                    // "datetime,timestamp_desc,source,source_long,message,parser,display_name,tag"
                    // var dt = csv.GetString("date");
                    //
                    // if (dt.StartsWith("Processing"))
                    // {
                    //     break;
                    // }

                    var dt = pool.Get(csv.GetOrNull("datetime"));
                    var tsD = pool.Get(csv.GetOrNull("timestamp_desc"));
                    var source = pool.Get(csv.GetOrNull("source"));
                    var sourceLong = pool.Get(csv.GetOrNull("source_long"));
                    var message = pool.Get(csv.GetOrNull("message"));
                    var parser = pool.Get(csv.GetOrNull("parser"));
                    var displayName = pool.Get(csv.GetOrNull("display_name"));
                    var tag = pool.Get(csv.GetOrNull("tag"));


                    var psd = new PsortTimelineData(ln, dt, tsD, source, sourceLong, message, parser, displayName, tag);
                    psd.Tag = tagged.Contains(ln);
                    DataList.Add(psd);
                    ln += 1;
                }
            }
            catch (Exception e)
            {
                throw new Exception(
                    $"Error loading data on line '{ln}': {e.Message}. Line: {csv.Current.ToString()}", e);
            }
        }
    }


    //3333333333333333333333333333333333333333


    public class SuperTimelineData : IFileSpecData
    {
        public SuperTimelineData(int line, string date, string time, string tz, string macb, string sourceName,
            string sourceDesc,
            string type, string userName, string hostName, string shortDesc, string longDesc, string version, string
                fileName, int inode, string notes, string format, string extra)
        {
            Line = line;

            //(an exception for every row without a time was slow)
            Timestamp = DateTime.TryParseExact($"{date} {time}", "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal, out var timestamp)
                ? timestamp.ToUniversalTime()
                : DateTime.MinValue;


            TimeZone = tz;
            var macbLower = macb.ToLowerInvariant();

            var hasM = macbLower.Contains("m") ? 1 : 0;
            var hasA = macbLower.Contains("a") ? 2 : 0;
            var hasC = macbLower.Contains("c") ? 4 : 0;
            var hasB = macbLower.Contains("b") ? 8 : 0;


            Macb = macbLower; //tst.ToString();

            SourceName = sourceName;
            SourceDescription = sourceDesc;
            Type = type;
            Username = userName;
            HostName = hostName;
            ShortDescription = shortDesc;
            LongDescription = longDesc;
            Version = version;
            FileName = fileName;
            Inode = inode;
            Notes = notes;
            Format = format;
            Extra = extra;

            if (longDesc == null)
            {
                return;
            }

            if (longDesc.Contains("RECYCLE") ||
                longDesc.Contains("DELETED") ||
                longDesc.Contains("Deleted Registry") ||
                longDesc.Contains("$Recycle.Bin"))
            {
                Color = "DeletedData";
            }

            if (longDesc.Contains("Expiration Time") ||
                longDesc.Contains("Cookie") ||
                longDesc.Contains("Visited") ||
                longDesc.Contains("URL") && sourceName.Contains("FILE") == false ||
                longDesc.Contains("Flash Cookie") ||
                longDesc.Contains("LSO") && sourceName.Contains("REG") == false ||
                //(ignoring case without a lower case copy of the description for each test)
                longDesc.IndexOf("http://", StringComparison.OrdinalIgnoreCase) >= 0 ||
                longDesc.IndexOf("https://", StringComparison.OrdinalIgnoreCase) >= 0 ||
                longDesc.Contains("Location:") ||
                longDesc.Contains("time(s) [HTTP") ||
                longDesc.Contains("Last Visited Time") ||
                sourceName.StartsWith("WEBHIST", StringComparison.Ordinal))
            {
                Color = "WebHistory";
            }


            if (longDesc.Contains("lnk/shell_items") ||
                sourceDesc.Contains("File entry shell item") ||
                longDesc.Contains("BagMRU") ||
                longDesc.Contains("ShellNoRoam/Bags"))
            {
                Color = "FolderOpening";
            }

            if (longDesc.Contains("visited file://") ||
                longDesc.Contains("CreateDate") ||
                longDesc.Contains("URL:file://") ||
                longDesc.Contains("File Opened") ||
                longDesc.Contains("Folder opened") ||
                longDesc.Contains("Shortcut LNK") ||
                longDesc.Contains("RecentDocs key") ||
                longDesc.Contains("Link target:") ||
                longDesc.Contains("File attribute flags") ||
                longDesc.Contains("Birth droid volume identifier:") ||
                longDesc.Contains("UserAssist entry") ||
                longDesc.EndsWith(".lnk", StringComparison.OrdinalIgnoreCase) ||
                longDesc.Contains("Recently opened file") ||
                longDesc.Contains("file of extension") ||
                longDesc.Contains("Recently") && sourceDesc.Contains("Firefox") == false ||
                longDesc.EndsWith("LNK", StringComparison.Ordinal) ||
                longDesc.Contains("file://") && sourceDesc.Contains("Firefox") == false ||
                longDesc.Contains("RecentDocs"))
            {
                Color = "FileOpening";
            }

            if (longDesc.Contains("MountPoints2") ||
                longDesc.Contains("volume mounted") ||
                longDesc.Contains("USB") ||
                longDesc.Contains("/USB/Vid_") ||
                longDesc.Contains("Enum/USBSTOR/Disk") ||
                longDesc.Contains("RemovableMedia") ||
                longDesc.Contains("STORAGE/RemovableMedia") ||
                longDesc.Contains("drive mounted") ||
                longDesc.Contains("Drive last mounted") ||
                longDesc.Contains("SetupAPI Log"))
            {
                Color = "Device|USBUsage";
            }

            if (longDesc.Contains("EVT") ||
                longDesc.Contains("XP Firewall Log") ||
                longDesc.Contains("Event Level:") ||
                sourceName.StartsWith("EVT", StringComparison.Ordinal)
            )
            {
                Color = "LogFile";
            }

            if (longDesc.Contains("Prefetch {") ||
                longDesc.Contains("AppCompatCache") ||
                longDesc.Contains(@"\Software\Sysinternals") ||
                longDesc.Contains("typed the following cmd") ||
                longDesc.Contains("CMD typed") ||
                longDesc.Contains("Last run") ||
                longDesc.Contains("RunMRU") ||
                longDesc.Contains("MUICache") ||
                longDesc.Contains("UserAssist key") ||
                longDesc.Contains("Time of Launch") ||
                longDesc.Contains("Prefetch") ||
                longDesc.Contains("SHIMCACHE") ||
                longDesc.Contains("Scheduled") ||
                longDesc.EndsWith(".pf", StringComparison.OrdinalIgnoreCase) ||
                longDesc.Contains("was run") ||
                longDesc.Contains("UEME_") ||
                longDesc.StartsWith("[PROCESS]", StringComparison.Ordinal)
            )
            {
                Color = "Execution";
            }
        }

        public DateTime Timestamp { get; }
        public string TimeZone { get; }
        public string Macb { get; }
        public string SourceName { get; }
        public string SourceDescription { get; }
        public string Type { get; }
        public string Username { get; }
        public string HostName { get; }
        public string ShortDescription { get; }
        public string LongDescription { get; }
        public string Version { get; }
        public string FileName { get; }
        public int Inode { get; }
        public string Notes { get; }
        public string Format { get; }
        public string Extra { get; }

        public string Color { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return
                $"{Timestamp} {TimeZone} {Macb} {SourceName} {ShortDescription} {SourceDescription} {Type} {Username} {HostName} {ShortDescription} {LongDescription} {Version} {FileName} {Inode} {Notes} {Format} {Extra}";
        }
    }

    public class SuperTimeline : IFileSpec
    {
        public SuperTimeline()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<SuperTimelineData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "date,time,timezone,macb,source,sourcetype,type,user,host,short,desc,version,filename,inode,notes,format,extra"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from Plaso in L2TCsv format";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "40dd7405-16cf-4612-a480-9a021e2a3353";

        public void ProcessFile(string filename)
        {
            //(TaggedLines.Contains was a list scan for every row)
            var tagged = new HashSet<int>(TaggedLines);

            DataList.Clear();

            //columns by name, matched ignoring case as TLE matches header lines (a lower-case header failed); bad quoting is recovered from
            using var csv = CsvReader.FromFile(filename, CsvLoader.Options);



            //one string for a value repeated in many rows (PoolStrings alone stops pooling most columns of a large file)
            var pool = new StringPool();

            var ln = 1;

            //(RawRecord was built for every row even with debug messages off)
            var debug = Log.IsEnabled(LogEventLevel.Debug);

            //one reset at the end instead of a ListChanged event for every row
            using var quiet = CsvLoader.QuietList<SuperTimelineData>(DataList);

            try
            {
                while (csv.Read())
                {
                    if (debug)
                    {
                        Log.Debug("Line # {Line}, Record: {RawRecord}", ln, csv.Current.ToString());
                    }

                    //"date,time,timezone,macb,source,sourcetype,type,user,host,short,desc,version,filename,inode,notes,format,extra":
                    var dt = pool.Get(csv.GetOrNull("date"));

                    if (dt.StartsWith("Processing", StringComparison.Ordinal))
                    {
                        break;
                    }

                    var time = pool.Get(csv.GetOrNull("time"));
                    var tz = pool.Get(csv.GetOrNull("timezone"));
                    var macb = pool.Get(csv.GetOrNull("MACB"));
                    var sourceName = pool.Get(csv.GetOrNull("source"));
                    var sourceDesc = pool.Get(csv.GetOrNull("sourcetype"));
                    var type = pool.Get(csv.GetOrNull("type"));
                    var username = pool.Get(csv.GetOrNull("user"));
                    var hostname = pool.Get(csv.GetOrNull("host"));
                    var shortDesc = pool.Get(csv.GetOrNull("short"));
                    var longDesc = pool.Get(csv.GetOrNull("desc"));
                    var version = pool.Get(csv.GetOrNull("version"));
                    var filename1 = pool.Get(csv.GetOrNull("filename"));
                    var inodeRaw = pool.Get(csv.GetOrNull("inode"));
                    var notes = pool.Get(csv.GetOrNull("notes"));
                    var format = pool.Get(csv.GetOrNull("format"));
                    var extra = pool.Get(csv.GetOrNull("extra"));

                    int.TryParse(inodeRaw, out var inode);

                    var lfe = new SuperTimelineData(ln, dt, time, tz, macb, sourceName, sourceDesc, type,
                        username, hostname, shortDesc, longDesc, version, filename1, inode, notes, format,
                        extra);
                    lfe.Tag = tagged.Contains(ln);
                    DataList.Add(lfe);
                    ln += 1;
                }
            }
            catch (Exception e)
            {
                throw new Exception(
                    $"Error loading data on line '{ln}': {e.Message}. Line: {csv.Current.ToString()}", e);
            }
        }
    }

    [CsvRecord]
    public partial class MacTimeData : IFileSpecData
    {
        [Flags]
        public enum TimestampType
        {
            Modified = 0x1,
            Accessed = 0x2,
            Changed = 0x4,
            Born = 0x8
        }

        public MacTimeData()
        {
        }

        public MacTimeData(int line, DateTime? timestamp, long fileSize, string macb, string permissions,
            string uid,
            string gid,
            string meta, string filename)
        {
            //Mon Apr 02 2012 00:46:02,1938,m...,r/rrwxrwxrwx,0,0,51206-128-4,"C:/Users/nromanoff/AppData/LocalLow/Microsoft/CryptnetUrlCache/Content/F4B372709D6C2AD766C34D274501DC76_516445E2D2E0044FF0510B085B354A0C"

            Line = line;

            Timestamp = timestamp;

            FileSize = fileSize;

            Macb = macb; //tst.ToString();
            Permissions = permissions;
            UId = uid;
            GId = gid;
            Meta = meta;
            FileName = filename.TrimStart('"').TrimEnd('"');
        }

        [CsvColumn(Index = 0), CsvConverter(typeof(MacTimeTimestampConverter))] public DateTime? Timestamp { get; set; }


        [CsvColumn("Size")] public long FileSize { get; set; }
        [CsvColumn("Type")] public string Macb { get; set; }
        [CsvColumn("Mode")] public string Permissions { get; set; }
        [CsvColumn("UID")] public string UId { get; set; }
        [CsvColumn("GID")] public string GId { get; set; }
        [CsvColumn("Meta")] public string Meta { get; set; }
        [CsvColumn("File Name")] public string FileName { get; set; }


        [CsvIgnore] public string Color { get; set; }

        [CsvIgnore] public int Line { get; set; }
        [CsvIgnore] public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{Timestamp} {Macb} {Permissions} {UId} {GId} {Meta} {FileName} {FileSize}";
        }


        public void UpdateColor()
        {
            if (
                FileName.Contains("$Recycle.Bin"))
            {
                Color = "DeletedData";
            }

            if (FileName.StartsWith("[IEHISTORY]", StringComparison.Ordinal))
            {
                Color = "WebHistory";
            }


            if (FileName.IndexOf(".lnk", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                Color = "FileOpening";
            }

            if (FileName.StartsWith("[SHIMCACHE]", StringComparison.Ordinal) ||
                FileName.EndsWith(".pf", StringComparison.OrdinalIgnoreCase) ||
                FileName.StartsWith(@"[USER ASSIST]", StringComparison.Ordinal) ||
                FileName.StartsWith("[PROCESS]", StringComparison.Ordinal))
            {
                Color = "Execution";
            }
        }
    }

    public class MacTime : IFileSpec
    {
        public MacTime()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<MacTimeData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "date,size,type,mode,uid,gid,meta,file name"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from bodyfile and mactime";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "30877d57-540d-4460-bc4e-859fee4b0aac";

        public void ProcessFile(string filename)
        {
            //(TaggedLines.Contains was a list scan for every row)
            var tagged = new HashSet<int>(TaggedLines);

            DataList.Clear();

            //the columns are matched ignoring case, as TLE matches header lines (a lower-case header failed)
            using var csv = CsvReader.FromFile(filename, CsvLoader.Options);



            var ln = 1;

            //(RawRecord was built for every row even with debug messages off)
            var debug = Log.IsEnabled(LogEventLevel.Debug);

            //one reset at the end instead of a ListChanged event for every row
            using var quiet = CsvLoader.QuietList<MacTimeData>(DataList);

            while (csv.Read())
            {
                if (debug)
                {
                    Log.Debug("Line # {Line}, Record: {RawRecord}", ln, csv.Current.ToString());
                }

                var f = csv.GetRecord<MacTimeData>();
                f.Line = ln;
                f.Tag = tagged.Contains(ln);
                if (f.Timestamp != null && f.Timestamp?.Year == 1)
                {
                    f.Timestamp = null;
                }

                f.UpdateColor();

                DataList.Add(f);

                ln += 1;
            }
        }
    }



    [CsvRecord]
    public partial class KapeMiniTimelineData : IFileSpecData
    {
        [CsvColumn("Time"), CsvConverter(typeof(AssumeUniversalTimeConverter))] public DateTime Timestamp { get; set; }

        [CsvColumn("Type")] public string DataType { get; set; }
        public string ComputerName { get; set; }
        [CsvColumn("User/Source")] public string UserSource { get; set; }
        public string Message { get; set; }


        [CsvIgnore] public int Line { get; set; }
        [CsvIgnore] public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{Timestamp} {DataType} {ComputerName} {UserSource} {Message}";
        }

    }

    public class KapeMiniTimeline : IFileSpec
    {
        public KapeMiniTimeline()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<KapeMiniTimelineData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Time,Type,ComputerName,User/Source,Message"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from KAPE Mini timeline module";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "12855d57-540d-4460-bc4e-859fee4b0aac";

        public void ProcessFile(string filename)
        {
            //(TaggedLines.Contains was a list scan for every row)
            var tagged = new HashSet<int>(TaggedLines);

            DataList.Clear();

            //the columns are matched ignoring case, as TLE matches header lines (a lower-case header failed)
            using var csv = CsvReader.FromFile(filename, CsvLoader.Options);

            var ln = 1;

            //(RawRecord was built for every row even with debug messages off)
            var debug = Log.IsEnabled(LogEventLevel.Debug);

            //one reset at the end instead of a ListChanged event for every row
            using var quiet = CsvLoader.QuietList<KapeMiniTimelineData>(DataList);

            while (csv.Read())
            {
                if (debug)
                {
                    Log.Debug("Line # {Line}, Record: {RawRecord}", ln, csv.Current.ToString());
                }

                var testStr = csv.GetString(0);

                if (DateTime.TryParse(testStr, out var s) == false)
                {
                    Log.Warning("Bad data found! Skipping. Raw data: {RawRecord}", csv.Current.ToString());
                    continue;
                }

                var f = csv.GetRecord<KapeMiniTimelineData>();
                f.Line = ln;
                f.Tag = tagged.Contains(ln);


                DataList.Add(f);

                ln += 1;
            }
        }
    }
}