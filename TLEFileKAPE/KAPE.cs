using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.TypeConversion;
using ITLEFileSpec;

namespace TLEFileKAPE
{
    public class KapeCopyLogData : IFileSpecData
    {
        public DateTime CopiedTimestamp { get; set; }
        public string SourceFile { get; set; }
        public string DestinationFile { get; set; }
        public long FileSize { get; set; }
        public string SourceFileSha1 { get; set; }
        public bool DeferredCopy { get; set; }

        public DateTime CreatedOnUtc { get; set; }
        public DateTime ModifiedOnUtc { get; set; }
        public DateTime LastAccessedOnUtc { get; set; }

        public TimeSpan CopyDuration { get; set; }

        public int Line { get; set; }

        public bool Tag { get; set; }

        public override string ToString()
        {
            return
                $"{CopiedTimestamp} {SourceFile} {DestinationFile} {FileSize} {SourceFileSha1} {DeferredCopy} {CreatedOnUtc} {ModifiedOnUtc} {LastAccessedOnUtc} {CopyDuration}";
        }
    }

    public class KapeCopyLog : IFileSpec
    {
        public KapeCopyLog()
        {
            //Initialize collections here, one for TaggedLines TLE can add values to, and the collection that TLE will display
            TaggedLines = new List<int>();

            DataList = new BindingList<KapeCopyLogData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "CopiedTimestamp,SourceFile,DestinationFile,FileSize,SourceFileSha1,DeferredCopy,CreatedOnUtc,ModifiedOnUtc,LastAccessedOnUtc,CopyDuration"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from KAPE for copied files";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "11dd7405-16cf-4612-a480-9b050d0a9952";

        public void ProcessFile(string filename)
        {
            DataList.Clear();

            using (var fileReader = File.OpenText(filename))
            {
                var csv = new CsvReader(fileReader, CultureInfo.InvariantCulture);
                csv.Configuration.HasHeaderRecord = true;
                
                var o = new TypeConverterOptions
                {
                    DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal
                };
                csv.Configuration.TypeConverterOptionsCache.AddOptions<KapeCopyLogData>(o);


                var foo = csv.Configuration.AutoMap<KapeCopyLogData>();

                foo.Map(t => t.Line).Ignore();
                foo.Map(t => t.Tag).Ignore();

                csv.Configuration.RegisterClassMap(foo);

                var records = csv.GetRecords<KapeCopyLogData>();

                var ln = 1;
                foreach (var record in records)
                {
                    record.Line = ln;

                    record.Tag = TaggedLines.Contains(ln);

                    DataList.Add(record);

                    ln += 1;
                }
            }
        }
    }

    public class KapeSkipLogData : IFileSpecData
    {
        public string SourceFile { get; set; }
        public string SourceFileSha1 { get; set; }
        public string Reason { get; set; }

        public int Line { get; set; }

        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{SourceFile} {SourceFileSha1} {Reason}";
        }
    }


    public class KapeSkipLog : IFileSpec
    {
        public KapeSkipLog()
        {
            //Initialize collections here, one for TaggedLines TLE can add values to, and the collection that TLE will display
            TaggedLines = new List<int>();

            DataList = new BindingList<KapeSkipLogData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "SourceFile,SourceFileSha1,Reason"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from KAPE for skipped files";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "40dd7405-16cf-4612-b220-9a050d0a9952";

        public void ProcessFile(string filename)
        {
            DataList.Clear();

            using (var fileReader = File.OpenText(filename))
            {
                var csv = new CsvReader(fileReader, CultureInfo.InvariantCulture);
                csv.Configuration.HasHeaderRecord = true;
                
                var o = new TypeConverterOptions
                {
                    DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal
                };
                csv.Configuration.TypeConverterOptionsCache.AddOptions<KapeSkipLogData>(o);


                var foo = csv.Configuration.AutoMap<KapeSkipLogData>();

                foo.Map(t => t.Line).Ignore();
                foo.Map(t => t.Tag).Ignore();

                csv.Configuration.RegisterClassMap(foo);

                var records = csv.GetRecords<KapeSkipLogData>();

                var ln = 1;
                foreach (var record in records)
                {
                    record.Line = ln;

                    record.Tag = TaggedLines.Contains(ln);

                    DataList.Add(record);

                    ln += 1;
                }
            }
        }
    }


    public class KapeTriageData : IFileSpecData
    {
        public DateTime DateTimeUTC { get; set; }

        public string EventTimeSource { get; set; }
        public string Hostname { get; set; }
        public string HostIPAddress { get; set; }
        public string UserID { get; set; }

        public string Assessment { get; set; }
        public string EventType { get; set; }
        public string EventDetails { get; set; }
        public string SDIPAddress { get; set; }

        public string ExaminerComments { get; set; }
        public string MD5 { get; set; }
        public string EventAddedBy { get; set; }
        public DateTime DateAdded { get; set; }
        public string RawEvent { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return
                $"{DateTimeUTC} {EventTimeSource} {Hostname} {HostIPAddress} {UserID} {Assessment} {EventType} {EventDetails} {SDIPAddress} {ExaminerComments} {MD5} {EventAddedBy} {DateAdded} {RawEvent}";
        }
    }


    public class KapeTriage : IFileSpec
    {
        public KapeTriage()
        {
            //Initialize collections here, one for TaggedLines TLE can add values to, and the collection that TLE will display
            TaggedLines = new List<int>();

            DataList = new BindingList<KapeSkipLogData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "\"datetimeutc\",\"eventtimesource\",\"hostname\",\"hostipaddress\",\"userid\",\"assessment\",\"eventtype\",\"eventdetails\",\"sdipaddress\",\"examinercomments\",\"md5\",\"eventaddedby\",\"dateadded\",\"rawevent\""
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from KAPE triage package";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "40dd7405-16cf-4612-a480-9a050d0a2222";

        public void ProcessFile(string filename)
        {
            DataList.Clear();

            using (var fileReader = File.OpenText(filename))
            {
                var csv = new CsvReader(fileReader, CultureInfo.InvariantCulture);
                csv.Configuration.HasHeaderRecord = true;
                
                var o = new TypeConverterOptions
                {
                    DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal
                };
                csv.Configuration.TypeConverterOptionsCache.AddOptions<KapeTriageData>(o);

                var foo = csv.Configuration.AutoMap<KapeTriageData>();

                foo.Map(t => t.Line).Ignore();
                foo.Map(t => t.Tag).Ignore();

                csv.Configuration.RegisterClassMap(foo);

                var records = csv.GetRecords<KapeTriageData>();

                var ln = 1;
                foreach (var record in records)
                {
                    record.Line = ln;

                    record.Tag = TaggedLines.Contains(ln);

                    DataList.Add(record);

                    ln += 1;
                }
            }
        }
    }
}