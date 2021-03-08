using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.TypeConversion;
using ITLEFileSpec;
using NLog;

namespace TLEFileMisc
{
    public class AnalyzeMftData : IFileSpecData
    {
        public int RecordNumber { get; set; }
        public bool Good { get; set; }
        public bool Active { get; set; }
        public string RecordType { get; set; }
        public int SequenceNumber { get; set; }
        public long ParentFileRecordNumber { get; set; }
        public int ParentFileRecordSequence { get; set; }

        public string FileName1 { get; set; }
        public DateTime? StdInfoCreated { get; set; }
        public DateTime? StdInfoModified { get; set; }
        public DateTime? StdInfoAccessed { get; set; }
        public DateTime? StdInfoEntryDate { get; set; }
        public DateTime? FileNameCreated { get; set; }
        public DateTime? FileNameModified { get; set; }
        public DateTime? FileNameAccessed { get; set; }
        public DateTime? FileNameEntryDate { get; set; }

        public string ObjectId { get; set; }
        public string BirthVolumeId { get; set; }
        public string BirthObjectId { get; set; }
        public string BirthDomainId { get; set; }


        public string FileName2 { get; set; }
        public DateTime? FileNameCreated2 { get; set; }
        public DateTime? FileNameModified2 { get; set; }
        public DateTime? FileNameAccessed2 { get; set; }
        public DateTime? FileNameEntryDate2 { get; set; }

        public string FileName3 { get; set; }
        public DateTime? FileNameCreated3 { get; set; }
        public DateTime? FileNameModified3 { get; set; }
        public DateTime? FileNameAccessed3 { get; set; }
        public DateTime? FileNameEntryDate3 { get; set; }

        public string FileName4 { get; set; }
        public DateTime? FileNameCreated4 { get; set; }
        public DateTime? FileNameModified4 { get; set; }
        public DateTime? FileNameAccessed4 { get; set; }
        public DateTime? FileNameEntryDate4 { get; set; }


        public bool StandardInformation { get; set; }
        public bool AttributeList { get; set; }
        public bool Filename { get; set; }
        public bool ObjectId2 { get; set; }
        public bool VolumeName { get; set; }
        public bool VolumeInfo { get; set; }
        public bool Data { get; set; }
        public bool IndexRoot { get; set; }

        public bool IndexAllocation { get; set; }
        public bool Bitmap { get; set; }
        public bool ReparsePoint { get; set; }
        public bool EaInformation { get; set; }
        public bool Ea { get; set; }
        public bool PropertySet { get; set; }
        public bool LoggedUtilityStream { get; set; }
        public string LogNotes { get; set; }
        public bool StfFnShift { get; set; }
        public bool uSecZero { get; set; }

        public bool Ads { get; set; }
        public int Line { get; set; }

        public bool Tag { get; set; }

        public override string ToString()
        {
            return
                $"{RecordNumber} {Good} {Active} {RecordType} {SequenceNumber} {ParentFileRecordNumber} {ParentFileRecordSequence} {FileName1} {StdInfoCreated} {StdInfoModified} {StdInfoAccessed} {StdInfoEntryDate} {FileNameCreated} {FileNameModified} {FileNameAccessed} {FileNameEntryDate} {ObjectId} {BirthVolumeId} {BirthObjectId} {BirthDomainId} {FileName2} {FileNameCreated2} {FileNameModified2} {FileNameAccessed2} {FileNameEntryDate2} {FileName3} {FileNameCreated3} {FileNameModified3} {FileNameAccessed3} {FileNameEntryDate3} {FileName4} {FileNameCreated4} {FileNameModified4} {FileNameAccessed4} {FileNameEntryDate4}";
        }
    }


    public class AnalyzeMft : IFileSpec
    {
        public AnalyzeMft()
        {
            //Initialize collections here, one for TaggedLines TLE can add values to, and the collection that TLE will display
            TaggedLines = new List<int>();

            DataList = new BindingList<AnalyzeMftData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "\"record number\",\"good\",\"active\",\"record type\",\"sequence number\",\"parent file rec. #\",\"parent file rec. seq. #\",\"filename #1\",\"std info creation date\",\"std info modification date\",\"std info access date\",\"std info entry date\",\"fn info creation date\",\"fn info modification date\",\"fn info access date\",\"fn info entry date\",\"object id\",\"birth volume id\",\"birth object id\",\"birth domain id\",\"filename #2\",\"fn info creation date\",\"fn info modify date\",\"fn info access date\",\"fn info entry date\",\"filename #3\",\"fn info creation date\",\"fn info modify date\",\"fn info access date\",\"fn info entry date\",\"filename #4\",\"fn info creation date\",\"fn info modify date\",\"fn info access date\",\"fn info entry date\",\"standard information\",\"attribute list\",\"filename\",\"object id\",\"volume name\",\"volume info\",\"data\",\"index root\",\"index allocation\",\"bitmap\",\"reparse point\",\"ea information\",\"ea\",\"property set\",\"logged utility stream\",\"log/notes\",\"stf fn shift\",\"usec zero\",\"ads\""
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from AnalyzeMft";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "7a62735d-cdaf-467a-a1cd-7139d724be19";

        public void ProcessFile(string filename)
        {
            DataList.Clear();

            using (var fileReader = File.OpenText(filename))
            {
                var csv = new CsvReader(fileReader, CultureInfo.InvariantCulture);
                
                var foo = csv.Context.AutoMap<AnalyzeMftData>();

                var o = new TypeConverterOptions
                {
                    DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal
                };
                csv.Context.TypeConverterOptionsCache.AddOptions<AnalyzeMftData>(o);

                foo.Map(t => t.Line).Ignore();
                foo.Map(t => t.Tag).Ignore();

                foo.Map(t => t.RecordNumber).Name("Record Number");
                foo.Map(t => t.Good).Name("Good");
                foo.Map(t => t.Active).Name("Active");
                foo.Map(t => t.RecordType).Name("Record type");
                foo.Map(t => t.SequenceNumber).Name("Sequence Number");
                foo.Map(t => t.ParentFileRecordNumber).Name("Parent File Rec. #");
                foo.Map(t => t.ParentFileRecordSequence).Name("Parent File Rec. Seq. #");
                foo.Map(t => t.FileName1).Name("Filename #1");
                foo.Map(t => t.StdInfoCreated).Name("Std Info Creation date");
                foo.Map(t => t.StdInfoModified).Name("Std Info Modification date");
                foo.Map(t => t.StdInfoAccessed).Name("Std Info Access date");
                foo.Map(t => t.StdInfoEntryDate).Name("Std Info Entry date");
                foo.Map(t => t.FileNameCreated).Name("FN Info Creation date");
                foo.Map(t => t.FileNameModified).Name("FN Info Modification date");
                foo.Map(t => t.FileNameAccessed).Name("FN Info Access date");
                foo.Map(t => t.FileNameEntryDate).Name("FN Info Entry date");
                foo.Map(t => t.ObjectId).Name("Object ID");
                foo.Map(t => t.BirthVolumeId).Name("Birth Volume ID");
                foo.Map(t => t.BirthObjectId).Name("Birth Object ID");
                foo.Map(t => t.BirthVolumeId).Name("Birth Domain ID");

                foo.Map(t => t.FileName2).Name("Filename #2");
                foo.Map(t => t.FileNameCreated2).Name("FN Info Creation date");
                foo.Map(t => t.FileNameModified2).Name("FN Info Modification date");
                foo.Map(t => t.FileNameAccessed2).Name("FN Info Access date");
                foo.Map(t => t.FileNameEntryDate2).Name("FN Info Entry date");

                foo.Map(t => t.FileName3).Name("Filename #3");
                foo.Map(t => t.FileNameCreated3).Name("FN Info Creation date");
                foo.Map(t => t.FileNameModified3).Name("FN Info Modification date");
                foo.Map(t => t.FileNameAccessed3).Name("FN Info Access date");
                foo.Map(t => t.FileNameEntryDate3).Name("FN Info Entry date");

                foo.Map(t => t.FileName4).Name("Filename #4");
                foo.Map(t => t.FileNameCreated4).Name("FN Info Creation date");
                foo.Map(t => t.FileNameModified4).Name("FN Info Modification date");
                foo.Map(t => t.FileNameAccessed4).Name("FN Info Access date");
                foo.Map(t => t.FileNameEntryDate4).Name("FN Info Entry date");

                foo.Map(t => t.StandardInformation).Name("Standard Information");
                foo.Map(t => t.AttributeList).Name("Attribute List");
                foo.Map(t => t.Filename).Name("Filename");
                foo.Map(t => t.ObjectId2).Name("Object ID");
                foo.Map(t => t.VolumeName).Name("Volume Name");
                foo.Map(t => t.VolumeInfo).Name("Volume Info");
                foo.Map(t => t.Data).Name("Data");
                foo.Map(t => t.IndexRoot).Name("Index Root");
                foo.Map(t => t.IndexAllocation).Name("Index Allocation");
                foo.Map(t => t.Bitmap).Name("Bitmap");
                foo.Map(t => t.ReparsePoint).Name("Reparse Point");
                foo.Map(t => t.EaInformation).Name("EA Information");
                foo.Map(t => t.Ea).Name("EA");
                foo.Map(t => t.PropertySet).Name("Property Set");
                foo.Map(t => t.LoggedUtilityStream).Name("Logged Utility Stream");
                foo.Map(t => t.LogNotes).Name("Log/Notes");
                foo.Map(t => t.StfFnShift).Name("STF FN Shift");
                foo.Map(t => t.uSecZero).Name("uSec Zero");
                foo.Map(t => t.Ads).Name("ADS");


                csv.Context.RegisterClassMap(foo);

                var l = LogManager.GetCurrentClassLogger();

                csv.Read();
                csv.ReadHeader();

                var ln = 1;
                while (csv.Read())
                {
                    l.Debug($"Line # {ln}, Record: {csv.Context.Parser.RawRecord}");

                    if (csv.Context.Parser.RawRecord.Contains("BAAD MFT Record"))
                    {
                        continue;
                    }

                    var recNum = int.Parse(csv.GetField(0));
                    var isGood = csv.GetField(1).Equals("Good");
                    var isActive = csv.GetField(2).Equals("Active");
                    var recType = csv.GetField(3);
                    var seqNumber = csv.GetField(4);

                    var parentEntryNum = csv.GetField(5);
                    var parentSeqNum = csv.GetField(6);

                    if (parentSeqNum == "NoParent")
                    {
                        parentSeqNum = "-1";
                    }

                    if (parentEntryNum == "NoParent")
                    {
                        parentEntryNum = "-1";
                    }

                    var fileName1 = csv.GetField(7);

                    var siCreate1 = csv.GetField(8).Trim('=').Trim('"');

                    DateTime? siCreate1D = null;
                    DateTime? siModify1d = null;
                    DateTime? siAccess1d = null;
                    DateTime? siEntry1d = null;

                    if (siCreate1.Equals("NoSIRecord") == false && siCreate1.Length > 0)
                    {
                        siCreate1D = DateTime.Parse(siCreate1,
                            CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime();
                        siModify1d = DateTime.Parse(csv.GetField(9).Trim('=').Trim('"'),
                            CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime();
                        siAccess1d = DateTime.Parse(csv.GetField(10).Trim('=').Trim('"'),
                            CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime();
                        siEntry1d = DateTime.Parse(csv.GetField(11).Trim('=').Trim('"'),
                            CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime();
                    }

                    var fnCreate1 = csv.GetField(12).Trim('=').Trim('"');
                    var fnModify1 = csv.GetField(13).Trim('=').Trim('"');
                    var fnAccess1 = csv.GetField(14).Trim('=').Trim('"');
                    var fnEntry1 = csv.GetField(15).Trim('=').Trim('"');

                    DateTime? fnCreate1d = null;
                    DateTime? fnModify1d = null;
                    DateTime? fnAccess1d = null;
                    DateTime? fnEntry1d = null;

                    if (fnCreate1.Equals("NoFNRecord") == false && fnCreate1.Length > 0)
                    {
                        fnCreate1d = DateTime.Parse(fnCreate1,
                            CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime();

                        fnModify1d = DateTime.Parse(fnModify1,
                            CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime();
                        fnAccess1d = DateTime.Parse(fnAccess1,
                            CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime();
                        fnEntry1d = DateTime.Parse(fnEntry1,
                            CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime();
                    }

                    var objectId = csv.GetField(16);
                    var birthVolId = csv.GetField(17);
                    var birthObjectId = csv.GetField(18);
                    var birthDomainId = csv.GetField(19);

                    var fileName2 = csv.GetField(20);

                    var fnCreate2 = csv.GetField(21).Trim('=').Trim('"');
                    DateTime? fnCreate2d = null;
                    DateTime? fnModify2d = null;
                    DateTime? fnAccess2d = null;
                    DateTime? fnEntry2d = null;

                    if (fnCreate2.Equals("NoFNRecord") == false && fnCreate2.Length > 0)
                    {
                        fnCreate2d = DateTime.Parse(fnCreate2,
                            CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime();
                        fnModify2d = DateTime.Parse(csv.GetField(22).Trim('=').Trim('"'),
                            CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime();
                        fnAccess2d = DateTime.Parse(csv.GetField(23).Trim('=').Trim('"'),
                            CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime();
                        fnEntry2d = DateTime.Parse(csv.GetField(24).Trim('=').Trim('"'),
                            CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime();
                    }


                    var fileName3 = csv.GetField(25);

                    var fnCreate3 = csv.GetField(26).Trim('=').Trim('"');
                    DateTime? fnCreate3d = null;
                    DateTime? fnModify3d = null;
                    DateTime? fnAccess3d = null;
                    DateTime? fnEntry3d = null;

                    if (fnCreate3.Equals("NoFNRecord") == false && fnCreate3.Length > 0)
                    {
                        fnCreate3d = DateTime.Parse(fnCreate3,
                            CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime();
                        fnModify3d = DateTime.Parse(csv.GetField(27).Trim('=').Trim('"'),
                            CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime();
                        fnAccess3d = DateTime.Parse(csv.GetField(29).Trim('=').Trim('"'),
                            CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime();
                        fnEntry3d = DateTime.Parse(csv.GetField(29).Trim('=').Trim('"'),
                            CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime();
                    }

                    var fileName4 = csv.GetField(30);

                    var fnCreate4 = csv.GetField(31).Trim('=').Trim('"');
                    DateTime? fnCreate4d = null;
                    DateTime? fnModify4d = null;
                    DateTime? fnAccess4d = null;
                    DateTime? fnEntry4d = null;

                    if (fnCreate4.Equals("NoFNRecord") == false && fnCreate4.Length > 0)
                    {
                        fnCreate4d = DateTime.Parse(fnCreate4,
                            CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime();
                        fnModify4d = DateTime.Parse(csv.GetField(32).Trim('=').Trim('"'),
                            CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime();
                        fnAccess4d = DateTime.Parse(csv.GetField(33).Trim('=').Trim('"'),
                            CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime();
                        fnEntry4d = DateTime.Parse(csv.GetField(34).Trim('=').Trim('"'),
                            CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime();
                    }

                    var stdInfo = csv.GetField(35).Equals("True");
                    var attrList = csv.GetField(36).Equals("True");
                    var fn = csv.GetField(37).Equals("True");
                    var objectId2 = csv.GetField(38).Equals("True");
                    var volName = csv.GetField(39).Equals("True");
                    var volInfo = csv.GetField(40).Equals("True");
                    var data = csv.GetField(41).Equals("True");
                    var indexRoot = csv.GetField(42).Equals("True");

                    var indexAlloc = csv.GetField(43).Equals("True");
                    var bitmap = csv.GetField(44).Equals("True");
                    var reparsePoint = csv.GetField(45).Equals("True");
                    var eaInfo = csv.GetField(46).Equals("True");
                    var ea = csv.GetField(47).Equals("True");
                    var propSet = csv.GetField(48).Equals("True");
                    var loggedUtil = csv.GetField(49).Equals("True");
                    var logNotes = csv.GetField(50);
                    var fnShift = csv.GetField(51).Equals("Y");
                    var uSecZero = csv.GetField(52).Equals("Y");
                    var hasAds = csv.GetField(53).Equals("Y");

                    var e = new AnalyzeMftData
                    {
                        RecordNumber = recNum,
                        Good = isGood,
                        Active = isActive,
                        RecordType = recType,
                        SequenceNumber = int.Parse(seqNumber),
                        ParentFileRecordNumber = long.Parse(parentEntryNum),
                        ParentFileRecordSequence = int.Parse(parentSeqNum),
                        FileName1 = fileName1,
                        FileNameCreated = fnCreate1d,
                        FileNameModified = fnModify1d,
                        FileNameAccessed = fnAccess1d,
                        FileNameEntryDate = fnEntry1d,
                        StdInfoCreated = siCreate1D,
                        StdInfoModified = siModify1d,
                        StdInfoAccessed = siAccess1d,
                        StdInfoEntryDate = siEntry1d,
                        FileName2 = fileName2,
                        FileNameCreated2 = fnCreate2d,
                        FileNameModified2 = fnModify2d,
                        FileNameAccessed2 = fnAccess2d,
                        FileNameEntryDate2 = fnEntry2d,
                        FileName3 = fileName3,
                        FileNameCreated3 = fnCreate3d,
                        FileNameModified3 = fnModify3d,
                        FileNameAccessed3 = fnAccess3d,
                        FileNameEntryDate3 = fnEntry3d,
                        FileName4 = fileName4,
                        FileNameCreated4 = fnCreate4d,
                        FileNameModified4 = fnModify4d,
                        FileNameAccessed4 = fnAccess4d,
                        FileNameEntryDate4 = fnEntry4d,
                        ObjectId = objectId,
                        BirthDomainId = birthDomainId,
                        BirthObjectId = birthObjectId,
                        BirthVolumeId = birthVolId,
                        LogNotes = logNotes,
                        StandardInformation = stdInfo,
                        AttributeList = attrList,
                        Filename = fn,
                        ObjectId2 = objectId2,
                        VolumeName = volName,
                        VolumeInfo = volInfo,
                        Data = data,
                        IndexRoot = indexRoot,
                        IndexAllocation = indexAlloc,
                        Bitmap = bitmap,
                        ReparsePoint = reparsePoint,
                        EaInformation = eaInfo,
                        Ea = ea,
                        PropertySet = propSet,
                        LoggedUtilityStream = loggedUtil,
                        StfFnShift = fnShift,
                        uSecZero = uSecZero,
                        Ads = hasAds,
                        Line = ln
                    };
                    e.Tag = TaggedLines.Contains(ln);

                    DataList.Add(e);
                    ln += 1;
                }
            }
        }
    }

    public class CrowdStrikeEventData : IFileSpecData
    {
        public DateTime Timestamp { get; set; }
        public string TreeId { get; set; }
        public string PatternId { get; set; }
        public string ParentProcessId { get; set; }
        public string ProcessId { get; set; }
        public string Event { get; set; }
        public string FileLoadedExecuted { get; set; }
        public string Domain { get; set; }
        public string DestIP { get; set; }
        public string FileAccessedWritten { get; set; }
        public string UserSid { get; set; }
        public string UserName { get; set; }
        public string Attributes { get; set; }
        public string Aid { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return
                $"{Timestamp} {TreeId} {PatternId} {ParentProcessId} {ProcessId} {Event} {FileLoadedExecuted} {Domain} {DestIP} {FileAccessedWritten} {UserSid} {UserName} {Attributes} {Aid}";
        }
    }


    public class CrowdStrikeEvent : IFileSpec
    {
        public CrowdStrikeEvent()
        {
            //Initialize collections here, one for TaggedLines TLE can add values to, and the collection that TLE will display
            TaggedLines = new List<int>();

            DataList = new BindingList<CrowdStrikeEventData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "timestamp,treeid,patternid,\"parent process id\",\"process id\",event,\"file loaded/executed\",domain,destip,\"file accessed/written\",\"usersid_readable\",username,attributes,aid"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from CrowdStrike Event Data";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "e8c703e2-a23b-45ce-b01a-b133a820ed7a";

        public void ProcessFile(string filename)
        {
            DataList.Clear();

            using (var fileReader = File.OpenText(filename))
            {
                var csv = new CsvReader(fileReader, CultureInfo.InvariantCulture);
                

                var o = new TypeConverterOptions
                {
                    DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal
                };
                csv.Context.TypeConverterOptionsCache.AddOptions<CrowdStrikeEventData>(o);


                var foo = csv.Context.AutoMap<CrowdStrikeEventData>();

                foo.Map(m => m.Timestamp).Convert(row =>
                    DateTime.Parse(row.Row.GetField<string>("Timestamp").Replace("Z", "")));

                foo.Map(t => t.ProcessId).Name("Process ID");
                foo.Map(t => t.ParentProcessId).Name("Parent Process ID");
                foo.Map(t => t.TreeId).Name("TreeID");
                foo.Map(t => t.PatternId).Name("PatternID");
                foo.Map(t => t.FileLoadedExecuted).Name("File Loaded/Executed");
                foo.Map(t => t.DestIP).Name("DestIP");
                foo.Map(t => t.FileAccessedWritten).Name("File Accessed/Written");
                foo.Map(t => t.UserSid).Name("UserSid_readable");
                foo.Map(t => t.Aid).Name("aid");


                foo.Map(t => t.Line).Ignore();
                foo.Map(t => t.Tag).Ignore();

                csv.Context.RegisterClassMap(foo);

                var l = LogManager.GetCurrentClassLogger();

                var records = csv.GetRecords<CrowdStrikeEventData>();

                var ln = 1;
                foreach (var record in records)
                {
                    l.Debug($"Line # {ln}, Record: {csv.Context.Parser.RawRecord}");

                    record.Line = ln;

                    record.Tag = TaggedLines.Contains(ln);

                    DataList.Add(record);

                    ln += 1;
                }
            }
        }
    }

    public class ShimcacheParserData : IFileSpecData
    {
        public ShimcacheParserData(int line, DateTime modified, DateTime? update, bool executed, long fileSize,
            string filePath)
        {
            Line = line;
            LastModified = modified;
            LastUpdate = update;
            Executed = executed;
            FileSize = fileSize;
            FilePath = filePath;
        }

        public int Order { get; set; }

        public DateTime LastModified { get; set; }
        public DateTime? LastUpdate { get; set; }
        public bool Executed { get; set; }

        public long FileSize { get; set; }

        public string FilePath { get; set; }

        public int Line { get; set; }

        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{LastModified} {LastUpdate} {Executed} {FileSize} {FilePath}";
        }
    }


    public class ShimcacheParser : IFileSpec
    {
        public ShimcacheParser()
        {
            //Initialize collections here, one for TaggedLines TLE can add values to, and the collection that TLE will display
            TaggedLines = new List<int>();

            DataList = new BindingList<ShimcacheParserData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "last modified,last update,path,file size,exec flag"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from Shimcache Parser";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "775d1a3f-092a-42ed-bd00-e6b8827ae70a";

        public void ProcessFile(string filename)
        {
            DataList.Clear();

            using (var fileReader = File.OpenText(filename))
            {
                var csv = new CsvReader(fileReader, CultureInfo.InvariantCulture);
                
                var foo = csv.Context.AutoMap<ShimcacheParserData>();

                var o = new TypeConverterOptions
                {
                    DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal
                };
                csv.Context.TypeConverterOptionsCache.AddOptions<ShimcacheParserData>(o);

                foo.Map(t => t.Line).Ignore();
                foo.Map(t => t.Tag).Ignore();

                foo.Map(t => t.LastModified).Name("Last Modified");
                foo.Map(t => t.LastUpdate).Name("Last Update");
                foo.Map(t => t.Executed).Name("Exec Flag");
                foo.Map(t => t.FileSize).Name("File Size");
                foo.Map(t => t.FilePath).Name("Path");

                csv.Context.RegisterClassMap(foo);

                var l = LogManager.GetCurrentClassLogger();

                csv.Read();
                csv.ReadHeader();

                var ln = 1;
                while (csv.Read())
                {
                    l.Debug($"Line # {ln}, Record: {csv.Context.Parser.RawRecord}");

                    var modified = DateTime.Parse(csv.GetField("Last Modified"),
                        CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime();

                    var upraw = csv.GetField("Last Update");
                    DateTime? update = null;
                    if (upraw.Length > 0 && upraw != "N/A")
                    {
                        update = DateTime.Parse(upraw,
                            CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime();
                    }

                    var executed = csv.GetField("Exec Flag") == "True";
                    var fileSize = csv.GetField("File Size").Length > 0 && csv.GetField("File Size") != "N/A"
                        ? long.Parse(csv.GetField("File Size"))
                        : 0;
                    var filePath = csv.GetField("Path");

                    var e = new ShimcacheParserData(ln, modified, update, executed, fileSize, filePath);
                    e.Tag = TaggedLines.Contains(ln);
                    DataList.Add(e);
                    ln += 1;
                }
            }
        }
    }

    public class ShimcacheVolatilityData : IFileSpecData
    {
        public ShimcacheVolatilityData(int line, int order, DateTime? modified, DateTime? update, bool executed,
            long fileSize,
            string filePath)
        {
            Line = line;
            Order = order;
            LastModified = modified;
            LastUpdate = update;
            Executed = executed;
            FileSize = fileSize;
            FilePath = filePath;
        }

        public int Order { get; set; }

        public DateTime? LastModified { get; set; }
        public DateTime? LastUpdate { get; set; }
        public bool Executed { get; set; }

        public long FileSize { get; set; }

        public string FilePath { get; set; }

        public int Line { get; set; }

        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{LastModified} {LastUpdate} {Executed} {FileSize} {FilePath}";
        }
    }


    public class ShimcacheVolatility : IFileSpec
    {
        public ShimcacheVolatility()
        {
            //Initialize collections here, one for TaggedLines TLE can add values to, and the collection that TLE will display
            TaggedLines = new List<int>();

            DataList = new BindingList<ShimcacheVolatilityData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "order,last modified,last update,exec flag,file size,file path"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from ShimcacheVolatility";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "45c6c2d5-9f30-4e08-9cad-130f9ca9b67b";

        public void ProcessFile(string filename)
        {
            DataList.Clear();

            using (var fileReader = File.OpenText(filename))
            {
                var csv = new CsvReader(fileReader, CultureInfo.InvariantCulture);
                
                var foo = csv.Context.AutoMap<ShimcacheVolatilityData>();

                var o = new TypeConverterOptions
                {
                    DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal
                };
                csv.Context.TypeConverterOptionsCache.AddOptions<ShimcacheVolatilityData>(o);

                foo.Map(t => t.Line).Ignore();
                foo.Map(t => t.Tag).Ignore();

                foo.Map(t => t.Order).Name("Order");
                foo.Map(t => t.LastModified).Name("Last Modified");
                foo.Map(t => t.LastUpdate).Name("Last Update");
                foo.Map(t => t.Executed).Name("Exec Flag");
                foo.Map(t => t.FileSize).Name("File Size");
                foo.Map(t => t.FilePath).Name("File Path");

                csv.Context.RegisterClassMap(foo);

                var l = LogManager.GetCurrentClassLogger();

                csv.Read();
                csv.ReadHeader();

                var ln = 1;
                while (csv.Read())
                {
                    l.Debug($"Line # {ln}, Record: {csv.Context.Parser.RawRecord}");

                    var order = int.Parse(csv.GetField("Order"));

                    var lastMod = csv.GetField("Last Modified");
                    DateTime? lastModTs = null;
                    if (lastMod.Length > 0)
                    {
                        lastModTs = DateTime.Parse(lastMod,
                            CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime();
                    }

                    var upraw = csv.GetField("Last Update");
                    DateTime? update = null;
                    if (upraw.Length > 0)
                    {
                        update = DateTime.Parse(upraw,
                            CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime();
                    }

                    var executed = csv.GetField("Exec Flag") == "True";
                    var fileSize = csv.GetField("File Size").Length > 0
                        ? long.Parse(csv.GetField("File Size"))
                        : 0;
                    var filePath = csv.GetField("File Path");

                    var e = new ShimcacheVolatilityData(ln, order, lastModTs, update, executed, fileSize, filePath);
                    e.Tag = TaggedLines.Contains(ln);
                    DataList.Add(e);
                    ln += 1;
                }
            }
        }
    }

    public class DensityScoutData : IFileSpecData
    {
        public float Score { get; set; }
        public string Path { get; set; }
        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{Score} {Path}";
        }
    }

    public class DensityScout : IFileSpec
    {
        public DensityScout()
        {
            //Initialize collections here, one for TaggedLines TLE can add values to, and the collection that TLE will display
            TaggedLines = new List<int>();

            DataList = new BindingList<DensityScoutData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "("
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from DensityScout";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "72ac8317-f9b5-4d91-9324-0a17b03d44df";

        public void ProcessFile(string filename)
        {
            DataList.Clear();

            using (var fileReader = File.OpenText(filename))
            {
                var l = LogManager.GetCurrentClassLogger();

                var ln = fileReader.ReadLine();
                var ln1 = 1;
                while (ln != null)
                {
                    l.Debug($"Line # {ln1}, Record: {ln}");

                    var d = new DensityScoutData();

                    var dsegs = ln.Split('|');

                    var rawNum = dsegs.First().Substring(1, dsegs.First().Length - 3);

                    d.Score = float.Parse(rawNum);
                    d.Path = dsegs.Last();
                    d.Line = ln1;
                    d.Tag = TaggedLines.Contains(ln1);
                    DataList.Add(d);
                    ln1 += 1;
                    ln = fileReader.ReadLine();
                }
            }
        }
    }
}