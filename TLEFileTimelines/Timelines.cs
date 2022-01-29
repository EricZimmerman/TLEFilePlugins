using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using ITLEFileSpec;
using Serilog;

namespace TLEFileTimelines
{
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

            try
            {
                //Timestamp = DateTime.ParseExact($"{date} {time}", "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime();
                Timestamp = DateTime.Parse(timestamp, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime();
            }
            catch (Exception)
            {
                Timestamp = DateTime.MinValue;
            }

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
                sourceLong.ToLowerInvariant().Contains("http://") ||
                sourceLong.ToLowerInvariant().Contains("https://") ||
                sourceLong.Contains("Location:") ||
                sourceLong.Contains("time(s) [HTTP") ||
                sourceLong.Contains("Last Visited Time") ||
                source.StartsWith("WEBHIST"))
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
                sourceLong.ToLowerInvariant().EndsWith(".lnk") ||
                sourceLong.Contains("Recently opened file") ||
                sourceLong.Contains("file of extension") ||
                sourceLong.Contains("Recently") && source.Contains("Firefox") == false ||
                sourceLong.EndsWith("LNK") ||
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
                source.StartsWith("EVT")
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
                sourceLong.ToLowerInvariant().EndsWith(".pf") ||
                sourceLong.Contains("was run") ||
                sourceLong.Contains("UEME_") ||
                sourceLong.StartsWith("[PROCESS]")
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
            DataList.Clear();

            using var fileReader = File.OpenText(filename);
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                BadDataFound = context =>
                {
                    Log.Warning("Bad data found in {Field}! Skipping. Raw data: {RawRecord}",context.Field, context.RawRecord);
                },
                MissingFieldFound = null,
                Mode = CsvMode.Escape
            };


            var csv = new CsvReader(fileReader, config);
         

            var foo = csv.Context.AutoMap<PsortTimelineData>();

            foo.Map(t => t.Line).Ignore();
            foo.Map(t => t.Tag).Ignore();
            foo.Map(t => t.Color).Ignore();
            foo.Map(t => t.Timestamp).Name("datetime");
            foo.Map(t => t.TimestampDescription).Name("timestamp_desc");
            foo.Map(t => t.Source).Name("source");
            foo.Map(t => t.SourceLong).Name("source_long");
            foo.Map(t => t.Message).Name("message");
            foo.Map(t => t.Parser).Name("parser");
            foo.Map(t => t.DisplayName).Name("display_name");
            foo.Map(t => t.TagInfo).Name("tag");
            

            var o = new TypeConverterOptions
            {
                DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal
            };
            csv.Context.TypeConverterOptionsCache.AddOptions<PsortTimelineData>(o);

            csv.Context.RegisterClassMap(foo);

            csv.Read();
            csv.ReadHeader();

                

            var ln = 1;

            try
            {
                while (csv.Read())
                {
                    Log.Debug("Line # {Line}, Record: {RawRecord}",ln,csv.Context.Parser.RawRecord);

                    // "datetime,timestamp_desc,source,source_long,message,parser,display_name,tag"
                    // var dt = csv.GetField("date");
                    //
                    // if (dt.StartsWith("Processing"))
                    // {
                    //     break;
                    // }

                    var dt = csv.GetField("datetime");
                    var tsD = csv.GetField("timestamp_desc");
                    var source = csv.GetField("source");
                    var sourceLong = csv.GetField("source_long");
                    var message = csv.GetField("message");
                    var parser = csv.GetField("parser");
                    var displayName = csv.GetField("display_name");
                    var tag = csv.GetField("tag");
                    

                    var psd = new PsortTimelineData(ln, dt,tsD,source,sourceLong,message,parser,displayName,tag);
                    psd.Tag = TaggedLines.Contains(ln);
                    DataList.Add(psd);
                    ln += 1;
                }
            }
            catch (Exception e)
            {
                throw new Exception(
                    $"Error loading data on line '{ln}': {e.Message}. Line: {csv.Context.Parser.RawRecord}", e);
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

            try
            {
                Timestamp = DateTime.ParseExact($"{date} {time}", "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal).ToUniversalTime();
            }
            catch (Exception)
            {
                Timestamp = DateTime.MinValue;
            }


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
                longDesc.ToLowerInvariant().Contains("http://") ||
                longDesc.ToLowerInvariant().Contains("https://") ||
                longDesc.Contains("Location:") ||
                longDesc.Contains("time(s) [HTTP") ||
                longDesc.Contains("Last Visited Time") ||
                sourceName.StartsWith("WEBHIST"))
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
                longDesc.ToLowerInvariant().EndsWith(".lnk") ||
                longDesc.Contains("Recently opened file") ||
                longDesc.Contains("file of extension") ||
                longDesc.Contains("Recently") && sourceDesc.Contains("Firefox") == false ||
                longDesc.EndsWith("LNK") ||
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
                sourceName.StartsWith("EVT")
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
                longDesc.ToLowerInvariant().EndsWith(".pf") ||
                longDesc.Contains("was run") ||
                longDesc.Contains("UEME_") ||
                longDesc.StartsWith("[PROCESS]")
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
            DataList.Clear();

            using var fileReader = File.OpenText(filename);
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                BadDataFound = null,
                MissingFieldFound = null
            };


            var csv = new CsvReader(fileReader, config);
                

            var foo = csv.Context.AutoMap<SuperTimelineData>();

            foo.Map(t => t.Line).Ignore();
            foo.Map(t => t.Tag).Ignore();
            foo.Map(t => t.Color).Ignore();
            foo.Map(t => t.Timestamp).Name("date");
            foo.Map(t => t.TimeZone).Name("timezone");
            foo.Map(t => t.Macb).Name("macb");
            foo.Map(t => t.SourceName).Name("source");
            foo.Map(t => t.ShortDescription).Name("sourcetype");
            foo.Map(t => t.Timestamp).Name("sourcetype");
            foo.Map(t => t.Type).Name("type");
            foo.Map(t => t.Username).Name("user");
            foo.Map(t => t.HostName).Name("host");
            foo.Map(t => t.ShortDescription).Name("short");
            foo.Map(t => t.LongDescription).Name("desc");
            foo.Map(t => t.Version).Name("version");
            foo.Map(t => t.FileName).Name("filename");
            foo.Map(t => t.Inode).Name("inode");
            foo.Map(t => t.Notes).Name("notes");
            foo.Map(t => t.Format).Name("format");
            foo.Map(t => t.Extra).Name("extra");

            var o = new TypeConverterOptions
            {
                DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal
            };
            csv.Context.TypeConverterOptionsCache.AddOptions<SuperTimelineData>(o);

            csv.Context.RegisterClassMap(foo);

            csv.Read();
            csv.ReadHeader();

                

            var ln = 1;

            try
            {
                while (csv.Read())
                {
                    Log.Debug("Line # {Line}, Record: {RawRecord}",ln,csv.Context.Parser.RawRecord);

                    //"date,time,timezone,macb,source,sourcetype,type,user,host,short,desc,version,filename,inode,notes,format,extra":
                    var dt = csv.GetField("date");

                    if (dt.StartsWith("Processing"))
                    {
                        break;
                    }

                    var time = csv.GetField("time");
                    var tz = csv.GetField("timezone");
                    var macb = csv.GetField("MACB");
                    var sourceName = csv.GetField("source");
                    var sourceDesc = csv.GetField("sourcetype");
                    var type = csv.GetField("type");
                    var username = csv.GetField("user");
                    var hostname = csv.GetField("host");
                    var shortDesc = csv.GetField("short");
                    var longDesc = csv.GetField("desc");
                    var version = csv.GetField("version");
                    var filename1 = csv.GetField("filename");
                    var inodeRaw = csv.GetField("inode");
                    var notes = csv.GetField("notes");
                    var format = csv.GetField("format");
                    var extra = csv.GetField("extra");

                    int.TryParse(inodeRaw, out var inode);

                    var lfe = new SuperTimelineData(ln, dt, time, tz, macb, sourceName, sourceDesc, type,
                        username, hostname, shortDesc, longDesc, version, filename1, inode, notes, format,
                        extra);
                    lfe.Tag = TaggedLines.Contains(ln);
                    DataList.Add(lfe);
                    ln += 1;
                }
            }
            catch (Exception e)
            {
                throw new Exception(
                    $"Error loading data on line '{ln}': {e.Message}. Line: {csv.Context.Parser.RawRecord}", e);
            }
        }
    }

    public class MacTimeData : IFileSpecData
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
            int uid,
            int gid,
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

        public DateTime? Timestamp { get; set; }


        public long FileSize { get; set; }
        public string Macb { get; set; }
        public string Permissions { get; set; }
        public int UId { get; set; }
        public int GId { get; set; }
        public string Meta { get; set; }
        public string FileName { get; set; }


        public string Color { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

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

            if (FileName.StartsWith("[IEHISTORY]"))
            {
                Color = "WebHistory";
            }


            if (FileName.ToLowerInvariant().Contains(".lnk"))
            {
                Color = "FileOpening";
            }

            if (FileName.StartsWith("[SHIMCACHE]") ||
                FileName.ToLowerInvariant().EndsWith(".pf") ||
                FileName.StartsWith(@"[USER ASSIST]") ||
                FileName.StartsWith("[PROCESS]"))
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
            DataList.Clear();

            using var fileReader = File.OpenText(filename);
            var csv = new CsvReader(fileReader, CultureInfo.InvariantCulture);
                
            var foo = csv.Context.AutoMap<MacTimeData>();

            var o = new TypeConverterOptions
            {
                DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal &
                                DateTimeStyles.AdjustToUniversal
            };
            csv.Context.TypeConverterOptionsCache.AddOptions<MacTimeData>(o);

            foo.Map(t => t.Line).Ignore();
            foo.Map(t => t.Tag).Ignore();
            foo.Map(t => t.Color).Ignore();
            //
            foo.Map(t => t.Timestamp).Name("Date");
            foo.Map(m => m.Timestamp).Convert(row =>
                DateTime.TryParse(row.Row.Context.Parser.Record[0], CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal, out var outDate)
                    ? outDate.ToUniversalTime()
                    : new DateTime?());
            foo.Map(m => m.Timestamp).TypeConverterOption.DateTimeStyles(DateTimeStyles.AssumeUniversal);

            foo.Map(t => t.FileSize).Name("Size");
            foo.Map(t => t.Macb).Name("Type");
            foo.Map(t => t.Permissions).Name("Mode");
            foo.Map(t => t.UId).Name("UID");
            foo.Map(t => t.GId).Name("GID");
            foo.Map(t => t.Meta).Name("Meta");
            foo.Map(t => t.FileName).Name("File Name");

            csv.Context.RegisterClassMap(foo);

            csv.Read();
            csv.ReadHeader();

                

            var ln = 1;
            while (csv.Read())
            {
                Log.Debug("Line # {Line}, Record: {RawRecord}",ln,csv.Context.Parser.RawRecord);

                var f = csv.GetRecord<MacTimeData>();
                f.Line = ln;
                f.Tag = TaggedLines.Contains(ln);
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



      public class KapeMiniTimelineData : IFileSpecData
    {
        public DateTime Timestamp { get; set; }

        public string DataType { get; set; }
        public string ComputerName { get; set; }
        public string UserSource { get; set; }
        public string Message { get; set; }


        public int Line { get; set; }
        public bool Tag { get; set; }

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
            DataList.Clear();

            using var fileReader = File.OpenText(filename);
            var csvO = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
              
            };

            var csv = new CsvReader(fileReader, csvO);
                
                

            var foo = csv.Context.AutoMap<KapeMiniTimelineData>();

            var o = new TypeConverterOptions
            {
                DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal &
                                DateTimeStyles.AdjustToUniversal
            };
            csv.Context.TypeConverterOptionsCache.AddOptions<KapeMiniTimelineData>(o);

            foo.Map(t => t.Line).Ignore();
            foo.Map(t => t.Tag).Ignore();
             
            foo.Map(t => t.Timestamp).Name("Time");
            foo.Map(t => t.DataType).Name("Type");
             
            foo.Map(m => m.Timestamp).TypeConverterOption.DateTimeStyles(DateTimeStyles.AssumeUniversal);

            foo.Map(t => t.UserSource).Name("User/Source");

            csv.Context.RegisterClassMap(foo);
                
            csv.Read();
            csv.ReadHeader();
            
            var ln = 1;
            while (csv.Read())
            {
                Log.Debug("Line # {Line}, Record: {RawRecord}",ln,csv.Context.Parser.RawRecord);

                var testStr = csv.GetField<string>(0);

                if (DateTime.TryParse(testStr, out var s) == false)
                {
                    Log.Warning("Bad data found! Skipping. Raw data: {RawRecord}",csv.Context.Parser.RawRecord);
                    continue;
                }

                var f = csv.GetRecord<KapeMiniTimelineData>();
                f.Line = ln;
                f.Tag = TaggedLines.Contains(ln);
                 

                DataList.Add(f);

                ln += 1;
            }
        }
    }
}