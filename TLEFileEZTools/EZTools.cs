using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.TypeConversion;
using ITLEFileSpec;

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
                var csv = new CsvReader(fileReader,CultureInfo.InvariantCulture);
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
            return $"{Note} {SourceFilename} {SourceCreated} {SourceModified} {SourceAccessed} {ExecutableName} {Hash} {Size} {Version} {RunCount} {LastRun} {PreviousRun0} {PreviousRun1} {PreviousRun2} {PreviousRun2} {PreviousRun3} {PreviousRun4} {PreviousRun5} {PreviousRun6} {Volume0Name} {Volume0Serial} {Volume0Created} {Volume1Name} {Volume1Serial} {Volume1Created} {Directories} {FilesLoaded} {ParsingError}";
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
                var csv = new CsvReader(fileReader,CultureInfo.InvariantCulture);
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
                var csv = new CsvReader(fileReader,CultureInfo.InvariantCulture);
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
            return $"{SourceFile} {SourceCreated} {SourceModified} {SourceAccessed} {TargetCreated} {TargetModified} {TargetAccessed} {FileSize} {RelativePath} {WorkingDirectory} {FileAttributes} {HeaderFlags} {DriveType} {VolumeSerialNumber} {VolumeLabel} {LocalPath} {NetworkPath} {CommonPath} {Arguments} {TargetIDAbsolutePath} {TargetMFTEntryNumber} {TargetMFTSequenceNumber} {MachineID} {MachineMACAddress} {MACVendor} {TrackerCreatedOn} {ExtraBlocksPresent}";
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
                var csv = new CsvReader(fileReader,CultureInfo.InvariantCulture);
                csv.Configuration.HasHeaderRecord = true;
                        

                var o = new TypeConverterOptions
                {
                    DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal
                };
                csv.Configuration.TypeConverterOptionsCache.AddOptions<LECmdData>(o);

                csv.Configuration.PrepareHeaderForMatch =  (header, index) => header.Replace(" ", "");

                var foo = csv.Configuration.AutoMap<LECmdData>();

                foo.Map(t => t.Line).Ignore();
                foo.Map(t => t.Tag).Ignore();

                csv.Configuration.RegisterClassMap(foo);

                var records = csv.GetRecords<LECmdData>();

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
            return $"{SourceFile} {SourceCreated} {SourceModified} {SourceAccessed} {AppId} {AppIdDescription} {DestListVersion} {LastUsedEntryNumber} {MRU} {CreationTime} {LastModified} {Hostname} {MacAddress} {Path} {InteractionCount} {PinStatus} {FileBirthDroid} {FileDroid} {VolumeBirthDroid} {VolumeDroid} {TargetCreated} {TargetModified} {TargetAccessed} {FileSize} {RelativePath} {WorkingDirectory} {FileAttributes} {HeaderFlags} {DriveType} {VolumeSerialNumber}" +
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
                var csv = new CsvReader(fileReader,CultureInfo.InvariantCulture);
                csv.Configuration.HasHeaderRecord = true;
                        

                csv.Configuration.PrepareHeaderForMatch =  (header, index) => header.Replace(" ", "");

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

                var ln = 1;
                foreach (var record in records)
                {
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
            return $"{SourceFile} {SourceCreated} {SourceModified} {SourceAccessed} {AppId} {AppIdDescription} {EntryName} {TargetCreated} {TargetModified} {TargetAccessed} {FileSize} {RelativePath} {WorkingDirectory} {FileAttributes} {HeaderFlags} {DriveType} {VolumeSerialNumber} {VolumeLabel} {LocalPath} {CommonPath} {Arguments} {TargetIDAbsolutePath} {TargetMFTEntryNumber} {TargetMFTSequenceNumber} {MachineID} {MachineMACAddress} {TrackerCreatedOn} {ExtraBlocksPresent}";
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
                var csv = new CsvReader(fileReader,CultureInfo.InvariantCulture);
                csv.Configuration.HasHeaderRecord = true;
                        

                var o = new TypeConverterOptions
                {
                    DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal
                };
                csv.Configuration.TypeConverterOptionsCache.AddOptions<JleCmdCustomData>(o);

                csv.Configuration.PrepareHeaderForMatch =  (header, index) => header.Replace(" ", "");

                var foo = csv.Configuration.AutoMap<JleCmdCustomData>();

                foo.Map(t => t.Line).Ignore();
                foo.Map(t => t.Tag).Ignore();

                csv.Configuration.RegisterClassMap(foo);

                var records = csv.GetRecords<JleCmdCustomData>();

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

    public class EvtECmdData : IFileSpecData
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
            return $"{RecordNumber} {EventRecordId} {TimeCreated} {EventId} {Level} {Provider} {Channel} {ProcessId} {ThreadId} {Computer} {UserId} {MapDescription} {UserName} {RemoteHost} {PayloadData1} {PayloadData2} {PayloadData3} {PayloadData4} {PayloadData5} {PayloadData6} {ExecutableInfo} {SourceFile} {Keywords} {HiddenRecord}";
        }
    }

   
    public class EvtECmd : IFileSpec
    {
        public EvtECmd()
        {
            //Initialize collections here, one for TaggedLines TLE can add values to, and the collection that TLE will display
            TaggedLines = new List<int>();

            DataList = new BindingList<EvtECmdData>();

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
                var csv = new CsvReader(fileReader,CultureInfo.InvariantCulture);
                csv.Configuration.HasHeaderRecord = true;
                        
                var o = new TypeConverterOptions
                {
                    DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal
                };
                csv.Configuration.TypeConverterOptionsCache.AddOptions<EvtECmdData>(o);

                var foo = csv.Configuration.AutoMap<EvtECmdData>();

                foo.Map(t => t.Line).Ignore();
                foo.Map(t => t.Tag).Ignore();
                foo.Map(t => t.Payload).Optional();
                foo.Map(t => t.HiddenRecord).Optional();
                foo.Map(t => t.Keywords).Optional();

                csv.Configuration.RegisterClassMap(foo);

                var records = csv.GetRecords<EvtECmdData>();

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
            return $"{HivePath} {HiveType} {Description} {Category} {KeyPath} {ValueName} {ValueType} {ValueData} {ValueData2} {ValueData3} {Comment} {Recursive} {Deleted} {LastWriteTimestamp} {PluginDetailFile}";
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
                var csv = new CsvReader(fileReader,CultureInfo.InvariantCulture);
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
            return $"{BagPath} {Slot} {NodeSlot} {MRUPosition} {AbsolutePath} {ShellType} {Value} {ChildBags} {CreatedOn} {ModifiedOn} {AccessedOn} {MFTEntry} {MFTSequenceNumber} {ExtensionBlockCount} {FirstInteracted} {LastInteracted} {HasExplored} {Miscellaneous} {LastWriteTime}";
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
                var csv = new CsvReader(fileReader,CultureInfo.InvariantCulture);
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
            return $"{ControlSet} {CacheEntryPosition} {Path} {LastModifiedTimeUTC} {Executed} {Duplicate} {SourceFile}";
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
                var csv = new CsvReader(fileReader,CultureInfo.InvariantCulture);
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
                var csv = new CsvReader(fileReader,CultureInfo.InvariantCulture);
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

    public class ActivityData : IFileSpecData
    {
        public override string ToString()
        {
            return $"{Id} {Executable} {DisplayText} {Payload} {ClipboardPayload} {ContentInfo} {LastModifiedTime} {ExpirationTime} {CreatedInCloud} {StartTime} {EndTime} {Duration} {LastModifiedOnClient} {OriginalLastModifiedOnClient} {ActivityType} {ActivityTypeOrg} {IsLocalOnly} {ETag} {PackageIdHash} {PlatformDeviceId} {DevicePlatform} {TimeZone}";
        }

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
                        var csv = new CsvReader(fileReader,CultureInfo.InvariantCulture);
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

    public class ActivityOperationData : IFileSpecData
    {
        public override string ToString()
        {
            return $"{OperationOrder} {OperationType} {AppId} {Executable} {Description} {StartTime} {DisplayText} {ClipboardPayload} {ContentInfo} {DevicePlatform} {TimeZone} {ActivityTypeOrg} {ActivityType} {Duration} {LastModifiedTime} {ExpirationTime} {Payload} {CreatedTime} {EndTime} {LastModifiedTimeOnClient} {OperationExpirationTime} {PlatformDeviceId}";
        }

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
                        var csv = new CsvReader(fileReader,CultureInfo.InvariantCulture);
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

 
    /*
                case ContentType.AmcacheParserFiles:

                    DataList = new BindingList<AmcacheParserFiles>();
                    using (var fileReader = File.OpenText(FileName))
                    {
                        var csv = new CsvReader(fileReader,CultureInfo.InvariantCulture);
                        csv.Configuration.HasHeaderRecord = true;
                        
                        var foo = csv.Configuration.AutoMap<AmcacheParserFiles>();

                        var o = new TypeConverterOptions
                        {
                            DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal
                        };
                        csv.Configuration.TypeConverterOptionsCache.AddOptions<AmcacheParserFiles>(o);

                        foo.Map(t => t.Line).Ignore();
                        foo.Map(t => t.Tag).Ignore();

                        csv.Configuration.RegisterClassMap(foo);

                        var records = csv.GetRecords<AmcacheParserFiles>();

                        var ln = 1;
                        foreach (var record in records)
                        {
                            record.Line = ln;
                            record.Tag = _taggedLines.Contains(ln);
                            DataList.Add(record);

                            ln += 1;
                        }
                    }

                    return true;

                case ContentType.AmcacheParserNewFile:

                    DataList = new BindingList<AmcacheParserNewFiles>();
                    using (var fileReader = File.OpenText(FileName))
                    {
                        var csv = new CsvReader(fileReader,CultureInfo.InvariantCulture);
                        csv.Configuration.HasHeaderRecord = true;
                        

                        var o = new TypeConverterOptions
                        {
                            DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal
                        };
                        csv.Configuration.TypeConverterOptionsCache.AddOptions<AmcacheParserNewFiles>(o);

                        var foo = csv.Configuration.AutoMap<AmcacheParserNewFiles>();

                        foo.Map(t => t.Line).Ignore();
                        foo.Map(t => t.Tag).Ignore();

                        csv.Configuration.RegisterClassMap(foo);

                        var records = csv.GetRecords<AmcacheParserNewFiles>();

                        var ln = 1;
                        foreach (var record in records)
                        {
                            record.Line = ln;
                            record.Tag = _taggedLines.Contains(ln);
                            DataList.Add(record);

                            ln += 1;
                        }
                    }

                    return true;

                case ContentType.AmcacheParserPrograms:

                    DataList = new BindingList<AmcacheParserPrograms>();
                    using (var fileReader = File.OpenText(FileName))
                    {
                        var csv = new CsvReader(fileReader,CultureInfo.InvariantCulture);
                        csv.Configuration.HasHeaderRecord = true;
                        
                        var o = new TypeConverterOptions
                        {
                            DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal
                        };
                        csv.Configuration.TypeConverterOptionsCache.AddOptions<AmcacheParserPrograms>(o);

                        var foo = csv.Configuration.AutoMap<AmcacheParserPrograms>();

                        foo.Map(t => t.Line).Ignore();
                        foo.Map(t => t.Tag).Ignore();

                        csv.Configuration.RegisterClassMap(foo);

                        var records = csv.GetRecords<AmcacheParserPrograms>();

                        var ln = 1;
                        foreach (var record in records)
                        {
                            record.Line = ln;
                            record.Tag = _taggedLines.Contains(ln);
                            DataList.Add(record);

                            ln += 1;
                        }
                    }

                    return true;



                case ContentType.AmcacheParserNewDevicePnP:
                    DataList = new BindingList<AmcacheParserNewDevicePnP>();
                    using (var fileReader = File.OpenText(FileName))
                    {
                        var csv = new CsvReader(fileReader,CultureInfo.InvariantCulture);
                        csv.Configuration.HasHeaderRecord = true;
                        
                        var foo = csv.Configuration.AutoMap<AmcacheParserNewDevicePnP>();

                        var o = new TypeConverterOptions
                        {
                            DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal
                        };
                        csv.Configuration.TypeConverterOptionsCache.AddOptions<AmcacheParserNewDevicePnP>(o);

                        foo.Map(t => t.Line).Ignore();
                        foo.Map(t => t.Tag).Ignore();

                        csv.Configuration.RegisterClassMap(foo);

                        var records = csv.GetRecords<AmcacheParserNewDevicePnP>();

                        var ln = 1;
                        foreach (var record in records)
                        {
                            record.Line = ln;
                            record.Tag = _taggedLines.Contains(ln);
                            DataList.Add(record);

                            ln += 1;
                        }
                    }

                    return true;
                case ContentType.AmcacheParserNewDeviceContainer:
                    DataList = new BindingList<AmcacheParserDeviceContainer>();
                    using (var fileReader = File.OpenText(FileName))
                    {
                        var csv = new CsvReader(fileReader,CultureInfo.InvariantCulture);
                        csv.Configuration.HasHeaderRecord = true;
                        
                        var foo = csv.Configuration.AutoMap<AmcacheParserDeviceContainer>();

                        var o = new TypeConverterOptions
                        {
                            DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal
                        };
                        csv.Configuration.TypeConverterOptionsCache.AddOptions<AmcacheParserDeviceContainer>(o);

                        foo.Map(t => t.Line).Ignore();
                        foo.Map(t => t.Tag).Ignore();

                        csv.Configuration.RegisterClassMap(foo);

                        var records = csv.GetRecords<AmcacheParserDeviceContainer>();

                        var ln = 1;
                        foreach (var record in records)
                        {
                            record.Line = ln;
                            record.Tag = _taggedLines.Contains(ln);
                            DataList.Add(record);

                            ln += 1;
                        }
                    }

                    return true;
                case ContentType.AmcacheParserNewDriverPackage:
                    DataList = new BindingList<AmcacheParserDriverPackage>();
                    using (var fileReader = File.OpenText(FileName))
                    {
                        var csv = new CsvReader(fileReader,CultureInfo.InvariantCulture);
                        csv.Configuration.HasHeaderRecord = true;
                        
                        var foo = csv.Configuration.AutoMap<AmcacheParserDriverPackage>();

                        var o = new TypeConverterOptions
                        {
                            DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal
                        };
                        csv.Configuration.TypeConverterOptionsCache.AddOptions<AmcacheParserDriverPackage>(o);

                        foo.Map(t => t.Line).Ignore();
                        foo.Map(t => t.Tag).Ignore();

                        csv.Configuration.RegisterClassMap(foo);

                        var records = csv.GetRecords<AmcacheParserDriverPackage>();

                        var ln = 1;
                        foreach (var record in records)
                        {
                            record.Line = ln;
                            record.Tag = _taggedLines.Contains(ln);
                            DataList.Add(record);

                            ln += 1;
                        }
                    }

                    return true;
                case ContentType.AmcacheParserNewDriveBinary:
                    DataList = new BindingList<AmcacheParserDriverBinary>();
                    using (var fileReader = File.OpenText(FileName))
                    {
                        var csv = new CsvReader(fileReader,CultureInfo.InvariantCulture);
                        csv.Configuration.HasHeaderRecord = true;
                        
                        var foo = csv.Configuration.AutoMap<AmcacheParserDriverBinary>();

                        var o = new TypeConverterOptions
                        {
                            DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal
                        };
                        csv.Configuration.TypeConverterOptionsCache.AddOptions<AmcacheParserDriverBinary>(o);

                        foo.Map(t => t.Line).Ignore();
                        foo.Map(t => t.Tag).Ignore();

                        csv.Configuration.RegisterClassMap(foo);

                        

                        var records = csv.GetRecords<AmcacheParserDriverBinary>();

                        var ln = 1;
                        foreach (var record in records)
                        {
                            record.Line = ln;
                            record.Tag = _taggedLines.Contains(ln);
                            DataList.Add(record);

                            ln += 1;
                        }
                    }

                    return true;
                case ContentType.AmcacheParserShortcuts:
                    DataList = new BindingList<AmcacheParserNewShortcut>();
                    using (var fileReader = File.OpenText(FileName))
                    {
                        var csv = new CsvReader(fileReader,CultureInfo.InvariantCulture);
                        csv.Configuration.HasHeaderRecord = true;
                        
                        var foo = csv.Configuration.AutoMap<AmcacheParserNewShortcut>();

                        var o = new TypeConverterOptions
                        {
                            DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal
                        };
                        csv.Configuration.TypeConverterOptionsCache.AddOptions<AmcacheParserNewShortcut>(o);

                        foo.Map(t => t.Line).Ignore();
                        foo.Map(t => t.Tag).Ignore();

                        csv.Configuration.RegisterClassMap(foo);

                        var records = csv.GetRecords<AmcacheParserNewShortcut>();

                        var ln = 1;
                        foreach (var record in records)
                        {
                            record.Line = ln;
                            record.Tag = _taggedLines.Contains(ln);
                            DataList.Add(record);

                            ln += 1;
                        }
                    }

                    return true;
                case ContentType.AmcacheParserNewPrograms:
                    DataList = new BindingList<AmcacheParserNewProgram>();
                    using (var fileReader = File.OpenText(FileName))
                    {
                        var csv = new CsvReader(fileReader,CultureInfo.InvariantCulture);
                        csv.Configuration.HasHeaderRecord = true;
                        
                        var foo = csv.Configuration.AutoMap<AmcacheParserNewProgram>();

                        var o = new TypeConverterOptions
                        {
                            DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal
                        };
                        csv.Configuration.TypeConverterOptionsCache.AddOptions<AmcacheParserNewProgram>(o);

                        foo.Map(t => t.Line).Ignore();
                        foo.Map(t => t.Tag).Ignore();

                        csv.Configuration.RegisterClassMap(foo);

                        var records = csv.GetRecords<AmcacheParserNewProgram>();

                        var ln = 1;
                        foreach (var record in records)
                        {
                            record.Line = ln;
                            record.Tag = _taggedLines.Contains(ln);
                            DataList.Add(record);

                            ln += 1;
                        }
                    }

                    return true;*/
}