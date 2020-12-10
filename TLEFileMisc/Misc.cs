using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLEFileMisc
{
    public class Misc
    {
    }


    /*

     if (headerLine.StartsWith("("))
                    {
                        FileType = ContentType.DensityScout;
                        break;
                    }

    case "order,last modified,last update,exec flag,file size,file path":
    FileType = ContentType.ShimcacheVolatility;
    break;
               

    case "last modified,last update,path,file size,exec flag":
    FileType = ContentType.ShimcacheParser;
    break;

    case "timestamp,treeid,patternid,\"parent process id\",\"process id\",event,\"file loaded/executed\",domain,destip,\"file accessed/written\",\"usersid_readable\",username,attributes,aid":
    FileType = ContentType.CrowdStrikeEvent;
    case
    "\"record number\",\"good\",\"active\",\"record type\",\"sequence number\",\"parent file rec. #\",\"parent file rec. seq. #\",\"filename #1\",\"std info creation date\",\"std info modification date\",\"std info access date\",\"std info entry date\",\"fn info creation date\",\"fn info modification date\",\"fn info access date\",\"fn info entry date\",\"object id\",\"birth volume id\",\"birth object id\",\"birth domain id\",\"filename #2\",\"fn info creation date\",\"fn info modify date\",\"fn info access date\",\"fn info entry date\",\"filename #3\",\"fn info creation date\",\"fn info modify date\",\"fn info access date\",\"fn info entry date\",\"filename #4\",\"fn info creation date\",\"fn info modify date\",\"fn info access date\",\"fn info entry date\",\"standard information\",\"attribute list\",\"filename\",\"object id\",\"volume name\",\"volume info\",\"data\",\"index root\",\"index allocation\",\"bitmap\",\"reparse point\",\"ea information\",\"ea\",\"property set\",\"logged utility stream\",\"log/notes\",\"stf fn shift\",\"usec zero\",\"ads\""
    :
    FileType = ContentType.AnalyzeMft;
    break;*/

          // case ContentType.CrowdStrikeEvent:
          //           DataList = new BindingList<CrowdStrikeEvents>();
          //           using (var fileReader = File.OpenText(FileName))
          //           {
          //               var csv = new CsvReader(fileReader,CultureInfo.InvariantCulture);
          //               csv.Configuration.HasHeaderRecord = true;
          //               csv.Configuration.Delimiter = separator;
          //               var o = new TypeConverterOptions
          //               {
          //                   DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal
          //               };
          //               csv.Configuration.TypeConverterOptionsCache.AddOptions<CrowdStrikeEvents>(o);
          //
          //
          //               var foo = csv.Configuration.AutoMap<CrowdStrikeEvents>();
          //
          //               foo.Map(m => m.Timestamp).ConvertUsing(row =>
          //                   DateTime.Parse(row.GetField<string>("Timestamp").Replace("Z", "")));
          //
          //               foo.Map(t => t.ProcessId).Name("Process ID");
          //               foo.Map(t => t.ParentProcessId).Name("Parent Process ID");
          //               foo.Map(t => t.TreeId).Name("TreeID");
          //               foo.Map(t => t.PatternId).Name("PatternID");
          //               foo.Map(t => t.FileLoadedExecuted).Name("File Loaded/Executed");
          //               foo.Map(t => t.DestIP).Name("DestIP");
          //               foo.Map(t => t.FileAccessedWritten).Name("File Accessed/Written");
          //               foo.Map(t => t.UserSid).Name("UserSid_readable");
          //               foo.Map(t => t.Aid).Name("aid");
          //
          //
          //               foo.Map(t => t.Line).Ignore();
          //               foo.Map(t => t.Tag).Ignore();
          //
          //               csv.Configuration.RegisterClassMap(foo);
          //
          //               var records = csv.GetRecords<CrowdStrikeEvents>();
          //
          //               var ln = 1;
          //               foreach (var record in records)
          //               {
          //                   record.Line = ln;
          //
          //                   record.Tag = _taggedLines.Contains(ln);
          //                   
          //                   DataList.Add(record);
          //
          //                   ln += 1;
          //               }
          //           }
          //           return true;

        /*case ContentType.DensityScout:
        DataList = new BindingList<DensityScout>();
        using (var fileReader = File.OpenText(FileName))
        {
            var l = fileReader.ReadLine();
            var ln1 = 1;
            while (l != null)
            {
                var d = new DensityScout();

                var dsegs = l.Split('|');

                var rawNum = dsegs.First().Substring(1, dsegs.First().Length - 3);

                d.Score = float.Parse(rawNum);
                d.Path = dsegs.Last();
                d.Line = ln1;
                d.Tag = _taggedLines.Contains(ln1);
                DataList.Add(d);
                ln1 += 1;
                l = fileReader.ReadLine();
            }
        }

        return true;*/




          /*
          case ContentType.ShimcacheParser:
                    DataList = new BindingList<ShimcacheParser>();
                    using (var fileReader = File.OpenText(FileName))
                    {
                        var csv = new CsvReader(fileReader,CultureInfo.InvariantCulture);
                        csv.Configuration.HasHeaderRecord = true;
                        var foo = csv.Configuration.AutoMap<ShimcacheParser>();

                        var o = new TypeConverterOptions
                        {
                            DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal
                        };
                        csv.Configuration.TypeConverterOptionsCache.AddOptions<ShimcacheParser>(o);

                        foo.Map(t => t.Line).Ignore();
                        foo.Map(t => t.Tag).Ignore();

                        foo.Map(t => t.LastModified).Name("Last Modified");
                        foo.Map(t => t.LastUpdate).Name("Last Update");
                        foo.Map(t => t.Executed).Name("Exec Flag");
                        foo.Map(t => t.FileSize).Name("File Size");
                        foo.Map(t => t.FilePath).Name("Path");

                        csv.Configuration.RegisterClassMap(foo);

                        csv.Read();
                        csv.ReadHeader();

                        var ln = 1;
                        while (csv.Read())
                        {
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

                            var e = new ShimcacheParser(ln, modified, update, executed, fileSize, filePath);
                            e.Tag = _taggedLines.Contains(ln);
                            DataList.Add(e);
                            ln += 1;
                        }
                    }

                    return true;
                case ContentType.ShimcacheVolatility:
                    DataList = new BindingList<ShimcacheMemory>();
                    using (var fileReader = File.OpenText(FileName))
                    {
                        var csv = new CsvReader(fileReader,CultureInfo.InvariantCulture);
                        csv.Configuration.HasHeaderRecord = true;
                        var foo = csv.Configuration.AutoMap<ShimcacheMemory>();

                        var o = new TypeConverterOptions
                        {
                            DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal
                        };
                        csv.Configuration.TypeConverterOptionsCache.AddOptions<ShimcacheMemory>(o);

                        foo.Map(t => t.Line).Ignore();
                        foo.Map(t => t.Tag).Ignore();

                        foo.Map(t => t.Order).Name("Order");
                        foo.Map(t => t.LastModified).Name("Last Modified");
                        foo.Map(t => t.LastUpdate).Name("Last Update");
                        foo.Map(t => t.Executed).Name("Exec Flag");
                        foo.Map(t => t.FileSize).Name("File Size");
                        foo.Map(t => t.FilePath).Name("File Path");

                        csv.Configuration.RegisterClassMap(foo);

                        csv.Read();
                        csv.ReadHeader();

                        var ln = 1;
                        while (csv.Read())
                        {
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

                            var e = new ShimcacheMemory(ln, order, lastModTs, update, executed, fileSize, filePath);
                            e.Tag = _taggedLines.Contains(ln);
                            DataList.Add(e);
                            ln += 1;
                        }
                    }

                    return true;
                case ContentType.AnalyzeMft:
                    DataList = new BindingList<AnalyzeMft>();
                    using (var fileReader = File.OpenText(FileName))
                    {
                        var csv = new CsvReader(fileReader,CultureInfo.InvariantCulture);
                        csv.Configuration.HasHeaderRecord = true;
                        var foo = csv.Configuration.AutoMap<AnalyzeMft>();

                        var o = new TypeConverterOptions
                        {
                            DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal
                        };
                        csv.Configuration.TypeConverterOptionsCache.AddOptions<AnalyzeMft>(o);

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


                        csv.Configuration.RegisterClassMap(foo);

                        csv.Read();
                        csv.ReadHeader();

                        var ln = 1;
                        while (csv.Read())
                        {
                            if (csv.Context.RawRecord.Contains("BAAD MFT Record"))
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

                            var e = new AnalyzeMft
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
                            e.Tag = _taggedLines.Contains(ln);

                            DataList.Add(e);
                            ln += 1;
                        }
                    }

                    return true;*/
    }

