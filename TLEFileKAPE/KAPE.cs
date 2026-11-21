using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using CsvHelper;
using ITLEFileSpec;
using TLEFilePlugins.Shared;
using Serilog;

namespace TLEFileKAPE
{
    [CsvRecord]
    public partial class KapeCopyLogData : IFileSpecData
    {
        [CsvColumn(Optional = true)] public DateTime CopiedTimestamp { get; set; }
        [CsvColumn(Optional = true)] public string SourceFile { get; set; }
        [CsvColumn(Optional = true)] public string DestinationFile { get; set; }
        [CsvColumn(Optional = true)] public long FileSize { get; set; }
        [CsvColumn(Optional = true)] public string SourceFileSha1 { get; set; }
        [CsvColumn(Optional = true)] public bool DeferredCopy { get; set; }

        [CsvColumn(Optional = true)] public DateTime CreatedOnUtc { get; set; }
        [CsvColumn(Optional = true)] public DateTime ModifiedOnUtc { get; set; }
        [CsvColumn(Optional = true)] public DateTime LastAccessedOnUtc { get; set; }

        [CsvColumn(Optional = true)] public TimeSpan CopyDuration { get; set; }

        [CsvIgnore] public int Line { get; set; }

        [CsvIgnore] public bool Tag { get; set; }

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
            CsvLoader.Load<KapeCopyLogData>(filename, DataList, TaggedLines);
        }
    }

    [CsvRecord]
    public partial class KapeSkipLogData : IFileSpecData
    {
        [CsvColumn(Optional = true)] public string SourceFile { get; set; }
        [CsvColumn(Optional = true)] public string SourceFileSha1 { get; set; }
        [CsvColumn(Optional = true)] public string Reason { get; set; }

        [CsvIgnore] public int Line { get; set; }

        [CsvIgnore] public bool Tag { get; set; }

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
            CsvLoader.Load<KapeSkipLogData>(filename, DataList, TaggedLines);
        }
    }


    [CsvRecord]
    public partial class KapeTriageData : IFileSpecData
    {
        [CsvColumn(Optional = true)] public DateTime DateTimeUTC { get; set; }

        [CsvColumn(Optional = true)] public string EventTimeSource { get; set; }
        [CsvColumn(Optional = true)] public string Hostname { get; set; }
        [CsvColumn(Optional = true)] public string HostIPAddress { get; set; }
        [CsvColumn(Optional = true)] public string UserID { get; set; }

        [CsvColumn(Optional = true)] public string Assessment { get; set; }
        [CsvColumn(Optional = true)] public string EventType { get; set; }
        [CsvColumn(Optional = true)] public string EventDetails { get; set; }
        [CsvColumn(Optional = true)] public string SDIPAddress { get; set; }

        [CsvColumn(Optional = true)] public string ExaminerComments { get; set; }
        [CsvColumn(Optional = true)] public string MD5 { get; set; }
        [CsvColumn(Optional = true)] public string EventAddedBy { get; set; }
        [CsvColumn(Optional = true)] public DateTime DateAdded { get; set; }
        [CsvColumn(Optional = true)] public string RawEvent { get; set; }

        [CsvIgnore] public int Line { get; set; }
        [CsvIgnore] public bool Tag { get; set; }

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

            DataList = new BindingList<KapeTriageData>();

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
            CsvLoader.Load<KapeTriageData>(filename, DataList, TaggedLines);
        }
    }
}