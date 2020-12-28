using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.TypeConversion;
using ITLEFileSpec;
using NLog;

namespace TLEFileEZTools
{
    public class BootData : IFileSpecData
    {
        public string EntryPoint { get; set; }
        public string Signature { get; set; }
        public int BytesPerSector { get; set; }
        public int SectorsPerCluster { get; set; }
        public int ClusterSize => BytesPerSector * SectorsPerCluster;
        public long ReservedSectors { get; set; }
        public long TotalSectors { get; set; }
        public long MftClusterBlockNumber { get; set; }
        public long MftMirrClusterBlockNumber { get; set; }
        public int MftEntrySize { get; set; }
        public int IndexEntrySize { get; set; }
        public string VolumeSerialNumberRaw { get; set; }
        public string VolumeSerialNumber { get; set; }
        public string VolumeSerialNumber32 { get; set; }
        public string VolumeSerialNumber32Reverse { get; set; }
        public string SectorSignature { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        /// <summary>
        ///     When searching in TLE, this is used to look for the search term across all the properties in the class
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return
                $"{EntryPoint} {Signature} {BytesPerSector} {SectorsPerCluster} {ClusterSize} {ReservedSectors} {TotalSectors} {MftClusterBlockNumber} {MftMirrClusterBlockNumber} {MftEntrySize} {IndexEntrySize} {VolumeSerialNumberRaw} {VolumeSerialNumber} {VolumeSerialNumber32} {VolumeSerialNumber32Reverse} {SectorSignature}";
        }
    }

    /// <summary>
    ///     This is the "container" class that defines properties, how to process a file, etc. The actual data the file
    ///     contains is defined in BootData.
    /// </summary>
    public class Boot : IFileSpec
    {
        public Boot()
        {
            //Initialize collections here, one for TaggedLines TLE can add values to, and the collection that TLE will display
            TaggedLines = new List<int>();

            DataList = new BindingList<BootData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "EntryPoint,Signature,BytesPerSector,SectorsPerCluster,ClusterSize,ReservedSectors,TotalSectors,MftClusterBlockNumber,MftMirrClusterBlockNumber,MftEntrySize,IndexEntrySize,VolumeSerialNumberRaw,VolumeSerialNumber,VolumeSerialNumber32,VolumeSerialNumber32Reverse,SectorSignature,SourceFile"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from MFTECmd for $Boot";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

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
                csv.Configuration.TypeConverterOptionsCache.AddOptions<BootData>(o);

                var foo = csv.Configuration.AutoMap<BootData>();

                foo.Map(t => t.Line).Ignore();
                foo.Map(t => t.Tag).Ignore();

                csv.Configuration.RegisterClassMap(foo);

                var records = csv.GetRecords<BootData>();

                var l = LogManager.GetCurrentClassLogger();

                var ln = 1;
                foreach (var record in records)
                {
                    //In TLE, there is an option to enable Debug messages which lets more context be seen when errors happen. Keep this call here so the last known good line of data is obvious in TLE messages
                    l.Debug($"Line # {ln}, Record: {csv.Context.RawRecord}");

                    record.Line = ln;
                    record.Tag = TaggedLines.Contains(ln);
                    DataList.Add(record);

                    ln += 1;
                }
            }
        }

        public string InternalGuid => "40dd7405-16cf-4612-a480-9b150d0a9952";
    }

    public class SdsData : IFileSpecData
    {
        public string Hash { get; set; }
        public uint Id { get; set; }
        public ulong Offset { get; set; }
        public string OwnerSid { get; set; }
        public string GroupSid { get; set; }
        public string Control { get; set; }

        public int SaclAceCount { get; set; }
        public string UniqueSaclAceTypes { get; set; }

        public int DaclAceCount { get; set; }
        public string UniqueDaclAceTypes { get; set; }

        public ulong FileOffset { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return
                $"{Hash} {Id} {Offset} {OwnerSid} {GroupSid} {Control} {SaclAceCount} {UniqueSaclAceTypes} {DaclAceCount} {UniqueDaclAceTypes} {FileOffset}";
        }
    }

    public class Sds : IFileSpec
    {
        public Sds()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<SdsData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Hash,Id,Offset,OwnerSid,GroupSid,Control,SaclAceCount,UniqueSaclAceTypes,DaclAceCount,UniqueDaclAceTypes,FileOffset,SourceFile"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from MFTECmd for $SDS";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "40dd7405-22cf-4612-a480-9a050d0a9952";

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
                csv.Configuration.TypeConverterOptionsCache.AddOptions<SdsData>(o);

                var foo = csv.Configuration.AutoMap<SdsData>();

                foo.Map(t => t.Line).Ignore();
                foo.Map(t => t.Tag).Ignore();

                csv.Configuration.RegisterClassMap(foo);

                var records = csv.GetRecords<SdsData>();

                var l = LogManager.GetCurrentClassLogger();


                var ln = 1;
                foreach (var record in records)
                {
                    l.Debug($"Line # {ln}, Record: {csv.Context.RawRecord}");
                    record.Line = ln;
                    record.Tag = TaggedLines.Contains(ln);
                    DataList.Add(record);

                    ln += 1;
                }
            }
        }
    }

    public class JData : IFileSpecData
    {
        public string Name { get; set; }
        public string Extension => $"{Path.GetExtension(Name)}{string.Empty}";
        public ulong EntryNumber { get; set; }
        public int SequenceNumber { get; set; }
        public ulong ParentEntryNumber { get; set; }
        public int ParentSequenceNumber { get; set; }

        public ulong UpdateSequenceNumber { get; set; }

        public DateTime UpdateTimestamp { get; set; }

        public string UpdateReasons { get; set; }
        public string FileAttributes { get; set; }
        public long OffsetToData { get; set; }
        public string SourceFile { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return
                $"{Name} {Extension} {EntryNumber} {SequenceNumber} {ParentEntryNumber} {ParentSequenceNumber} {UpdateSequenceNumber} {UpdateReasons} {FileAttributes} {OffsetToData} {SourceFile}";
        }
    }

    public class J : IFileSpec
    {
        public J()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<JData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "name,extension,entrynumber,sequencenumber,parententrynumber,parentsequencenumber,updatesequencenumber,updatetimestamp,updatereasons,fileattributes,offsettodata,sourcefile"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from MFTECmd for $J";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "40ee7405-16cf-4612-a480-9a050d0a9952";

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
                csv.Configuration.TypeConverterOptionsCache.AddOptions<JData>(o);

                var foo = csv.Configuration.AutoMap<JData>();

                foo.Map(m => m.UpdateTimestamp).TypeConverterOption
                    .DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);

                foo.Map(t => t.Line).Ignore();
                foo.Map(t => t.Tag).Ignore();

                csv.Configuration.RegisterClassMap(foo);

                var records = csv.GetRecords<JData>();

                var l = LogManager.GetCurrentClassLogger();


                var ln = 1;
                foreach (var record in records)
                {
                    l.Debug($"Line # {ln}, Record: {csv.Context.RawRecord}");
                    record.Line = ln;
                    record.Tag = TaggedLines.Contains(ln);
                    DataList.Add(record);

                    ln += 1;
                }
            }
        }
    }


    public class MFTData : IFileSpecData
    {
        public uint EntryNumber { get; set; }
        public ushort SequenceNumber { get; set; }
        public uint ParentEntryNumber { get; set; }
        public short? ParentSequenceNumber { get; set; }

        public bool InUse { get; set; }
        public string ParentPath { get; set; }
        public string FileName { get; set; }

        public string Extension { get; set; }

        public bool IsDirectory { get; set; }
        public bool HasAds { get; set; }
        public bool IsAds { get; set; }

        public ulong FileSize { get; set; }

        public DateTime? Created0x10 { get; set; }
        public DateTime? Created0x30 { get; set; }

        public DateTime? LastModified0x10 { get; set; }
        public DateTime? LastModified0x30 { get; set; }

        public DateTime? LastRecordChange0x10 { get; set; }
        public DateTime? LastRecordChange0x30 { get; set; }

        public DateTime? LastAccess0x10 { get; set; }

        public DateTime? LastAccess0x30 { get; set; }

        public long UpdateSequenceNumber { get; set; }
        public long LogfileSequenceNumber { get; set; }

        public int SecurityId { get; set; }

        public string ZoneIdContents { get; set; }
        public string SiFlags { get; set; }
        public string ObjectIdFileDroid { get; set; }
        public string ReparseTarget { get; set; }
        public int ReferenceCount { get; set; }
        public string NameType { get; set; }
        public string LoggedUtilStream { get; set; }
        public bool Timestomped { get; set; }
        public bool uSecZeros { get; set; }
        public bool Copied { get; set; }

        public int Line { get; set; }

        public bool Tag { get; set; }

        public override string ToString()
        {
            return
                $"{EntryNumber} {SequenceNumber} {ParentEntryNumber} {ParentSequenceNumber} {InUse} {ParentPath} {FileName} {Extension} {IsDirectory} {HasAds} {IsAds} {FileSize} {Created0x10} {Created0x30} {LastModified0x10} {LastModified0x30} {LastRecordChange0x10} {LastRecordChange0x30} {LastAccess0x10} {LastAccess0x30} {UpdateSequenceNumber} {LogfileSequenceNumber} {SecurityId} {ZoneIdContents} {SiFlags} {ObjectIdFileDroid} {ReparseTarget}" +
                $" {ReferenceCount} {NameType} {LoggedUtilStream} {Timestomped} {uSecZeros} {Copied}";
        }
    }

    public class MFT : IFileSpec
    {
        public MFT()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<MFTData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "entrynumber,sequencenumber,inuse,parententrynumber,parentsequencenumber,parentpath,filename,extension,filesize,referencecount,reparsetarget,isdirectory,hasads,isads,si<fn,useczeros,copied,siflags,nametype,created0x10,created0x30,lastmodified0x10,lastmodified0x30,lastrecordchange0x10,lastrecordchange0x30,lastaccess0x10,lastaccess0x30,updatesequencenumber,logfilesequencenumber,securityid,objectidfiledroid,loggedutilstream,zoneidcontents"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from MFTECmd for $MFT";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "10dd7405-16cf-4612-a480-9a050d0a9952";

        public void ProcessFile(string filename)
        {
            DataList.Clear();

            using (var fileReader = File.OpenText(filename))
            {
                var csv = new CsvReader(fileReader, CultureInfo.InvariantCulture);
                csv.Configuration.HasHeaderRecord = true;

                var foo = csv.Configuration.AutoMap<MFTData>();

                var o = new TypeConverterOptions
                {
                    DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal,
                    NullValues = {"=\"\""}
                };
                csv.Configuration.TypeConverterOptionsCache.AddOptions<MFTData>(o);

                foo.Map(t => t.Line).Ignore();
                foo.Map(t => t.Tag).Ignore();
                foo.Map(t => t.Timestomped).Name("SI<FN");
                foo.Map(m => m.Created0x10).TypeConverterOption
                    .DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);
                foo.Map(m => m.Created0x30).TypeConverterOption
                    .DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);
                foo.Map(m => m.LastModified0x10).TypeConverterOption
                    .DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);
                foo.Map(m => m.LastModified0x30).TypeConverterOption
                    .DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);
                foo.Map(m => m.LastRecordChange0x10).TypeConverterOption
                    .DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);
                foo.Map(m => m.LastRecordChange0x30).TypeConverterOption
                    .DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);
                foo.Map(m => m.LastAccess0x10).TypeConverterOption
                    .DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);
                foo.Map(m => m.LastAccess0x30).TypeConverterOption
                    .DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);

                csv.Configuration.RegisterClassMap(foo);

                var records = csv.GetRecords<MFTData>();

                var l = LogManager.GetCurrentClassLogger();


                var ln = 1;
                foreach (var record in records)
                {
                    l.Debug($"Line # {ln}, Record: {csv.Context.RawRecord}");
                    record.Line = ln;
                    record.Tag = TaggedLines.Contains(ln);
                    DataList.Add(record);

                    ln += 1;
                }
            }
        }
    }


    public class RbCmdData : IFileSpecData
    {
        public string SourceName { get; set; }
        public string FileType { get; set; }
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public DateTime DeletedOn { get; set; }

        public int Line { get; set; }

        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{SourceName} {FileType} {FileName} {FileSize} {DeletedOn}";
        }
    }


    public class RbCmd : IFileSpec
    {
        public RbCmd()
        {
            //Initialize collections here, one for TaggedLines TLE can add values to, and the collection that TLE will display
            TaggedLines = new List<int>();

            DataList = new BindingList<RbCmdData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "SourceName,FileType,FileName,FileSize,DeletedOn"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from RBCmd";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "acba03f3-9886-4d33-8464-98d11be6e07d";

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
                csv.Configuration.TypeConverterOptionsCache.AddOptions<RbCmdData>(o);

                var foo = csv.Configuration.AutoMap<RbCmdData>();

                foo.Map(m => m.DeletedOn).TypeConverterOption
                    .DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);

                foo.Map(t => t.Line).Ignore();
                foo.Map(t => t.Tag).Ignore();

                csv.Configuration.RegisterClassMap(foo);

                var records = csv.GetRecords<RbCmdData>();
                var l = LogManager.GetCurrentClassLogger();
                var ln = 1;
                foreach (var record in records)
                {
                    l.Debug($"Line # {ln}, Record: {csv.Context.RawRecord}");
                    record.Line = ln;
                    record.Tag = TaggedLines.Contains(ln);
                    DataList.Add(record);

                    ln += 1;
                }
            }
        }
    }


    public class PeCmdData : IFileSpecData
    {
        public string Note { get; set; }
        public string SourceFilename { get; set; }
        public DateTime SourceCreated { get; set; }
        public DateTime SourceModified { get; set; }
        public DateTime SourceAccessed { get; set; }

        public string ExecutableName { get; set; }
        public string Hash { get; set; }
        public long Size { get; set; }
        public string Version { get; set; }
        public int RunCount { get; set; }

        public DateTime LastRun { get; set; }
        public DateTime? PreviousRun0 { get; set; }
        public DateTime? PreviousRun1 { get; set; }
        public DateTime? PreviousRun2 { get; set; }
        public DateTime? PreviousRun3 { get; set; }
        public DateTime? PreviousRun4 { get; set; }
        public DateTime? PreviousRun5 { get; set; }
        public DateTime? PreviousRun6 { get; set; }

        public string Volume0Name { get; set; }
        public string Volume0Serial { get; set; }

        public DateTime? Volume0Created { get; set; }

        public string Volume1Name { get; set; }
        public string Volume1Serial { get; set; }

        public DateTime? Volume1Created { get; set; }

        public string Directories { get; set; }

        public string FilesLoaded { get; set; }
        public bool ParsingError { get; set; }

        //Note	SourceFilename	SourceCreated	SourceModified	SourceAccessed	ExecutableName	Hash	Size	Version	RunCount	LastRun	PreviousRun0	PreviousRun1	PreviousRun2	PreviousRun3	PreviousRun4	PreviousRun5	PreviousRun6	Volume0Name	Volume0Serial	Volume0Created	Volume1Name	Volume1Serial	Volume1Created	Directories	FilesLoaded
        public int Line { get; set; }

        public bool Tag { get; set; }

        public override string ToString()
        {
            return
                $"{Note} {SourceFilename} {SourceCreated} {SourceModified} {SourceAccessed} {ExecutableName} {Hash} {Size} {Version} {RunCount} {LastRun} {PreviousRun0} {PreviousRun1} {PreviousRun2} {PreviousRun2} {PreviousRun3} {PreviousRun4} {PreviousRun5} {PreviousRun6} {Volume0Name} {Volume0Serial} {Volume0Created} {Volume1Name} {Volume1Serial} {Volume1Created} {Directories} {FilesLoaded} {ParsingError}";
        }
    }


    public class PeCmd : IFileSpec
    {
        public PeCmd()
        {
            //Initialize collections here, one for TaggedLines TLE can add values to, and the collection that TLE will display
            TaggedLines = new List<int>();

            DataList = new BindingList<PeCmdData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Note,SourceFilename,SourceCreated,SourceModified,SourceAccessed,ExecutableName,Hash,Size,Version,RunCount,LastRun,PreviousRun0,PreviousRun1,PreviousRun2,PreviousRun3,PreviousRun4,PreviousRun5,PreviousRun6,Volume0Name,Volume0Serial,Volume0Created,Volume1Name,Volume1Serial,Volume1Created,Directories,FilesLoaded,ParsingError"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from PECmd";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "acba03f3-9886-4d33-8464-12d11be6e07d";

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
                csv.Configuration.TypeConverterOptionsCache.AddOptions<PeCmdData>(o);

                var foo = csv.Configuration.AutoMap<PeCmdData>();

                foo.Map(t => t.Line).Ignore();
                foo.Map(t => t.Tag).Ignore();

                csv.Configuration.RegisterClassMap(foo);

                var records = csv.GetRecords<PeCmdData>();
                var l = LogManager.GetCurrentClassLogger();
                var ln = 1;
                foreach (var record in records)
                {
                    l.Debug($"Line # {ln}, Record: {csv.Context.RawRecord}");
                    record.Line = ln;
                    record.Tag = TaggedLines.Contains(ln);
                    DataList.Add(record);

                    ln += 1;
                }
            }
        }
    }


    public class PECmdTimelineData : IFileSpecData
    {
        public DateTime RunTime { get; set; }
        public string ExecutableName { get; set; }
        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{RunTime} {ExecutableName}";
        }
    }


    public class PECmdTimeline : IFileSpec
    {
        public PECmdTimeline()
        {
            //Initialize collections here, one for TaggedLines TLE can add values to, and the collection that TLE will display
            TaggedLines = new List<int>();

            DataList = new BindingList<PECmdTimelineData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "RunTime,ExecutableName"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from PECmd (Timeline)";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "d941bd90-753b-4f2e-964b-2ae8f1bc3812";

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
                csv.Configuration.TypeConverterOptionsCache.AddOptions<PECmdTimelineData>(o);

                var foo = csv.Configuration.AutoMap<PECmdTimelineData>();

                foo.Map(t => t.Line).Ignore();
                foo.Map(t => t.Tag).Ignore();

                csv.Configuration.RegisterClassMap(foo);

                var records = csv.GetRecords<PECmdTimelineData>();
                var l = LogManager.GetCurrentClassLogger();
                var ln = 1;
                foreach (var record in records)
                {
                    l.Debug($"Line # {ln}, Record: {csv.Context.RawRecord}");
                    record.Line = ln;
                    record.Tag = TaggedLines.Contains(ln);
                    DataList.Add(record);

                    ln += 1;
                }
            }
        }
    }


    public class LECmdData : IFileSpecData
    {
        public string SourceFile { get; set; }
        public DateTime? SourceCreated { get; set; }
        public DateTime? SourceModified { get; set; }
        public DateTime? SourceAccessed { get; set; }
        public DateTime? TargetCreated { get; set; }
        public DateTime? TargetModified { get; set; }
        public DateTime? TargetAccessed { get; set; }
        public long FileSize { get; set; }
        public string RelativePath { get; set; }
        public string WorkingDirectory { get; set; }
        public string FileAttributes { get; set; }
        public string HeaderFlags { get; set; }
        public string DriveType { get; set; }
        public string VolumeSerialNumber { get; set; }
        public string VolumeLabel { get; set; }
        public string LocalPath { get; set; }
        public string NetworkPath { get; set; }
        public string CommonPath { get; set; }
        public string Arguments { get; set; }
        public string TargetIDAbsolutePath { get; set; }
        public string TargetMFTEntryNumber { get; set; }
        public string TargetMFTSequenceNumber { get; set; }
        public string MachineID { get; set; }
        public string MachineMACAddress { get; set; }
        public string MACVendor { get; set; }

        public DateTime? TrackerCreatedOn { get; set; }
        public string ExtraBlocksPresent { get; set; }
        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return
                $"{SourceFile} {SourceCreated} {SourceModified} {SourceAccessed} {TargetCreated} {TargetModified} {TargetAccessed} {FileSize} {RelativePath} {WorkingDirectory} {FileAttributes} {HeaderFlags} {DriveType} {VolumeSerialNumber} {VolumeLabel} {LocalPath} {NetworkPath} {CommonPath} {Arguments} {TargetIDAbsolutePath} {TargetMFTEntryNumber} {TargetMFTSequenceNumber} {MachineID} {MachineMACAddress} {MACVendor} {TrackerCreatedOn} {ExtraBlocksPresent}";
        }
    }


    public class LECmd : IFileSpec
    {
        public LECmd()
        {
            //Initialize collections here, one for TaggedLines TLE can add values to, and the collection that TLE will display
            TaggedLines = new List<int>();

            DataList = new BindingList<LECmdData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "SourceFile,SourceCreated,SourceModified,SourceAccessed,TargetCreated,TargetModified,TargetAccessed,FileSize,RelativePath,WorkingDirectory,FileAttributes,HeaderFlags,DriveType,VolumeSerialNumber,VolumeLabel,LocalPath,NetworkPath,CommonPath,Arguments,TargetIDAbsolutePath,TargetMFTEntryNumber,TargetMFTSequenceNumber,MachineID,MachineMACAddress,MACVendor,TrackerCreatedOn,ExtraBlocksPresent"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from LECmd";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "9d37e338-7393-4569-9d71-1d6a9fe714d1";

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
                csv.Configuration.TypeConverterOptionsCache.AddOptions<LECmdData>(o);

                csv.Configuration.PrepareHeaderForMatch = (header, index) => header.Replace(" ", "");

                var foo = csv.Configuration.AutoMap<LECmdData>();

                foo.Map(t => t.Line).Ignore();
                foo.Map(t => t.Tag).Ignore();

                csv.Configuration.RegisterClassMap(foo);

                var records = csv.GetRecords<LECmdData>();
                var l = LogManager.GetCurrentClassLogger();
                var ln = 1;
                foreach (var record in records)
                {
                    l.Debug($"Line # {ln}, Record: {csv.Context.RawRecord}");
                    record.Line = ln;
                    record.Tag = TaggedLines.Contains(ln);
                    DataList.Add(record);

                    ln += 1;
                }
            }
        }
    }

    public class JleCmdAutoData : IFileSpecData
    {
        public string SourceFile { get; set; }
        public DateTime? SourceCreated { get; set; }
        public DateTime? SourceModified { get; set; }
        public DateTime? SourceAccessed { get; set; }

        public string AppId { get; set; }

        public string AppIdDescription { get; set; }

        public int DestListVersion { get; set; }
        public string LastUsedEntryNumber { get; set; }
        public int MRU { get; set; }
        public DateTime? CreationTime { get; set; }
        public DateTime? LastModified { get; set; }
        public string Hostname { get; set; }
        public string MacAddress { get; set; }
        public string Path { get; set; }
        public int InteractionCount { get; set; }
        public string PinStatus { get; set; }
        public string FileBirthDroid { get; set; }
        public string FileDroid { get; set; }
        public string VolumeBirthDroid { get; set; }
        public string VolumeDroid { get; set; }


        public DateTime? TargetCreated { get; set; }
        public DateTime? TargetModified { get; set; }
        public DateTime? TargetAccessed { get; set; }
        public long FileSize { get; set; }
        public string RelativePath { get; set; }
        public string WorkingDirectory { get; set; }
        public string FileAttributes { get; set; }
        public string HeaderFlags { get; set; }
        public string DriveType { get; set; }
        public string VolumeSerialNumber { get; set; }
        public string VolumeLabel { get; set; }
        public string LocalPath { get; set; }
        public string CommonPath { get; set; }
        public string Arguments { get; set; }
        public string TargetIDAbsolutePath { get; set; }
        public string TargetMFTEntryNumber { get; set; }
        public string TargetMFTSequenceNumber { get; set; }
        public string MachineID { get; set; }
        public string MachineMACAddress { get; set; }

        public DateTime? TrackerCreatedOn { get; set; }
        public string ExtraBlocksPresent { get; set; }
        public string Notes { get; set; }
        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return
                $"{SourceFile} {SourceCreated} {SourceModified} {SourceAccessed} {AppId} {AppIdDescription} {DestListVersion} {LastUsedEntryNumber} {MRU} {CreationTime} {LastModified} {Hostname} {MacAddress} {Path} {InteractionCount} {PinStatus} {FileBirthDroid} {FileDroid} {VolumeBirthDroid} {VolumeDroid} {TargetCreated} {TargetModified} {TargetAccessed} {FileSize} {RelativePath} {WorkingDirectory} {FileAttributes} {HeaderFlags} {DriveType} {VolumeSerialNumber}" +
                $" {VolumeLabel} {LocalPath} {CommonPath} {Arguments} {TargetIDAbsolutePath} {TargetMFTEntryNumber} {TargetMFTSequenceNumber} {MachineID} {MachineMACAddress} {TrackerCreatedOn} {ExtraBlocksPresent} {Notes}";
        }
    }


    public class JleCmdAuto : IFileSpec
    {
        public JleCmdAuto()
        {
            //Initialize collections here, one for TaggedLines TLE can add values to, and the collection that TLE will display
            TaggedLines = new List<int>();

            DataList = new BindingList<JleCmdAutoData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "SourceFile,SourceCreated,SourceModified,SourceAccessed,AppId,AppIdDescription,DestListVersion,LastUsedEntryNumber,MRU,EntryNumber,CreationTime,LastModified,Hostname,MacAddress,Path,InteractionCount,PinStatus,FileBirthDroid,FileDroid,VolumeBirthDroid,VolumeDroid,TargetCreated,TargetModified,TargetAccessed,FileSize,RelativePath,WorkingDirectory,FileAttributes,HeaderFlags,DriveType,VolumeSerialNumber,VolumeLabel,LocalPath,CommonPath,TargetIDAbsolutePath,TargetMFTEntryNumber,TargetMFTSequenceNumber,MachineID,MachineMACAddress,TrackerCreatedOn,ExtraBlocksPresent,Arguments,Notes"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from JleCmd for AutomaticDestinations";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "f0111bc1-7626-4e5a-8dbe-4619377c6c93";

        public void ProcessFile(string filename)
        {
            DataList.Clear();

            using (var fileReader = File.OpenText(filename))
            {
                var csv = new CsvReader(fileReader, CultureInfo.InvariantCulture);
                csv.Configuration.HasHeaderRecord = true;


                csv.Configuration.PrepareHeaderForMatch = (header, index) => header.Replace(" ", "");

                var o = new TypeConverterOptions
                {
                    DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal
                };
                csv.Configuration.TypeConverterOptionsCache.AddOptions<JleCmdAutoData>(o);

                var foo = csv.Configuration.AutoMap<JleCmdAutoData>();

                foo.Map(t => t.Line).Ignore();
                foo.Map(t => t.Tag).Ignore();

                csv.Configuration.RegisterClassMap(foo);

                var records = csv.GetRecords<JleCmdAutoData>();
                var l = LogManager.GetCurrentClassLogger();
                var ln = 1;
                foreach (var record in records)
                {
                    l.Debug($"Line # {ln}, Record: {csv.Context.RawRecord}");

                    if (record.FileAttributes.Equals("0"))
                    {
                        record.FileAttributes = string.Empty;
                    }

                    record.Line = ln;
                    record.Tag = TaggedLines.Contains(ln);
                    DataList.Add(record);

                    ln += 1;
                }
            }
        }
    }

    public class JleCmdCustomData : IFileSpecData
    {
        public string SourceFile { get; set; }
        public DateTime? SourceCreated { get; set; }
        public DateTime? SourceModified { get; set; }
        public DateTime? SourceAccessed { get; set; }

        public string AppId { get; set; }

        public string AppIdDescription { get; set; }

        public string EntryName { get; set; }
        public DateTime? TargetCreated { get; set; }
        public DateTime? TargetModified { get; set; }
        public DateTime? TargetAccessed { get; set; }
        public long FileSize { get; set; }
        public string RelativePath { get; set; }
        public string WorkingDirectory { get; set; }
        public string FileAttributes { get; set; }
        public string HeaderFlags { get; set; }
        public string DriveType { get; set; }
        public string VolumeSerialNumber { get; set; }
        public string VolumeLabel { get; set; }
        public string LocalPath { get; set; }
        public string CommonPath { get; set; }
        public string Arguments { get; set; }
        public string TargetIDAbsolutePath { get; set; }
        public string TargetMFTEntryNumber { get; set; }
        public string TargetMFTSequenceNumber { get; set; }
        public string MachineID { get; set; }
        public string MachineMACAddress { get; set; }

        public DateTime? TrackerCreatedOn { get; set; }
        public string ExtraBlocksPresent { get; set; }
        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return
                $"{SourceFile} {SourceCreated} {SourceModified} {SourceAccessed} {AppId} {AppIdDescription} {EntryName} {TargetCreated} {TargetModified} {TargetAccessed} {FileSize} {RelativePath} {WorkingDirectory} {FileAttributes} {HeaderFlags} {DriveType} {VolumeSerialNumber} {VolumeLabel} {LocalPath} {CommonPath} {Arguments} {TargetIDAbsolutePath} {TargetMFTEntryNumber} {TargetMFTSequenceNumber} {MachineID} {MachineMACAddress} {TrackerCreatedOn} {ExtraBlocksPresent}";
        }
    }


    public class JleCmdCustom : IFileSpec
    {
        public JleCmdCustom()
        {
            //Initialize collections here, one for TaggedLines TLE can add values to, and the collection that TLE will display
            TaggedLines = new List<int>();

            DataList = new BindingList<JleCmdCustomData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "SourceFile,SourceCreated,SourceModified,SourceAccessed,AppId,AppIdDescription,EntryName,TargetCreated,TargetModified,TargetAccessed,FileSize,RelativePath,WorkingDirectory,FileAttributes,HeaderFlags,DriveType,VolumeSerialNumber,VolumeLabel,LocalPath,CommonPath,TargetIDAbsolutePath,TargetMFTEntryNumber,TargetMFTSequenceNumber,MachineID,MachineMACAddress,TrackerCreatedOn,ExtraBlocksPresent,Arguments"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from JleCmd for CustomDestinations";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "87087bee-7492-4458-aaa0-237d3659e861";

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
                csv.Configuration.TypeConverterOptionsCache.AddOptions<JleCmdCustomData>(o);

                csv.Configuration.PrepareHeaderForMatch = (header, index) => header.Replace(" ", "");

                var foo = csv.Configuration.AutoMap<JleCmdCustomData>();

                foo.Map(t => t.Line).Ignore();
                foo.Map(t => t.Tag).Ignore();

                csv.Configuration.RegisterClassMap(foo);

                var records = csv.GetRecords<JleCmdCustomData>();
                var l = LogManager.GetCurrentClassLogger();
                var ln = 1;
                foreach (var record in records)
                {
                    l.Debug($"Line # {ln}, Record: {csv.Context.RawRecord}");
                    record.Line = ln;
                    record.Tag = TaggedLines.Contains(ln);
                    DataList.Add(record);

                    ln += 1;
                }
            }
        }
    }

    public class EvtxECmdData : IFileSpecData
    {
        public string RecordNumber { get; set; }
        public string EventRecordId { get; set; }
        public DateTime TimeCreated { get; set; }
        public int EventId { get; set; }
        public string Level { get; set; }
        public string Provider { get; set; }
        public string Channel { get; set; }
        public int ProcessId { get; set; }
        public int ThreadId { get; set; }
        public string Computer { get; set; }
        public string UserId { get; set; }
        public string MapDescription { get; set; }
        public string UserName { get; set; }
        public string RemoteHost { get; set; }
        public string PayloadData1 { get; set; }
        public string PayloadData2 { get; set; }
        public string PayloadData3 { get; set; }
        public string PayloadData4 { get; set; }
        public string PayloadData5 { get; set; }
        public string PayloadData6 { get; set; }
        public string ExecutableInfo { get; set; }
        public string SourceFile { get; set; }
        public string Payload { get; set; }
        public string Keywords { get; set; }
        public bool HiddenRecord { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return
                $"{RecordNumber} {EventRecordId} {TimeCreated} {EventId} {Level} {Provider} {Channel} {ProcessId} {ThreadId} {Computer} {UserId} {MapDescription} {UserName} {RemoteHost} {PayloadData1} {PayloadData2} {PayloadData3} {PayloadData4} {PayloadData5} {PayloadData6} {ExecutableInfo} {SourceFile} {Keywords} {HiddenRecord} {Payload}";
        }
    }


    public class EvtxECmd : IFileSpec
    {
        public EvtxECmd()
        {
            //Initialize collections here, one for TaggedLines TLE can add values to, and the collection that TLE will display
            TaggedLines = new List<int>();

            DataList = new BindingList<EvtxECmdData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "RecordNumber,EventRecordId,TimeCreated,EventId,Level,Provider,Channel,ProcessId,ThreadId,Computer,UserId,MapDescription,ChunkNumber,UserName,RemoteHost,PayloadData1,PayloadData2,PayloadData3,PayloadData4,PayloadData5,PayloadData6,ExecutableInfo,HiddenRecord,SourceFile,Payload,Keywords,ExtraDataOffset"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from EvtECmd";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "73a98fdf-5567-4c3f-9a61-a3c00a63892e";

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
                csv.Configuration.TypeConverterOptionsCache.AddOptions<EvtxECmdData>(o);

                var foo = csv.Configuration.AutoMap<EvtxECmdData>();

                foo.Map(t => t.Line).Ignore();
                foo.Map(t => t.Tag).Ignore();
                foo.Map(t => t.Payload).Optional();
                foo.Map(t => t.HiddenRecord).Optional();
                foo.Map(t => t.Keywords).Optional();

                csv.Configuration.RegisterClassMap(foo);

                var records = csv.GetRecords<EvtxECmdData>();
                var l = LogManager.GetCurrentClassLogger();
                var ln = 1;
                foreach (var record in records)
                {
                    l.Debug($"Line # {ln}, Record: {csv.Context.RawRecord}");
                    record.Line = ln;
                    record.Tag = TaggedLines.Contains(ln);
                    DataList.Add(record);

                    ln += 1;
                }
            }
        }
    }

    public class RecmdBatchData : IFileSpecData
    {
        public string HivePath { get; set; }
        public string HiveType { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string KeyPath { get; set; }
        public string ValueName { get; set; }

        public string ValueType { get; set; }
        public string ValueData { get; set; }
        public string ValueData2 { get; set; }
        public string ValueData3 { get; set; }

        public string Comment { get; set; }
        public bool Recursive { get; set; }
        public bool Deleted { get; set; }


        public DateTime LastWriteTimestamp { get; set; }

        public string PluginDetailFile { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return
                $"{HivePath} {HiveType} {Description} {Category} {KeyPath} {ValueName} {ValueType} {ValueData} {ValueData2} {ValueData3} {Comment} {Recursive} {Deleted} {LastWriteTimestamp} {PluginDetailFile}";
        }
    }


    public class RecmdBatch : IFileSpec
    {
        public RecmdBatch()
        {
            //Initialize collections here, one for TaggedLines TLE can add values to, and the collection that TLE will display
            TaggedLines = new List<int>();

            DataList = new BindingList<RecmdBatchData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "hivepath,hivetype,description,category,keypath,valuename,valuetype,valuedata,valuedata2,valuedata3,comment,recursive,deleted,lastwritetimestamp,plugindetailfile"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from RECmd batch";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "5c6ee898-eab0-47bf-b8a0-74a325a7dacb";

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
                csv.Configuration.TypeConverterOptionsCache.AddOptions<RecmdBatchData>(o);

                var foo = csv.Configuration.AutoMap<RecmdBatchData>();

                foo.Map(t => t.Line).Ignore();
                foo.Map(t => t.Tag).Ignore();

                csv.Configuration.RegisterClassMap(foo);

                var records = csv.GetRecords<RecmdBatchData>();
                var l = LogManager.GetCurrentClassLogger();
                var ln = 1;
                foreach (var record in records)
                {
                    l.Debug($"Line # {ln}, Record: {csv.Context.RawRecord}");
                    record.Line = ln;
                    record.Tag = TaggedLines.Contains(ln);
                    DataList.Add(record);

                    ln += 1;
                }
            }
        }
    }


    public class SbeCmdData : IFileSpecData
    {
        public string BagPath { get; set; }
        public int Slot { get; set; }
        public int NodeSlot { get; set; }
        public int MRUPosition { get; set; }
        public string AbsolutePath { get; set; }
        public string ShellType { get; set; }
        public string Value { get; set; }
        public int ChildBags { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public DateTime? AccessedOn { get; set; }
        public DateTime? LastWriteTime { get; set; }
        public long? MFTEntry { get; set; }
        public int? MFTSequenceNumber { get; set; }
        public int ExtensionBlockCount { get; set; }
        public DateTime? FirstInteracted { get; set; }
        public DateTime? LastInteracted { get; set; }

        public bool HasExplored { get; set; }

        public string Miscellaneous { get; set; }

        public int Line { get; set; }

        public bool Tag { get; set; }

        public override string ToString()
        {
            return
                $"{BagPath} {Slot} {NodeSlot} {MRUPosition} {AbsolutePath} {ShellType} {Value} {ChildBags} {CreatedOn} {ModifiedOn} {AccessedOn} {MFTEntry} {MFTSequenceNumber} {ExtensionBlockCount} {FirstInteracted} {LastInteracted} {HasExplored} {Miscellaneous} {LastWriteTime}";
        }
    }


    public class SbeCmd : IFileSpec
    {
        public SbeCmd()
        {
            //Initialize collections here, one for TaggedLines TLE can add values to, and the collection that TLE will display
            TaggedLines = new List<int>();

            DataList = new BindingList<SbeCmdData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "bagpath,slot,nodeslot,mruposition,absolutepath,shelltype,value,childbags,createdon,modifiedon,accessedon,lastwritetime,mftentry,mftsequencenumber,extensionblockcount,firstinteracted,lastinteracted,hasexplored,miscellaneous"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from SBECmd";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "f5f8b988-c2f1-457b-8c77-653619156713";

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
                csv.Configuration.TypeConverterOptionsCache.AddOptions<SbeCmdData>(o);

                csv.Configuration.PrepareHeaderForMatch = (header, index) => header.Replace(" ", "");

                var foo = csv.Configuration.AutoMap<SbeCmdData>();

                foo.Map(t => t.Line).Ignore();
                foo.Map(t => t.Tag).Ignore();

                csv.Configuration.RegisterClassMap(foo);

                var records = csv.GetRecords<SbeCmdData>();
                var l = LogManager.GetCurrentClassLogger();
                var ln = 1;
                foreach (var record in records)
                {
                    l.Debug($"Line # {ln}, Record: {csv.Context.RawRecord}");
                    record.Line = ln;
                    record.Tag = TaggedLines.Contains(ln);
                    DataList.Add(record);

                    ln += 1;
                }
            }
        }
    }


    public class AppCompatCacheData : IFileSpecData
    {
        public int ControlSet { get; set; }
        public int CacheEntryPosition { get; set; }
        public string Path { get; set; }
        public DateTime? LastModifiedTimeUTC { get; set; }

        public string Executed { get; set; }
        public bool Duplicate { get; set; }
        public string SourceFile { get; set; }


        //ControlSet	CacheEntryPosition	Path	LastModifiedTimeUTC	Executed
        public int Line { get; set; }

        public bool Tag { get; set; }

        public override string ToString()
        {
            return
                $"{ControlSet} {CacheEntryPosition} {Path} {LastModifiedTimeUTC} {Executed} {Duplicate} {SourceFile}";
        }
    }


    public class AppCompatCache : IFileSpec
    {
        public AppCompatCache()
        {
            //Initialize collections here, one for TaggedLines TLE can add values to, and the collection that TLE will display
            TaggedLines = new List<int>();

            DataList = new BindingList<AppCompatCacheData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "ControlSet,CacheEntryPosition,Path,LastModifiedTimeUTC,Executed,Duplicate,SourceFile"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from AppCompatCacheParser";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "24893919-7b37-42e8-9e8a-b49a1254f36d";

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
                csv.Configuration.TypeConverterOptionsCache.AddOptions<AppCompatCacheData>(o);

                var foo = csv.Configuration.AutoMap<AppCompatCacheData>();

                foo.Map(t => t.Line).Ignore();
                foo.Map(t => t.Tag).Ignore();

                csv.Configuration.RegisterClassMap(foo);

                var records = csv.GetRecords<AppCompatCacheData>();
                var l = LogManager.GetCurrentClassLogger();
                var ln = 1;
                foreach (var record in records)
                {
                    l.Debug($"Line # {ln}, Record: {csv.Context.RawRecord}");
                    record.Line = ln;
                    record.Tag = TaggedLines.Contains(ln);
                    DataList.Add(record);

                    ln += 1;
                }
            }
        }
    }

    public class ActivityPackageData : IFileSpecData
    {
        public string Id { get; set; }
        public string Platform { get; set; }
        public string Name { get; set; }
        public string AdditionalInformation { get; set; }

        public DateTime Expires { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{Id} {Platform} {Name} {AdditionalInformation} {Expires}";
        }
    }


    public class ActivityPackage : IFileSpec
    {
        public ActivityPackage()
        {
            //Initialize collections here, one for TaggedLines TLE can add values to, and the collection that TLE will display
            TaggedLines = new List<int>();

            DataList = new BindingList<ActivityPackageData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Id,Platform,Name,AdditionalInformation,Expires"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from WxTCmd for ActivityPackage";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "1f676ce9-54c8-46e4-bf5c-d26c922243d2";

        public void ProcessFile(string filename)
        {
            DataList.Clear();

            using (var fileReader = File.OpenText(filename))
            {
                var csv = new CsvReader(fileReader, CultureInfo.InvariantCulture);
                csv.Configuration.HasHeaderRecord = true;
                var foo = csv.Configuration.AutoMap<ActivityPackageData>();

                var o = new TypeConverterOptions
                {
                    DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal
                };
                csv.Configuration.TypeConverterOptionsCache.AddOptions<ActivityPackageData>(o);

                foo.Map(t => t.Line).Ignore();
                foo.Map(t => t.Tag).Ignore();

                foo.Map(m => m.Expires).TypeConverterOption
                    .DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);

                csv.Configuration.RegisterClassMap(foo);

                var records = csv.GetRecords<ActivityPackageData>();
                var l = LogManager.GetCurrentClassLogger();
                var ln = 1;
                foreach (var record in records)
                {
                    l.Debug($"Line # {ln}, Record: {csv.Context.RawRecord}");
                    record.Line = ln;
                    record.Tag = TaggedLines.Contains(ln);
                    DataList.Add(record);

                    ln += 1;
                }
            }
        }
    }

    public class ActivityData : IFileSpecData
    {
        public string Id { get; set; }
        public string Executable { get; set; }
        public string DisplayText { get; set; }
        public string Payload { get; set; }
        public string ClipboardPayload { get; set; }
        public string ContentInfo { get; set; }
        public DateTime LastModifiedTime { get; set; }
        public DateTime ExpirationTime { get; set; }
        public DateTime? CreatedInCloud { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        public TimeSpan? Duration { get; set; }

        public DateTime LastModifiedOnClient { get; set; }
        public DateTime? OriginalLastModifiedOnClient { get; set; }

        public int ActivityTypeOrg { get; set; }
        public string ActivityType { get; set; }

        public bool IsLocalOnly { get; set; }

        public int ETag { get; set; }

        public string PackageIdHash { get; set; }


        public string PlatformDeviceId { get; set; }

        public string DevicePlatform { get; set; }
        public string TimeZone { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return
                $"{Id} {Executable} {DisplayText} {Payload} {ClipboardPayload} {ContentInfo} {LastModifiedTime} {ExpirationTime} {CreatedInCloud} {StartTime} {EndTime} {Duration} {LastModifiedOnClient} {OriginalLastModifiedOnClient} {ActivityType} {ActivityTypeOrg} {IsLocalOnly} {ETag} {PackageIdHash} {PlatformDeviceId} {DevicePlatform} {TimeZone}";
        }
    }


    public class Activity : IFileSpec
    {
        public Activity()
        {
            //Initialize collections here, one for TaggedLines TLE can add values to, and the collection that TLE will display
            TaggedLines = new List<int>();

            DataList = new BindingList<ActivityData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Id,ActivityTypeOrg,ActivityType,Executable,DisplayText,ContentInfo,Payload,ClipboardPayload,StartTime,EndTime,Duration,LastModifiedTime,LastModifiedOnClient,OriginalLastModifiedOnClient,ExpirationTime,CreatedInCloud,IsLocalOnly,ETag,PackageIdHash,PlatformDeviceId,DevicePlatform,TimeZone"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from WxTCmd for Activity";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "1f676ce9-54c8-46e4-bf5c-d36c972342d5";

        public void ProcessFile(string filename)
        {
            DataList.Clear();

            using (var fileReader = File.OpenText(filename))
            {
                var csv = new CsvReader(fileReader, CultureInfo.InvariantCulture);
                csv.Configuration.HasHeaderRecord = true;

                var foo = csv.Configuration.AutoMap<ActivityData>();

                var o = new TypeConverterOptions
                {
                    DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal
                };
                csv.Configuration.TypeConverterOptionsCache.AddOptions<ActivityData>(o);

                foo.Map(t => t.Line).Ignore();
                foo.Map(t => t.Tag).Ignore();

                foo.Map(m => m.CreatedInCloud).TypeConverterOption
                    .DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);

                foo.Map(m => m.EndTime).TypeConverterOption
                    .DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);

                foo.Map(m => m.ExpirationTime).TypeConverterOption
                    .DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);

                foo.Map(m => m.LastModifiedOnClient).TypeConverterOption
                    .DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);

                foo.Map(m => m.LastModifiedTime).TypeConverterOption
                    .DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);

                foo.Map(m => m.OriginalLastModifiedOnClient).TypeConverterOption
                    .DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);

                foo.Map(m => m.StartTime).TypeConverterOption
                    .DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);

                csv.Configuration.RegisterClassMap(foo);

                var records = csv.GetRecords<ActivityData>();
                var l = LogManager.GetCurrentClassLogger();
                var ln = 1;
                foreach (var record in records)
                {
                    l.Debug($"Line # {ln}, Record: {csv.Context.RawRecord}");
                    record.Line = ln;
                    record.Tag = TaggedLines.Contains(ln);
                    DataList.Add(record);

                    ln += 1;
                }
            }
        }
    }

    public class ActivityOperationData : IFileSpecData
    {
        public string Id { get; set; }

        public int OperationOrder { get; set; }
        public int OperationType { get; set; }

        public string AppId { get; set; }

        public string Executable { get; set; }
        public string Description { get; set; }

        public DateTime? StartTime { get; set; }
        public string DisplayText { get; set; }
        public string ClipboardPayload { get; set; }
        public string ContentInfo { get; set; }


        public string DevicePlatform { get; set; }
        public string TimeZone { get; set; }


        public int ActivityTypeOrg { get; set; }
        public string ActivityType { get; set; }

        public TimeSpan? Duration { get; set; }

        public DateTime LastModifiedTime { get; set; }
        public DateTime ExpirationTime { get; set; }

        public string Payload { get; set; }

        public DateTime CreatedTime { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime LastModifiedTimeOnClient { get; set; }
        public DateTime OperationExpirationTime { get; set; }

        public string PlatformDeviceId { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return
                $"{OperationOrder} {OperationType} {AppId} {Executable} {Description} {StartTime} {DisplayText} {ClipboardPayload} {ContentInfo} {DevicePlatform} {TimeZone} {ActivityTypeOrg} {ActivityType} {Duration} {LastModifiedTime} {ExpirationTime} {Payload} {CreatedTime} {EndTime} {LastModifiedTimeOnClient} {OperationExpirationTime} {PlatformDeviceId}";
        }
    }


    public class ActivityOperation : IFileSpec
    {
        public ActivityOperation()
        {
            //Initialize collections here, one for TaggedLines TLE can add values to, and the collection that TLE will display
            TaggedLines = new List<int>();

            DataList = new BindingList<ActivityOperationData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Id,ActivityTypeOrg,ActivityType,Executable,DisplayText,ContentInfo,Payload,ClipboardPayload,StartTime,EndTime,Duration,LastModifiedTime,LastModifiedTimeOnClient,CreatedTime,ExpirationTime,OperationExpirationTime,OperationOrder,AppId,OperationType,Description,PlatformDeviceId,DevicePlatform,TimeZone"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from WxTCmd for ActivityOperation";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "1f676ce9-54c8-46e4-bf5c-d26c972342d4";

        public void ProcessFile(string filename)
        {
            DataList.Clear();

            using (var fileReader = File.OpenText(filename))
            {
                var csv = new CsvReader(fileReader, CultureInfo.InvariantCulture);
                csv.Configuration.HasHeaderRecord = true;

                var foo = csv.Configuration.AutoMap<ActivityOperationData>();

                var o = new TypeConverterOptions
                {
                    DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal
                };
                csv.Configuration.TypeConverterOptionsCache.AddOptions<ActivityOperationData>(o);

                foo.Map(t => t.Line).Ignore();
                foo.Map(t => t.Tag).Ignore();

                foo.Map(m => m.CreatedTime).TypeConverterOption
                    .DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);

                foo.Map(m => m.EndTime).TypeConverterOption
                    .DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);

                foo.Map(m => m.ExpirationTime).TypeConverterOption
                    .DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);

                foo.Map(m => m.LastModifiedTimeOnClient).TypeConverterOption
                    .DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);

                foo.Map(m => m.LastModifiedTime).TypeConverterOption
                    .DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);

                foo.Map(m => m.OperationExpirationTime).TypeConverterOption
                    .DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);

                foo.Map(m => m.StartTime).TypeConverterOption
                    .DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);

                csv.Configuration.RegisterClassMap(foo);

                var records = csv.GetRecords<ActivityOperationData>();
                var l = LogManager.GetCurrentClassLogger();
                var ln = 1;
                foreach (var record in records)
                {
                    l.Debug($"Line # {ln}, Record: {csv.Context.RawRecord}");
                    record.Line = ln;
                    record.Tag = TaggedLines.Contains(ln);
                    DataList.Add(record);

                    ln += 1;
                }
            }
        }
    }


    public class AmcacheParserProgramsData : IFileSpecData
    {
        public string ProgramID { get; set; }
        public DateTime LastWriteTimestamp { get; set; }
        public string ProgramName_0 { get; set; }
        public string ProgramVersion_1 { get; set; }
        public string VendorName_2 { get; set; }
        public DateTime? InstallDateEpoch_a { get; set; }
        public DateTime? InstallDateEpoch_b { get; set; }
        public string LanguageCode_3 { get; set; }
        public string InstallSource_6 { get; set; }
        public string UninstallRegistryKey_7 { get; set; }
        public string PathsList_d { get; set; }
        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return
                $"{ProgramID} {LastWriteTimestamp} {ProgramName_0} {ProgramVersion_1} {VendorName_2} {InstallDateEpoch_a} {InstallDateEpoch_b} {LanguageCode_3} {InstallSource_6} {UninstallRegistryKey_7} {PathsList_d}";
        }
    }


    public class AmcacheParserPrograms : IFileSpec
    {
        public AmcacheParserPrograms()
        {
            //Initialize collections here, one for TaggedLines TLE can add values to, and the collection that TLE will display
            TaggedLines = new List<int>();

            DataList = new BindingList<AmcacheParserProgramsData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "ProgramID,LastWriteTimestamp,ProgramName_0,ProgramVersion_1,VendorName_2,InstallDateEpoch_a,InstallDateEpoch_b,LanguageCode_3,InstallSource_6,UninstallRegistryKey_7,PathsList_d,UnknownGuid_10,UnknownGuid_12,UninstallGuid_11,UnknownDword_5,UnknownDword_13,UnknownDword_14,UnknownDword_15,UnknownBytes_16,UnknownQWord_17,UnknownDword_18,UninstallGuid_f"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from AmcacheParser for old Programs format";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "344f6e65-b127-4a57-b4f4-866839406256";

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
                csv.Configuration.TypeConverterOptionsCache.AddOptions<AmcacheParserProgramsData>(o);

                var foo = csv.Configuration.AutoMap<AmcacheParserProgramsData>();

                foo.Map(t => t.Line).Ignore();
                foo.Map(t => t.Tag).Ignore();

                csv.Configuration.RegisterClassMap(foo);

                var records = csv.GetRecords<AmcacheParserProgramsData>();
                var l = LogManager.GetCurrentClassLogger();
                var ln = 1;
                foreach (var record in records)
                {
                    l.Debug($"Line # {ln}, Record: {csv.Context.RawRecord}");
                    record.Line = ln;
                    record.Tag = TaggedLines.Contains(ln);
                    DataList.Add(record);

                    ln += 1;
                }
            }
        }
    }

    public class AmcacheParserFilesData : IFileSpecData
    {
        public string ProgramName { get; set; }
        public string ProgramID { get; set; }
        public string VolumeID { get; set; }
        public DateTime VolumeIDLastWriteTimestamp { get; set; }
        public string FileID { get; set; }
        public DateTime FileIDLastWriteTimestamp { get; set; }
        public string SHA1 { get; set; }
        public string FullPath { get; set; }
        public string FileExtension { get; set; }
        public long MFTEntryNumber { get; set; }
        public int MFTSequenceNumber { get; set; }
        public string FileSize { get; set; }
        public string FileVersionString { get; set; }
        public string FileVersionNumber { get; set; }
        public string FileDescription { get; set; }
        public string SizeOfImage { get; set; }
        public string PEHeaderHash { get; set; }
        public string PEHeaderChecksum { get; set; }

        public DateTime? Created { get; set; }
        public DateTime? LastModified { get; set; }
        public DateTime? LastModifiedStore { get; set; }
        public DateTime? LinkDate { get; set; }
        public string LanguageID { get; set; }
        public string ProductName { get; set; }
        public string CompanyName { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return
                $"{ProgramName} {ProgramID} {VolumeID} {VolumeIDLastWriteTimestamp} {FileID} {FileIDLastWriteTimestamp} {SHA1} {FullPath} {FileExtension} {MFTEntryNumber} {MFTSequenceNumber} {FileSize} {FileVersionString} {FileVersionNumber} {FileDescription} {SizeOfImage} {PEHeaderHash} {PEHeaderChecksum} {Created} {LastModified} {LastModifiedStore} {LinkDate} {LanguageID} {ProductName} {CompanyName}";
        }
    }

    public class AmcacheParserFiles : IFileSpec
    {
        public AmcacheParserFiles()
        {
            //Initialize collections here, one for TaggedLines TLE can add values to, and the collection that TLE will display
            TaggedLines = new List<int>();

            DataList = new BindingList<AmcacheParserFilesData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "ProgramName,ProgramID,VolumeID,VolumeIDLastWriteTimestamp,FileID,FileIDLastWriteTimestamp,SHA1,FullPath,FileExtension,MFTEntryNumber,MFTSequenceNumber,FileSize,FileVersionString,FileVersionNumber,FileDescription,SizeOfImage,PEHeaderHash,PEHeaderChecksum,BinProductVersion,BinFileVersion,LinkerVersion,BinaryType,IsLocal,GuessProgramID,Created,LastModified,LastModifiedStore,LinkDate,LanguageID,ProductName,CompanyName,SwitchBackContext"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from AmcacheParser for old File Entries format";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "ed1c0ddb-ffb3-4d2e-b60d-fd4fb859ef4f";

        public void ProcessFile(string filename)
        {
            DataList.Clear();

            using (var fileReader = File.OpenText(filename))
            {
                var csv = new CsvReader(fileReader, CultureInfo.InvariantCulture);
                csv.Configuration.HasHeaderRecord = true;

                var foo = csv.Configuration.AutoMap<AmcacheParserFilesData>();

                var o = new TypeConverterOptions
                {
                    DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal
                };
                csv.Configuration.TypeConverterOptionsCache.AddOptions<AmcacheParserFilesData>(o);

                foo.Map(t => t.Line).Ignore();
                foo.Map(t => t.Tag).Ignore();

                csv.Configuration.RegisterClassMap(foo);

                var records = csv.GetRecords<AmcacheParserFilesData>();
                var l = LogManager.GetCurrentClassLogger();
                var ln = 1;
                foreach (var record in records)
                {
                    l.Debug($"Line # {ln}, Record: {csv.Context.RawRecord}");
                    record.Line = ln;
                    record.Tag = TaggedLines.Contains(ln);
                    DataList.Add(record);

                    ln += 1;
                }
            }
        }
    }


    public class AmcacheParserNewFilesData : IFileSpecData
    {
        public string ApplicationName { get; set; }
        public string ProgramId { get; set; }
        public DateTime FileKeyLastWriteTimestamp { get; set; }
        public string SHA1 { get; set; }
        public bool IsOsComponent { get; set; }
        public string FullPath { get; set; }
        public string Name { get; set; }
        public string FileExtension { get; set; }
        public DateTime? LinkDate { get; set; }
        public string ProductName { get; set; }
        public long Size { get; set; }
        public string Version { get; set; }
        public string ProductVersion { get; set; }
        public string LongPathHash { get; set; }
        public string BinaryType { get; set; }
        public bool IsPeFile { get; set; }
        public string BinFileVersion { get; set; }
        public string BinProductVersion { get; set; }
        public int Language { get; set; }
        public long Usn { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return
                $"{ApplicationName} {Name} {ProgramId} {FileKeyLastWriteTimestamp} {SHA1} {IsOsComponent} {FullPath} {FileExtension} {LinkDate} {ProductName} {Size} {Version} {ProductVersion} {LongPathHash} {BinaryType} {IsPeFile} {BinFileVersion} {BinProductVersion} {Language} {Usn}";
        }
    }


    public class AmcacheParserNewFiles : IFileSpec
    {
        public AmcacheParserNewFiles()
        {
            //Initialize collections here, one for TaggedLines TLE can add values to, and the collection that TLE will display
            TaggedLines = new List<int>();

            DataList = new BindingList<AmcacheParserNewFilesData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "ApplicationName,ProgramId,FileKeyLastWriteTimestamp,SHA1,IsOsComponent,FullPath,Name,FileExtension,LinkDate,ProductName,Size,Version,ProductVersion,LongPathHash,BinaryType,IsPeFile,BinFileVersion,BinProductVersion,Language,Usn,Description"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from AmcacheParser for new File Entries format";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "aa4517f0-c1a3-4591-8591-069b57cf9249";

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
                csv.Configuration.TypeConverterOptionsCache.AddOptions<AmcacheParserNewFilesData>(o);

                var foo = csv.Configuration.AutoMap<AmcacheParserNewFilesData>();

                foo.Map(t => t.Line).Ignore();
                foo.Map(t => t.Tag).Ignore();
                foo.Map(t => t.Usn).Ignore();

                csv.Configuration.RegisterClassMap(foo);

                var records = csv.GetRecords<AmcacheParserNewFilesData>();
                var l = LogManager.GetCurrentClassLogger();
                var ln = 1;
                foreach (var record in records)
                {
                    l.Debug($"Line # {ln}, Record: {csv.Context.RawRecord}");
                    record.Line = ln;
                    record.Tag = TaggedLines.Contains(ln);
                    DataList.Add(record);

                    ln += 1;
                }
            }
        }
    }


    public class AmcacheParserNewDevicePnPData : IFileSpecData
    {
        public string KeyName { get; set; }
        public DateTime KeyLastWriteTimestamp { get; set; }
        public string BusReportedDescription { get; set; }
        public string Class { get; set; }
        public string ClassGuid { get; set; }
        public string Compid { get; set; }
        public string ContainerId { get; set; }
        public string Description { get; set; }
        public string DriverId { get; set; }
        public string DriverPackageStrongName { get; set; }
        public string DriverName { get; set; }
        public DateTime? DriverVerDate { get; set; }
        public string DriverVerVersion { get; set; }
        public string Enumerator { get; set; }
        public string HWID { get; set; }
        public string Inf { get; set; }
        public string InstallState { get; set; }
        public string Manufacturer { get; set; }
        public string MatchingId { get; set; }
        public string Model { get; set; }
        public string ParentId { get; set; }
        public string ProblemCode { get; set; }
        public string Provider { get; set; }
        public string Service { get; set; }
        public string Stackid { get; set; }
        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return
                $"{KeyName} {KeyLastWriteTimestamp} {BusReportedDescription} {Class} {ClassGuid} {Compid} {ContainerId} {Description} {DriverId} {DriverPackageStrongName} {DriverName} {DriverVerDate} {DriverVerVersion} {Enumerator} {HWID} {Inf} {InstallState} {Manufacturer} {MatchingId} {Model} {ParentId} {ProblemCode} {Provider} {Service} {Stackid}";
        }
    }


    public class AmcacheParserNewDevicePnP : IFileSpec
    {
        public AmcacheParserNewDevicePnP()
        {
            //Initialize collections here, one for TaggedLines TLE can add values to, and the collection that TLE will display
            TaggedLines = new List<int>();

            DataList = new BindingList<AmcacheParserNewDevicePnPData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "KeyName,KeyLastWriteTimestamp,BusReportedDescription,Class,ClassGuid,Compid,ContainerId,Description,DriverId,DriverPackageStrongName,DriverName,DriverVerDate,DriverVerVersion,Enumerator,HWID,Inf,InstallState,Manufacturer,MatchingId,Model,ParentId,ProblemCode,Provider,Service,Stackid"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from AmcacheParser for new Device PnP format";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "bb16e468-f637-4872-b40f-e288fd98ed91";

        public void ProcessFile(string filename)
        {
            DataList.Clear();

            using (var fileReader = File.OpenText(filename))
            {
                var csv = new CsvReader(fileReader, CultureInfo.InvariantCulture);
                csv.Configuration.HasHeaderRecord = true;

                var foo = csv.Configuration.AutoMap<AmcacheParserNewDevicePnPData>();

                var o = new TypeConverterOptions
                {
                    DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal
                };
                csv.Configuration.TypeConverterOptionsCache.AddOptions<AmcacheParserNewDevicePnPData>(o);

                foo.Map(t => t.Line).Ignore();
                foo.Map(t => t.Tag).Ignore();

                csv.Configuration.RegisterClassMap(foo);

                var records = csv.GetRecords<AmcacheParserNewDevicePnPData>();
                var l = LogManager.GetCurrentClassLogger();
                var ln = 1;
                foreach (var record in records)
                {
                    l.Debug($"Line # {ln}, Record: {csv.Context.RawRecord}");
                    record.Line = ln;
                    record.Tag = TaggedLines.Contains(ln);
                    DataList.Add(record);

                    ln += 1;
                }
            }
        }
    }

    public class AmcacheParserDeviceContainerData : IFileSpecData
    {
        public string KeyName { get; set; }
        public DateTime KeyLastWriteTimestamp { get; set; }
        public string Categories { get; set; }
        public string DiscoveryMethod { get; set; }
        public string FriendlyName { get; set; }
        public string Icon { get; set; }
        public bool IsActive { get; set; }
        public bool IsConnected { get; set; }
        public bool IsMachineContainer { get; set; }
        public bool IsNetworked { get; set; }
        public bool IsPaired { get; set; }
        public string Manufacturer { get; set; }
        public string ModelId { get; set; }
        public string ModelName { get; set; }
        public string ModelNumber { get; set; }
        public string PrimaryCategory { get; set; }
        public int State { get; set; }
        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return
                $"{KeyName} {KeyLastWriteTimestamp} {Categories} {DiscoveryMethod} {FriendlyName} {Icon} {IsActive} {IsConnected} {IsMachineContainer} {IsNetworked} {IsPaired} {Manufacturer} {ModelId} {ModelName} {ModelNumber} {PrimaryCategory} {State}";
        }
    }


    public class AmcacheParserDeviceContainer : IFileSpec
    {
        public AmcacheParserDeviceContainer()
        {
            //Initialize collections here, one for TaggedLines TLE can add values to, and the collection that TLE will display
            TaggedLines = new List<int>();

            DataList = new BindingList<AmcacheParserDeviceContainerData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "KeyName,KeyLastWriteTimestamp,Categories,DiscoveryMethod,FriendlyName,Icon,IsActive,IsConnected,IsMachineContainer,IsNetworked,IsPaired,Manufacturer,ModelId,ModelName,ModelNumber,PrimaryCategory,State"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from AmcacheParser for new File Device Container format";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "b99337ec-c635-4ebb-8175-3fa72ec12d47";

        public void ProcessFile(string filename)
        {
            DataList.Clear();

            using (var fileReader = File.OpenText(filename))
            {
                var csv = new CsvReader(fileReader, CultureInfo.InvariantCulture);
                csv.Configuration.HasHeaderRecord = true;

                var foo = csv.Configuration.AutoMap<AmcacheParserDeviceContainerData>();

                var o = new TypeConverterOptions
                {
                    DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal
                };
                csv.Configuration.TypeConverterOptionsCache.AddOptions<AmcacheParserDeviceContainerData>(o);

                foo.Map(t => t.Line).Ignore();
                foo.Map(t => t.Tag).Ignore();

                csv.Configuration.RegisterClassMap(foo);

                var records = csv.GetRecords<AmcacheParserDeviceContainerData>();
                var l = LogManager.GetCurrentClassLogger();
                var ln = 1;
                foreach (var record in records)
                {
                    l.Debug($"Line # {ln}, Record: {csv.Context.RawRecord}");
                    record.Line = ln;
                    record.Tag = TaggedLines.Contains(ln);
                    DataList.Add(record);

                    ln += 1;
                }
            }
        }
    }

    public class AmcacheParserDriverPackageData : IFileSpecData
    {
        public string KeyName { get; set; }
        public DateTime KeyLastWriteTimestamp { get; set; }
        public DateTime Date { get; set; }
        public string Class { get; set; }
        public string Directory { get; set; }
        public bool DriverInBox { get; set; }
        public string Hwids { get; set; }
        public string Inf { get; set; }
        public string Provider { get; set; }
        public string SubmissionId { get; set; }
        public string SYSFILE { get; set; }
        public string Version { get; set; }
        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return
                $"{KeyName} {KeyLastWriteTimestamp} {Date} {Class} {Directory} {DriverInBox} {Hwids} {Inf} {Provider} {SubmissionId} {SYSFILE} {Version}";
        }
    }


    public class AmcacheParserDriverPackage : IFileSpec
    {
        public AmcacheParserDriverPackage()
        {
            //Initialize collections here, one for TaggedLines TLE can add values to, and the collection that TLE will display
            TaggedLines = new List<int>();

            DataList = new BindingList<AmcacheParserDriverPackageData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "KeyName,KeyLastWriteTimestamp,Date,Class,Directory,DriverInBox,Hwids,Inf,Provider,SubmissionId,SYSFILE,Version"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from AmcacheParser for new Driver Package format";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "e16beaa2-349c-4dbd-8f63-038712789ed8";

        public void ProcessFile(string filename)
        {
            DataList.Clear();

            using (var fileReader = File.OpenText(filename))
            {
                var csv = new CsvReader(fileReader, CultureInfo.InvariantCulture);
                csv.Configuration.HasHeaderRecord = true;

                var foo = csv.Configuration.AutoMap<AmcacheParserDriverPackageData>();

                var o = new TypeConverterOptions
                {
                    DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal
                };
                csv.Configuration.TypeConverterOptionsCache.AddOptions<AmcacheParserDriverPackageData>(o);

                foo.Map(t => t.Line).Ignore();
                foo.Map(t => t.Tag).Ignore();

                csv.Configuration.RegisterClassMap(foo);

                var records = csv.GetRecords<AmcacheParserDriverPackageData>();
                var l = LogManager.GetCurrentClassLogger();
                var ln = 1;
                foreach (var record in records)
                {
                    l.Debug($"Line # {ln}, Record: {csv.Context.RawRecord}");
                    record.Line = ln;
                    record.Tag = TaggedLines.Contains(ln);
                    DataList.Add(record);

                    ln += 1;
                }
            }
        }
    }


    public class AmcacheParserDriverBinaryData : IFileSpecData
    {
        public string KeyName { get; set; }
        public DateTime KeyLastWriteTimestamp { get; set; }
        public DateTime DriverTimeStamp { get; set; }
        public DateTime DriverLastWriteTime { get; set; }
        public string DriverName { get; set; }
        public bool DriverInBox { get; set; }
        public bool DriverIsKernelMode { get; set; }
        public bool DriverSigned { get; set; }
        public string DriverCheckSum { get; set; }
        public string DriverCompany { get; set; }
        public string DriverId { get; set; }
        public string DriverPackageStrongName { get; set; }
        public string DriverType { get; set; }
        public string DriverVersion { get; set; }
        public string ImageSize { get; set; }
        public string Inf { get; set; }
        public string Product { get; set; }
        public string ProductVersion { get; set; }
        public string Service { get; set; }
        public string WdfVersion { get; set; }
        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return
                $"{KeyName} {KeyLastWriteTimestamp} {DriverTimeStamp} {DriverLastWriteTime} {DriverName} {DriverInBox} {DriverIsKernelMode} {DriverSigned} {DriverCheckSum} {DriverCompany} {DriverId} {DriverPackageStrongName} {DriverType} {DriverVersion} {ImageSize} {Inf} {Product} {ProductVersion} {Service} {WdfVersion}";
        }
    }


    public class AmcacheParserDriverBinary : IFileSpec
    {
        public AmcacheParserDriverBinary()
        {
            //Initialize collections here, one for TaggedLines TLE can add values to, and the collection that TLE will display
            TaggedLines = new List<int>();

            DataList = new BindingList<AmcacheParserDriverBinaryData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "KeyName,KeyLastWriteTimestamp,DriverTimeStamp,DriverLastWriteTime,DriverName,DriverInBox,DriverIsKernelMode,DriverSigned,DriverCheckSum,DriverCompany,DriverId,DriverPackageStrongName,DriverType,DriverVersion,ImageSize,Inf,Product,ProductVersion,Service,WdfVersion"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from AmcacheParser for new Driver Binary format";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "b50eaff5-063e-4864-b129-7d6100b2dd09";

        public void ProcessFile(string filename)
        {
            DataList.Clear();

            using (var fileReader = File.OpenText(filename))
            {
                var csv = new CsvReader(fileReader, CultureInfo.InvariantCulture);
                csv.Configuration.HasHeaderRecord = true;

                var foo = csv.Configuration.AutoMap<AmcacheParserDriverBinaryData>();

                var o = new TypeConverterOptions
                {
                    DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal
                };
                csv.Configuration.TypeConverterOptionsCache.AddOptions<AmcacheParserDriverBinaryData>(o);

                foo.Map(t => t.Line).Ignore();
                foo.Map(t => t.Tag).Ignore();

                csv.Configuration.RegisterClassMap(foo);


                var records = csv.GetRecords<AmcacheParserDriverBinaryData>();
                var l = LogManager.GetCurrentClassLogger();
                var ln = 1;
                foreach (var record in records)
                {
                    l.Debug($"Line # {ln}, Record: {csv.Context.RawRecord}");
                    record.Line = ln;
                    record.Tag = TaggedLines.Contains(ln);
                    DataList.Add(record);

                    ln += 1;
                }
            }
        }
    }


    public class AmcacheParserNewShortcutData : IFileSpecData
    {
        public string KeyName { get; set; }
        public string LnkName { get; set; }
        public DateTime KeyLastWriteTimestamp { get; set; }
        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{KeyName} {LnkName} {KeyLastWriteTimestamp}";
        }
    }


    public class AmcacheParserNewShortcut : IFileSpec
    {
        public AmcacheParserNewShortcut()
        {
            //Initialize collections here, one for TaggedLines TLE can add values to, and the collection that TLE will display
            TaggedLines = new List<int>();

            DataList = new BindingList<AmcacheParserNewShortcutData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "KeyName,LnkName,KeyLastWriteTimestamp"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from AmcacheParser for new Shortcuts format";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "4df80328-9ece-453b-939d-5aba89d3354d";

        public void ProcessFile(string filename)
        {
            DataList.Clear();

            using (var fileReader = File.OpenText(filename))
            {
                var csv = new CsvReader(fileReader, CultureInfo.InvariantCulture);
                csv.Configuration.HasHeaderRecord = true;

                var foo = csv.Configuration.AutoMap<AmcacheParserNewShortcutData>();

                var o = new TypeConverterOptions
                {
                    DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal
                };
                csv.Configuration.TypeConverterOptionsCache.AddOptions<AmcacheParserNewShortcutData>(o);

                foo.Map(t => t.Line).Ignore();
                foo.Map(t => t.Tag).Ignore();

                csv.Configuration.RegisterClassMap(foo);

                var l = LogManager.GetCurrentClassLogger();

                var records = csv.GetRecords<AmcacheParserNewShortcutData>();

                var ln = 1;
                foreach (var record in records)
                {
                    l.Debug($"Line # {ln}, Record: {csv.Context.RawRecord}");
                    record.Line = ln;
                    record.Tag = TaggedLines.Contains(ln);
                    DataList.Add(record);

                    ln += 1;
                }
            }
        }
    }

    public class AmcacheParserNewProgramData : IFileSpecData
    {
        public string ProgramId { get; set; }
        public DateTime KeyLastWriteTimestamp { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public string Publisher { get; set; }
        public DateTime? InstallDate { get; set; }
        public string OSVersionAtInstallTime { get; set; }
        public string BundleManifestPath { get; set; }
        public bool HiddenArp { get; set; }
        public bool InboxModernApp { get; set; }
        public int Language { get; set; }
        public string ManifestPath { get; set; }
        public string MsiPackageCode { get; set; }
        public string MsiProductCode { get; set; }
        public string PackageFullName { get; set; }
        public string ProgramInstanceId { get; set; }
        public string RegistryKeyPath { get; set; }
        public string RootDirPath { get; set; }
        public string Type { get; set; }
        public string Source { get; set; }
        public string StoreAppType { get; set; }
        public string UninstallString { get; set; }
        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return
                $"{ProgramId} {KeyLastWriteTimestamp} {Name} {Version} {Publisher} {InstallDate} {OSVersionAtInstallTime} {BundleManifestPath} {HiddenArp} {InboxModernApp} {Language} {ManifestPath} {MsiPackageCode} {MsiProductCode} {PackageFullName} {ProgramInstanceId} {RegistryKeyPath} {RootDirPath} {Type} {Source} {StoreAppType} {UninstallString}";
        }
    }


    public class AmcacheParserNewProgram : IFileSpec
    {
        public AmcacheParserNewProgram()
        {
            //Initialize collections here, one for TaggedLines TLE can add values to, and the collection that TLE will display
            TaggedLines = new List<int>();

            DataList = new BindingList<AmcacheParserNewProgramData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "ProgramId,KeyLastWriteTimestamp,Name,Version,Publisher,InstallDate,OSVersionAtInstallTime,BundleManifestPath,HiddenArp,InboxModernApp,Language,ManifestPath,MsiPackageCode,MsiProductCode,PackageFullName,ProgramInstanceId,RegistryKeyPath,RootDirPath,Type,Source,StoreAppType,UninstallString,InstallDateArpLastModified,InstallDateMsi,InstallDateFromLinkFile,Manufacturer"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from AmcacheParser for new Program format";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "86d6aee1-caf7-4e46-986f-86563f22c09d";

        public void ProcessFile(string filename)
        {
            DataList.Clear();

            using (var fileReader = File.OpenText(filename))
            {
                var csv = new CsvReader(fileReader, CultureInfo.InvariantCulture);
                csv.Configuration.HasHeaderRecord = true;

                var foo = csv.Configuration.AutoMap<AmcacheParserNewProgramData>();

                var o = new TypeConverterOptions
                {
                    DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal
                };
                csv.Configuration.TypeConverterOptionsCache.AddOptions<AmcacheParserNewProgramData>(o);

                foo.Map(t => t.Line).Ignore();
                foo.Map(t => t.Tag).Ignore();

                csv.Configuration.RegisterClassMap(foo);

                var l = LogManager.GetCurrentClassLogger();

                var records = csv.GetRecords<AmcacheParserNewProgramData>();

                var ln = 1;
                foreach (var record in records)
                {
                    l.Debug($"Line # {ln}, Record: {csv.Context.RawRecord}");

                    record.Line = ln;
                    record.Tag = TaggedLines.Contains(ln);
                    DataList.Add(record);

                    ln += 1;
                }
            }
        }
    }


    public class NetworkUsageData : IFileSpecData
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }

        public string ExeInfo { get; set; }
        public string ExeInfoDescription { get; set; }

        public DateTime? ExeTimestamp { get; set; }

        public string SidType { get; set; }
        public string Sid { get; set; }
        public string UserName { get; set; }
        public int UserId { get; set; }
        public int AppId { get; set; }
        public long BytesReceived { get; set; }
        public long BytesSent { get; set; }
        public long InterfaceLuid { get; set; }
        public string InterfaceType { get; set; }
        public int L2ProfileFlags { get; set; }
        public int L2ProfileId { get; set; }
        public string ProfileName { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return
                $"{Timestamp} {Id} {ExeInfo} {ExeInfoDescription} {ExeTimestamp} {SidType} {Sid} {UserName} {UserId} {AppId} {BytesReceived} {BytesSent} {InterfaceLuid} {InterfaceType} {L2ProfileFlags} {L2ProfileId} {ProfileName}";
        }
    }

    public class NetworkUsage : IFileSpec
    {
        public NetworkUsage()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<NetworkUsageData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Id,Timestamp,ExeInfo,ExeInfoDescription,ExeTimestamp,SidType,Sid,UserName,UserId,AppId,BytesReceived,BytesSent,InterfaceLuid,InterfaceType,L2ProfileFlags,L2ProfileId,ProfileName"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from SrumECmd for NetworkUsage";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "41ee6405-22cf-4612-a480-9a050d0a9952";

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
                csv.Configuration.TypeConverterOptionsCache.AddOptions<NetworkUsageData>(o);

                var foo = csv.Configuration.AutoMap<NetworkUsageData>();

                foo.Map(t => t.Line).Ignore();
                foo.Map(t => t.Tag).Ignore();

                csv.Configuration.RegisterClassMap(foo);

                var records = csv.GetRecords<NetworkUsageData>();

                var l = LogManager.GetCurrentClassLogger();


                var ln = 1;
                foreach (var record in records)
                {
                    l.Debug($"Line # {ln}, Record: {csv.Context.RawRecord}");
                    record.Line = ln;
                    record.Tag = TaggedLines.Contains(ln);
                    DataList.Add(record);

                    ln += 1;
                }
            }
        }
    }


    public class PushNotificationData : IFileSpecData
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }

        public string ExeInfo { get; set; }
        public string ExeInfoDescription { get; set; }
        public DateTime? ExeTimestamp { get; set; }
        public string SidType { get; set; }
        public string Sid { get; set; }
        public string UserName { get; set; }
        public int UserId { get; set; }
        public int AppId { get; set; }

        public int NetworkType { get; set; }
        public int NotificationType { get; set; }
        public int PayloadSize { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return
                $"{Timestamp} {Id} {ExeInfo} {ExeInfoDescription} {ExeTimestamp} {SidType} {Sid} {UserName} {UserId} {AppId} {NetworkType} {NotificationType} {PayloadSize}";
        }
    }

    public class PushNotification : IFileSpec
    {
        public PushNotification()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<PushNotificationData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Id,Timestamp,ExeInfo,ExeInfoDescription,ExeTimestamp,SidType,Sid,UserName,UserId,AppId,NetworkType,NotificationType,PayloadSize"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from SrumECmd for PushNotification";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "41ee6405-33ef-4612-a480-9a050d0a9952";

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
                csv.Configuration.TypeConverterOptionsCache.AddOptions<PushNotificationData>(o);

                var foo = csv.Configuration.AutoMap<PushNotificationData>();

                foo.Map(t => t.Line).Ignore();
                foo.Map(t => t.Tag).Ignore();

                csv.Configuration.RegisterClassMap(foo);

                var records = csv.GetRecords<PushNotificationData>();

                var l = LogManager.GetCurrentClassLogger();


                var ln = 1;
                foreach (var record in records)
                {
                    l.Debug($"Line # {ln}, Record: {csv.Context.RawRecord}");
                    record.Line = ln;
                    record.Tag = TaggedLines.Contains(ln);
                    DataList.Add(record);

                    ln += 1;
                }
            }
        }
    }

    public class NetworkConnectionData : IFileSpecData
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }

        public string ExeInfo { get; set; }
        public string ExeInfoDescription { get; set; }

        public DateTime? ExeTimestamp { get; set; }
        public string SidType { get; set; }
        public string Sid { get; set; }
        public string UserName { get; set; }

        public int UserId { get; set; }
        public int AppId { get; set; }

        public int ConnectedTime { get; set; }
        public DateTime ConnectStartTime { get; set; }
        public long InterfaceLuid { get; set; }
        public string InterfaceType { get; set; }
        public int L2ProfileFlags { get; set; }
        public int L2ProfileId { get; set; }
        public string ProfileName { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return
                $"{Timestamp} {Id} {ExeInfo} {ExeInfoDescription} {ExeTimestamp} {SidType} {Sid} {UserName} {UserId} {AppId} {ConnectedTime} {ConnectStartTime} {InterfaceLuid} {InterfaceType} {L2ProfileFlags} {L2ProfileId} {ProfileName}";
        }
    }

    public class NetworkConnection : IFileSpec
    {
        public NetworkConnection()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<NetworkConnectionData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Id,Timestamp,ExeInfo,ExeInfoDescription,ExeTimestamp,SidType,Sid,UserName,UserId,AppId,ConnectedTime,ConnectStartTime,InterfaceLuid,InterfaceType,L2ProfileFlags,L2ProfileId,ProfileName"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from SrumECmd for NetworkConnection";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "41ab4565-22cf-4612-a480-9a050d0a9952";

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
                csv.Configuration.TypeConverterOptionsCache.AddOptions<NetworkConnectionData>(o);

                var foo = csv.Configuration.AutoMap<NetworkConnectionData>();

                foo.Map(t => t.Line).Ignore();
                foo.Map(t => t.Tag).Ignore();

                csv.Configuration.RegisterClassMap(foo);

                var records = csv.GetRecords<NetworkConnectionData>();

                var l = LogManager.GetCurrentClassLogger();


                var ln = 1;
                foreach (var record in records)
                {
                    l.Debug($"Line # {ln}, Record: {csv.Context.RawRecord}");
                    record.Line = ln;
                    record.Tag = TaggedLines.Contains(ln);
                    DataList.Add(record);

                    ln += 1;
                }
            }
        }
    }

    public class AppResourceUseInfoData : IFileSpecData
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }

        public string ExeInfo { get; set; }
        public string ExeInfoDescription { get; set; }
        public DateTime? ExeTimestamp { get; set; }
        public string SidType { get; set; }
        public string Sid { get; set; }
        public string UserName { get; set; }

        public int UserId { get; set; }
        public int AppId { get; set; }

        public long BackgroundBytesRead { get; set; }
        public long BackgroundBytesWritten { get; set; }
        public int BackgroundContextSwitches { get; set; }
        public long BackgroundCycleTime { get; set; }
        public int BackgroundNumberOfFlushes { get; set; }
        public int BackgroundNumReadOperations { get; set; }
        public int BackgroundNumWriteOperations { get; set; }
        public long FaceTime { get; set; }
        public long ForegroundBytesRead { get; set; }
        public long ForegroundBytesWritten { get; set; }
        public int ForegroundContextSwitches { get; set; }
        public long ForegroundCycleTime { get; set; }
        public int ForegroundNumberOfFlushes { get; set; }
        public int ForegroundNumReadOperations { get; set; }

        public int ForegroundNumWriteOperations { get; set; }


        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return
                $"{Timestamp} {Id} {ExeInfo} {ExeInfoDescription} {ExeTimestamp} {SidType} {Sid} {UserName} {UserId} {AppId} {BackgroundBytesRead} {BackgroundBytesWritten} {BackgroundContextSwitches} {BackgroundCycleTime} {BackgroundNumReadOperations} {BackgroundNumWriteOperations} {BackgroundNumberOfFlushes} {FaceTime} {ForegroundBytesRead} {ForegroundBytesWritten} {ForegroundContextSwitches} {ForegroundCycleTime} {ForegroundNumReadOperations} {ForegroundNumWriteOperations} {ForegroundNumberOfFlushes}";
        }
    }

    public class AppResourceUseInfo : IFileSpec
    {
        public AppResourceUseInfo()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<AppResourceUseInfoData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Id,Timestamp,ExeInfo,ExeInfoDescription,ExeTimestamp,SidType,Sid,UserName,UserId,AppId,BackgroundBytesRead,BackgroundBytesWritten,BackgroundContextSwitches,BackgroundCycleTime,BackgroundNumberOfFlushes,BackgroundNumReadOperations,BackgroundNumWriteOperations,FaceTime,ForegroundBytesRead,ForegroundBytesWritten,ForegroundContextSwitches,ForegroundCycleTime,ForegroundNumberOfFlushes,ForegroundNumReadOperations,ForegroundNumWriteOperations"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from SrumECmd for AppResourceUseInfo";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "41ee6405-22cf-5678-b580-9a050d0a9952";

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
                csv.Configuration.TypeConverterOptionsCache.AddOptions<AppResourceUseInfoData>(o);

                var foo = csv.Configuration.AutoMap<AppResourceUseInfoData>();

                foo.Map(t => t.Line).Ignore();
                foo.Map(t => t.Tag).Ignore();


                csv.Configuration.RegisterClassMap(foo);

                var records = csv.GetRecords<AppResourceUseInfoData>();

                var l = LogManager.GetCurrentClassLogger();


                var ln = 1;
                foreach (var record in records)
                {
                    l.Debug($"Line # {ln}, Record: {csv.Context.RawRecord}");
                    record.Line = ln;
                    record.Tag = TaggedLines.Contains(ln);
                    DataList.Add(record);

                    ln += 1;
                }
            }
        }
    }

    public class UnknownD8FData : IFileSpecData
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public int UserId { get; set; }
        public int AppId { get; set; }

        public string ExeInfo { get; set; }
        public string ExeInfoDescription { get; set; }

        public DateTime? ExeTimestamp { get; set; }

        public string SidType { get; set; }
        public string Sid { get; set; }
        public string UserName { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public int Flags { get; set; }

        public TimeSpan Duration { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return
                $"{Timestamp} {Id} {ExeInfo} {ExeInfoDescription} {ExeTimestamp} {SidType} {Sid} {UserName} {UserId} {AppId} {StartTime} {EndTime} {Flags} {Duration}";
        }
    }

    public class UnknownD8F : IFileSpec
    {
        public UnknownD8F()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<UnknownD8FData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Id,Timestamp,UserId,AppId,ExeInfo,ExeInfoDescription,ExeTimestamp,SidType,Sid,UserName,StartTime,EndTime,Flags,Duration"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from SrumECmd for UnknownD8F";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "41ee6405-35ea-4612-b480-9a050d1a9952";

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
                csv.Configuration.TypeConverterOptionsCache.AddOptions<UnknownD8FData>(o);

                var foo = csv.Configuration.AutoMap<UnknownD8FData>();

                foo.Map(t => t.Line).Ignore();
                foo.Map(t => t.Tag).Ignore();

                csv.Configuration.RegisterClassMap(foo);

                var records = csv.GetRecords<UnknownD8FData>();

                var l = LogManager.GetCurrentClassLogger();


                var ln = 1;
                foreach (var record in records)
                {
                    l.Debug($"Line # {ln}, Record: {csv.Context.RawRecord}");
                    record.Line = ln;
                    record.Tag = TaggedLines.Contains(ln);
                    DataList.Add(record);

                    ln += 1;
                }
            }
        }
    }

    public class Unknown312Data : IFileSpecData
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }

        public string ExeInfo { get; set; }
        public string ExeInfoDescription { get; set; }

        public DateTime? ExeTimestamp { get; set; }

        public string SidType { get; set; }
        public string Sid { get; set; }
        public string UserName { get; set; }

        public int UserId { get; set; }
        public int AppId { get; set; }

        public DateTime EndTime { get; set; }

        public int DurationMs { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return
                $"{Timestamp} {Id} {ExeInfo} {ExeInfoDescription} {ExeTimestamp} {SidType} {Sid} {UserName} {UserId} {AppId} {EndTime} {DurationMs}";
        }
    }

    public class Unknown312 : IFileSpec
    {
        public Unknown312()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<Unknown312Data>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Id,Timestamp,ExeInfo,ExeInfoDescription,ExeTimestamp,SidType,Sid,UserName,UserId,AppId,EndTime,DurationMs"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from SrumECmd for Unknown312";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "41ee6405-22cf-4612-a480-9a050d1b5631";

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
                csv.Configuration.TypeConverterOptionsCache.AddOptions<Unknown312Data>(o);

                var foo = csv.Configuration.AutoMap<Unknown312Data>();

                foo.Map(t => t.Line).Ignore();
                foo.Map(t => t.Tag).Ignore();

                csv.Configuration.RegisterClassMap(foo);

                var records = csv.GetRecords<Unknown312Data>();

                var l = LogManager.GetCurrentClassLogger();

                var ln = 1;
                foreach (var record in records)
                {
                    l.Debug($"Line # {ln}, Record: {csv.Context.RawRecord}");
                    record.Line = ln;
                    record.Tag = TaggedLines.Contains(ln);
                    DataList.Add(record);

                    ln += 1;
                }
            }
        }
    }

    public class EnergyUsageData : IFileSpecData
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string ExeInfo { get; set; }
        public string ExeInfoDescription { get; set; }

        public DateTime? ExeTimestamp { get; set; }

        public string SidType { get; set; }
        public string Sid { get; set; }
        public string UserName { get; set; }

        public int UserId { get; set; }
        public int AppId { get; set; }
        public bool IsLt { get; set; }

        public long ConfigurationHash { get; set; }
        public DateTime? EventTimestamp { get; set; }
        public long StateTransition { get; set; }

        public int ChargeLevel { get; set; }
        public int CycleCount { get; set; }
        public int DesignedCapacity { get; set; }
        public int FullChargedCapacity { get; set; }
        public int ActiveAcTime { get; set; }
        public int ActiveDcTime { get; set; }
        public int ActiveDischargeTime { get; set; }
        public int ActiveEnergy { get; set; }
        public int CsAcTime { get; set; }
        public int CsDcTime { get; set; }
        public int CsDischargeTime { get; set; }
        public int CsEnergy { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return
                $"{Timestamp} {Id} {ExeInfo} {ExeInfoDescription} {ExeTimestamp} {SidType} {Sid} {UserName} {UserId} {AppId} {IsLt} {ConfigurationHash} {EventTimestamp} {StateTransition} {ChargeLevel} {CycleCount} {DesignedCapacity} {FullChargedCapacity} {ActiveAcTime} {ActiveDcTime} {ActiveDischargeTime} {ActiveEnergy} {CsAcTime} {CsDcTime} {CsDischargeTime} {CsEnergy}";
        }
    }

    public class EnergyUsage : IFileSpec
    {
        public EnergyUsage()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<EnergyUsageData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Id,Timestamp,ExeInfo,ExeInfoDescription,ExeTimestamp,SidType,Sid,UserName,UserId,AppId,IsLt,ConfigurationHash,EventTimestamp,StateTransition,ChargeLevel,CycleCount,DesignedCapacity,FullChargedCapacity,ActiveAcTime,ActiveDcTime,ActiveDischargeTime,ActiveEnergy,CsAcTime,CsDcTime,CsDischargeTime,CsEnergy"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from SrumECmd for EnergyUsage";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "41ee6405-22cf-4612-a480-8b120d1a9952";

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
                csv.Configuration.TypeConverterOptionsCache.AddOptions<EnergyUsageData>(o);

                var foo = csv.Configuration.AutoMap<EnergyUsageData>();

                foo.Map(t => t.Line).Ignore();
                foo.Map(t => t.Tag).Ignore();

                csv.Configuration.RegisterClassMap(foo);

                var records = csv.GetRecords<EnergyUsageData>();

                var l = LogManager.GetCurrentClassLogger();


                var ln = 1;
                foreach (var record in records)
                {
                    l.Debug($"Line # {ln}, Record: {csv.Context.RawRecord}");
                    record.Line = ln;
                    record.Tag = TaggedLines.Contains(ln);
                    DataList.Add(record);

                    ln += 1;
                }
            }
        }
    }
}