using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.TypeConversion;
using ITLEFileSpec;
using NLog;

namespace TLEFileTimelines
{
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

            using (var fileReader = File.OpenText(filename))
            {
                var csv = new CsvReader(fileReader, CultureInfo.InvariantCulture);
                csv.Configuration.HasHeaderRecord = true;

                var foo = csv.Configuration.AutoMap<SuperTimelineData>();

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
                csv.Configuration.TypeConverterOptionsCache.AddOptions<SuperTimelineData>(o);

                csv.Configuration.BadDataFound = null;
                csv.Configuration.MissingFieldFound = null;

                csv.Configuration.RegisterClassMap(foo);

                csv.Read();
                csv.ReadHeader();

                var l = LogManager.GetCurrentClassLogger();

                var ln = 1;

                try
                {
                    while (csv.Read())
                    {
                        l.Debug($"Line # {ln}, Record: {csv.Context.RawRecord}");

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
                        $"Error loading data on line '{ln}': {e.Message}. Line: {csv.Context.RawRecord}", e);
                }
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

            using (var fileReader = File.OpenText(filename))
            {
                var csv = new CsvReader(fileReader, CultureInfo.InvariantCulture);
                csv.Configuration.HasHeaderRecord = true;
                var foo = csv.Configuration.AutoMap<MacTimeData>();

                var o = new TypeConverterOptions
                {
                    DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal &
                                    DateTimeStyles.AdjustToUniversal
                };
                csv.Configuration.TypeConverterOptionsCache.AddOptions<MacTimeData>(o);

                foo.Map(t => t.Line).Ignore();
                foo.Map(t => t.Tag).Ignore();
                foo.Map(t => t.Color).Ignore();
                //
                foo.Map(t => t.Timestamp).Name("Date");
                foo.Map(m => m.Timestamp).ConvertUsing(row =>
                    DateTime.TryParse(row.Context.Record[0], CultureInfo.InvariantCulture,
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

                csv.Configuration.RegisterClassMap(foo);

                csv.Read();
                csv.ReadHeader();

                var l = LogManager.GetCurrentClassLogger();

                var ln = 1;
                while (csv.Read())
                {
                    l.Debug($"Line # {ln}, Record: {csv.Context.RawRecord}");

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
    }
}