using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using ITLEFileSpec;
using TLEFilePlugins.Shared;
using Serilog;

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
        public string SourceFile { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        /// <summary>
        ///     When searching in TLE, this is used to look for the search term across all the properties in the class
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return
                $"{EntryPoint} {Signature} {BytesPerSector} {SectorsPerCluster} {ClusterSize} {ReservedSectors} {TotalSectors} {MftClusterBlockNumber} {MftMirrClusterBlockNumber} {MftEntrySize} {IndexEntrySize} {VolumeSerialNumberRaw} {VolumeSerialNumber} {VolumeSerialNumber32} {VolumeSerialNumber32Reverse} {SectorSignature} {SourceFile}";
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
            CsvLoader.Load<BootData>(filename, DataList, TaggedLines);
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
        public string SourceFile { get; set; }

        //added in the 2026.10 versions of the tool
        public bool? FromMirror { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return
                $"{Hash} {Id} {Offset} {OwnerSid} {GroupSid} {Control} {SaclAceCount} {UniqueSaclAceTypes} {DaclAceCount} {UniqueDaclAceTypes} {FileOffset} {SourceFile} {FromMirror}";
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
                "Hash,Id,Offset,OwnerSid,GroupSid,Control,SaclAceCount,UniqueSaclAceTypes,DaclAceCount,UniqueDaclAceTypes,FileOffset,SourceFile",
                "Hash,Id,Offset,OwnerSid,GroupSid,Control,SaclAceCount,UniqueSaclAceTypes,DaclAceCount,UniqueDaclAceTypes,FileOffset,SourceFile,FromMirror"
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
            CsvLoader.Load<SdsData>(filename, DataList, TaggedLines);
        }
    }

    public class JData : IFileSpecData
    {
        public string Name { get; set; }
        public string Extension => $"{Path.GetExtension(Name)}{string.Empty}";
        public ulong EntryNumber { get; set; }
        public int SequenceNumber { get; set; }
        public ulong ParentEntryNumber { get; set; }
        public uint ParentSequenceNumber { get; set; }
        
        public string ParentPath { get; set; }
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
                $"{Name} {Extension} {EntryNumber} {SequenceNumber} {ParentEntryNumber} {ParentSequenceNumber} {ParentPath} {UpdateSequenceNumber} {UpdateTimestamp} {UpdateReasons} {FileAttributes} {OffsetToData} {SourceFile}";
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
                "Name,Extension,EntryNumber,SequenceNumber,ParentEntryNumber,ParentSequenceNumber,ParentPath,UpdateSequenceNumber,UpdateTimestamp,UpdateReasons,FileAttributes,OffsetToData,SourceFile"
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
            CsvLoader.Load<JData>(filename, DataList, TaggedLines, map => map.Map(m => m.UpdateTimestamp).TypeConverterOption.DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal));
        }
    }

    public class MFTSourceFileResidentData : IFileSpecData
    {
        public uint EntryNumber { get; set; }
        public ushort SequenceNumber { get; set; }
        public uint ParentEntryNumber { get; set; }
        public ushort? ParentSequenceNumber { get; set; }

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
        public string SourceFile { get; set; }
        public string ResidentDataBase64 { get; set; }
        public string ResidentDataHex { get; set; }
        public string ResidentDataASCII { get; set; }

        //added in the 2026.10 versions of the tool
        public string ParseErrors { get; set; }

        public int Line { get; set; }

        public bool Tag { get; set; }

        public override string ToString()
        {
            return
                $"{EntryNumber} {SequenceNumber} {ParentEntryNumber} {ParentSequenceNumber} {InUse} {ParentPath} {FileName} {Extension} {IsDirectory} {HasAds} {IsAds} {FileSize} {Created0x10} {Created0x30} {LastModified0x10} {LastModified0x30} {LastRecordChange0x10} {LastRecordChange0x30} {LastAccess0x10} {LastAccess0x30} {UpdateSequenceNumber} {LogfileSequenceNumber} {SecurityId} {ZoneIdContents} {SiFlags} {ObjectIdFileDroid} {ReparseTarget}" +
                $" {ReferenceCount} {NameType} {LoggedUtilStream} {Timestomped} {uSecZeros} {Copied} {SourceFile} {ResidentDataBase64} {ResidentDataHex} {ResidentDataASCII} {ParseErrors}";
        }
    }

    public class MFTSourceFileResident : IFileSpec
    {
        public MFTSourceFileResident()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<MFTSourceFileResidentData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "entrynumber,sequencenumber,inuse,parententrynumber,parentsequencenumber,parentpath,filename,extension,filesize,referencecount,reparsetarget,isdirectory,hasads,isads,si<fn,useczeros,copied,siflags,nametype,created0x10,created0x30,lastmodified0x10,lastmodified0x30,lastrecordchange0x10,lastrecordchange0x30,lastaccess0x10,lastaccess0x30,updatesequencenumber,logfilesequencenumber,securityid,objectidfiledroid,loggedutilstream,zoneidcontents,sourcefile,residentdatabase64,residentdatahex,residentdataascii",
                "EntryNumber,SequenceNumber,InUse,ParentEntryNumber,ParentSequenceNumber,ParentPath,FileName,Extension,FileSize,ReferenceCount,ReparseTarget,IsDirectory,HasAds,IsAds,SI<FN,uSecZeros,Copied,SiFlags,NameType,Created0x10,Created0x30,LastModified0x10,LastModified0x30,LastRecordChange0x10,LastRecordChange0x30,LastAccess0x10,LastAccess0x30,UpdateSequenceNumber,LogfileSequenceNumber,SecurityId,ObjectIdFileDroid,LoggedUtilStream,ZoneIdContents,SourceFile,ResidentDataBase64,ResidentDataHex,ResidentDataASCII,ParseErrors"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from MFTECmd for $MFT with SourceFile and Resident Data";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "2d54bee2-225b-4170-b515-0fc4ff1b1f77";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<MFTSourceFileResidentData>(filename, DataList, TaggedLines, map =>
            {
                map.Map(t => t.Timestomped).Name("SI<FN");
                map.Map(m => m.Created0x10).TypeConverterOption.DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);
                map.Map(m => m.Created0x30).TypeConverterOption.DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);
                map.Map(m => m.LastModified0x10).TypeConverterOption.DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);
                map.Map(m => m.LastModified0x30).TypeConverterOption.DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);
                map.Map(m => m.LastRecordChange0x10).TypeConverterOption.DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);
                map.Map(m => m.LastRecordChange0x30).TypeConverterOption.DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);
                map.Map(m => m.LastAccess0x10).TypeConverterOption.DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);
                map.Map(m => m.LastAccess0x30).TypeConverterOption.DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);
            });
        }
    }

    public class MFTSourceFileData : IFileSpecData
    {
        public uint EntryNumber { get; set; }
        public ushort SequenceNumber { get; set; }
        public uint ParentEntryNumber { get; set; }
        public ushort? ParentSequenceNumber { get; set; }

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
        public string SourceFile { get; set; }

        public int Line { get; set; }

        public bool Tag { get; set; }

        public override string ToString()
        {
            return
                $"{EntryNumber} {SequenceNumber} {ParentEntryNumber} {ParentSequenceNumber} {InUse} {ParentPath} {FileName} {Extension} {IsDirectory} {HasAds} {IsAds} {FileSize} {Created0x10} {Created0x30} {LastModified0x10} {LastModified0x30} {LastRecordChange0x10} {LastRecordChange0x30} {LastAccess0x10} {LastAccess0x30} {UpdateSequenceNumber} {LogfileSequenceNumber} {SecurityId} {ZoneIdContents} {SiFlags} {ObjectIdFileDroid} {ReparseTarget}" +
                $" {ReferenceCount} {NameType} {LoggedUtilStream} {Timestomped} {uSecZeros} {Copied} {SourceFile}";
        }
    }

    public class MFTSourceFile : IFileSpec
    {
        public MFTSourceFile()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<MFTSourceFileData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "entrynumber,sequencenumber,inuse,parententrynumber,parentsequencenumber,parentpath,filename,extension,filesize,referencecount,reparsetarget,isdirectory,hasads,isads,si<fn,useczeros,copied,siflags,nametype,created0x10,created0x30,lastmodified0x10,lastmodified0x30,lastrecordchange0x10,lastrecordchange0x30,lastaccess0x10,lastaccess0x30,updatesequencenumber,logfilesequencenumber,securityid,objectidfiledroid,loggedutilstream,zoneidcontents,sourcefile"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from MFTECmd for $MFT with SourceFile";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "f47ac10b-58cc-4372-a567-0e02b2c3d479";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<MFTSourceFileData>(filename, DataList, TaggedLines, map =>
            {
                map.Map(t => t.Timestomped).Name("SI<FN");
                map.Map(m => m.Created0x10).TypeConverterOption.DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);
                map.Map(m => m.Created0x30).TypeConverterOption.DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);
                map.Map(m => m.LastModified0x10).TypeConverterOption.DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);
                map.Map(m => m.LastModified0x30).TypeConverterOption.DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);
                map.Map(m => m.LastRecordChange0x10).TypeConverterOption.DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);
                map.Map(m => m.LastRecordChange0x30).TypeConverterOption.DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);
                map.Map(m => m.LastAccess0x10).TypeConverterOption.DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);
                map.Map(m => m.LastAccess0x30).TypeConverterOption.DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);
            });
        }
    }
  

        public class MFTData : IFileSpecData
    {
        public uint EntryNumber { get; set; }
        public ushort SequenceNumber { get; set; }
        public uint ParentEntryNumber { get; set; }
        public ushort? ParentSequenceNumber { get; set; }

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
            CsvLoader.Load<MFTData>(filename, DataList, TaggedLines, map =>
            {
                map.Map(t => t.Timestomped).Name("SI<FN");
                map.Map(m => m.Created0x10).TypeConverterOption.DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);
                map.Map(m => m.Created0x30).TypeConverterOption.DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);
                map.Map(m => m.LastModified0x10).TypeConverterOption.DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);
                map.Map(m => m.LastModified0x30).TypeConverterOption.DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);
                map.Map(m => m.LastRecordChange0x10).TypeConverterOption.DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);
                map.Map(m => m.LastRecordChange0x30).TypeConverterOption.DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);
                map.Map(m => m.LastAccess0x10).TypeConverterOption.DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);
                map.Map(m => m.LastAccess0x30).TypeConverterOption.DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);
            });
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
            CsvLoader.Load<SbeCmdData>(filename, DataList, TaggedLines, map =>
            {
                map.Map(m => m.AccessedOn).TypeConverterOption.DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);
                map.Map(m => m.CreatedOn).TypeConverterOption.DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);
                map.Map(m => m.ModifiedOn).TypeConverterOption.DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);
                map.Map(m => m.FirstInteracted).TypeConverterOption.DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);
                map.Map(m => m.LastInteracted).TypeConverterOption.DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);
                map.Map(m => m.LastWriteTime).TypeConverterOption.DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);
            });
        }
    }

    public class RbCmdData : IFileSpecData
    {
        public string SourceName { get; set; }
        public string FileType { get; set; }
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public DateTime DeletedOn { get; set; }

        //added in the 2026.10 versions of the tool
        public bool? Removed { get; set; }

        public int Line { get; set; }

        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{SourceName} {FileType} {FileName} {FileSize} {DeletedOn} {Removed}";
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
                "SourceName,FileType,FileName,FileSize,DeletedOn",
                "SourceName,FileType,FileName,FileSize,DeletedOn,Removed"
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
            CsvLoader.Load<RbCmdData>(filename, DataList, TaggedLines, map => map.Map(m => m.DeletedOn).TypeConverterOption.DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal));
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
        //added in the 2026.10 versions of the tool
        public string FullPath { get; set; }
        public bool? PathValidated { get; set; }

        public int Line { get; set; }

        public bool Tag { get; set; }

        public override string ToString()
        {
            return
                $"{Note} {SourceFilename} {SourceCreated} {SourceModified} {SourceAccessed} {ExecutableName} {Hash} {Size} {Version} {RunCount} {LastRun} {PreviousRun0} {PreviousRun1} {PreviousRun2} {PreviousRun2} {PreviousRun3} {PreviousRun4} {PreviousRun5} {PreviousRun6} {Volume0Name} {Volume0Serial} {Volume0Created} {Volume1Name} {Volume1Serial} {Volume1Created} {Directories} {FilesLoaded} {ParsingError} {FullPath} {PathValidated}";
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
                "Note,SourceFilename,SourceCreated,SourceModified,SourceAccessed,ExecutableName,Hash,Size,Version,RunCount,LastRun,PreviousRun0,PreviousRun1,PreviousRun2,PreviousRun3,PreviousRun4,PreviousRun5,PreviousRun6,Volume0Name,Volume0Serial,Volume0Created,Volume1Name,Volume1Serial,Volume1Created,Directories,FilesLoaded,ParsingError",
                "Note,SourceFilename,SourceCreated,SourceModified,SourceAccessed,ExecutableName,FullPath,PathValidated,Hash,Size,Version,RunCount,LastRun,PreviousRun0,PreviousRun1,PreviousRun2,PreviousRun3,PreviousRun4,PreviousRun5,PreviousRun6,Volume0Name,Volume0Serial,Volume0Created,Volume1Name,Volume1Serial,Volume1Created,Directories,FilesLoaded,ParsingError"
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
            CsvLoader.Load<PeCmdData>(filename, DataList, TaggedLines);
        }
    }


    public class PECmdTimelineData : IFileSpecData
    {
        public DateTime RunTime { get; set; }
        public string ExecutableName { get; set; }
        //added in the 2026.10 versions of the tool
        public string SourceFilename { get; set; }
        public bool? PathValidated { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{RunTime} {ExecutableName} {SourceFilename} {PathValidated}";
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
                "RunTime,ExecutableName",
                "RunTime,ExecutableName,SourceFilename,PathValidated"
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
            CsvLoader.Load<PECmdTimelineData>(filename, DataList, TaggedLines);
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
        //added in the 2026.10 versions of the tool
        public string Name { get; set; }
        public string IconLocation { get; set; }
        public string ShowWindow { get; set; }
        public string EnvironmentVariables { get; set; }
        public string VolumeDroid { get; set; }
        public string FileDroid { get; set; }
        public string VolumeDroidBirth { get; set; }
        public string FileDroidBirth { get; set; }
        public string Anomalies { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return
                $"{SourceFile} {SourceCreated} {SourceModified} {SourceAccessed} {TargetCreated} {TargetModified} {TargetAccessed} {FileSize} {RelativePath} {WorkingDirectory} {FileAttributes} {HeaderFlags} {DriveType} {VolumeSerialNumber} {VolumeLabel} {LocalPath} {NetworkPath} {CommonPath} {Arguments} {TargetIDAbsolutePath} {TargetMFTEntryNumber} {TargetMFTSequenceNumber} {MachineID} {MachineMACAddress} {MACVendor} {TrackerCreatedOn} {ExtraBlocksPresent} {Name} {IconLocation} {ShowWindow} {EnvironmentVariables} {VolumeDroid} {FileDroid} {VolumeDroidBirth} {FileDroidBirth} {Anomalies}";
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
                "SourceFile,SourceCreated,SourceModified,SourceAccessed,TargetCreated,TargetModified,TargetAccessed,FileSize,RelativePath,WorkingDirectory,FileAttributes,HeaderFlags,DriveType,VolumeSerialNumber,VolumeLabel,LocalPath,NetworkPath,CommonPath,Arguments,TargetIDAbsolutePath,TargetMFTEntryNumber,TargetMFTSequenceNumber,MachineID,MachineMACAddress,MACVendor,TrackerCreatedOn,ExtraBlocksPresent",
                "SourceFile,SourceCreated,SourceModified,SourceAccessed,TargetCreated,TargetModified,TargetAccessed,FileSize,RelativePath,WorkingDirectory,FileAttributes,HeaderFlags,DriveType,VolumeSerialNumber,VolumeLabel,LocalPath,NetworkPath,CommonPath,Arguments,TargetIDAbsolutePath,TargetMFTEntryNumber,TargetMFTSequenceNumber,MachineID,MachineMACAddress,MACVendor,TrackerCreatedOn,ExtraBlocksPresent,Name,IconLocation,ShowWindow,EnvironmentVariables,VolumeDroid,FileDroid,VolumeDroidBirth,FileDroidBirth,Anomalies"
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
            CsvLoader.Load<LECmdData>(filename, DataList, TaggedLines);
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
        public bool HasSps { get; set; }

        public int DestListVersion { get; set; }
        public int LastUsedEntryNumber { get; set; }
        public int MRU { get; set; }
        public string EntryNumber { get; set; }
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
        //added in the 2026.10 versions of the tool
        public string User { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return
                $"{SourceFile} {SourceCreated} {SourceModified} {SourceAccessed} {AppId} {AppIdDescription} {HasSps} {DestListVersion} {LastUsedEntryNumber} {MRU} {EntryNumber} {CreationTime} {LastModified} {Hostname} {MacAddress} {Path} {InteractionCount} {PinStatus} {FileBirthDroid} {FileDroid} {VolumeBirthDroid} {VolumeDroid} {TargetCreated} {TargetModified} {TargetAccessed} {FileSize} {RelativePath} {WorkingDirectory} {FileAttributes} {HeaderFlags} {DriveType} {VolumeSerialNumber}" +
                $" {VolumeLabel} {LocalPath} {CommonPath} {Arguments} {TargetIDAbsolutePath} {TargetMFTEntryNumber} {TargetMFTSequenceNumber} {MachineID} {MachineMACAddress} {TrackerCreatedOn} {ExtraBlocksPresent} {Notes} {User}";
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
                "SourceFile,SourceCreated,SourceModified,SourceAccessed,AppId,AppIdDescription,HasSps,DestListVersion,LastUsedEntryNumber,MRU,EntryNumber,CreationTime,LastModified,Hostname,MacAddress,Path,InteractionCount,PinStatus,FileBirthDroid,FileDroid,VolumeBirthDroid,VolumeDroid,TargetCreated,TargetModified,TargetAccessed,FileSize,RelativePath,WorkingDirectory,FileAttributes,HeaderFlags,DriveType,VolumeSerialNumber,VolumeLabel,LocalPath,CommonPath,TargetIDAbsolutePath,TargetMFTEntryNumber,TargetMFTSequenceNumber,MachineID,MachineMACAddress,TrackerCreatedOn,ExtraBlocksPresent,Arguments,Notes",
                "User,SourceFile,SourceCreated,SourceModified,SourceAccessed,AppId,AppIdDescription,HasSps,DestListVersion,LastUsedEntryNumber,MRU,EntryNumber,CreationTime,LastModified,Hostname,MacAddress,Path,InteractionCount,PinStatus,FileBirthDroid,FileDroid,VolumeBirthDroid,VolumeDroid,TargetCreated,TargetModified,TargetAccessed,FileSize,RelativePath,WorkingDirectory,FileAttributes,HeaderFlags,DriveType,VolumeSerialNumber,VolumeLabel,LocalPath,CommonPath,TargetIDAbsolutePath,TargetMFTEntryNumber,TargetMFTSequenceNumber,MachineID,MachineMACAddress,TrackerCreatedOn,ExtraBlocksPresent,Arguments,Notes"
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
            CsvLoader.Load<JleCmdAutoData>(filename, DataList, TaggedLines, fixup: record =>
            {
                if (record.FileAttributes == "0")
                {
                    record.FileAttributes = string.Empty;
                }
            });
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
        //added in the 2026.10 versions of the tool
        public string User { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return
                $"{SourceFile} {SourceCreated} {SourceModified} {SourceAccessed} {AppId} {AppIdDescription} {EntryName} {TargetCreated} {TargetModified} {TargetAccessed} {FileSize} {RelativePath} {WorkingDirectory} {FileAttributes} {HeaderFlags} {DriveType} {VolumeSerialNumber} {VolumeLabel} {LocalPath} {CommonPath} {Arguments} {TargetIDAbsolutePath} {TargetMFTEntryNumber} {TargetMFTSequenceNumber} {MachineID} {MachineMACAddress} {TrackerCreatedOn} {ExtraBlocksPresent} {User}";
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
                "SourceFile,SourceCreated,SourceModified,SourceAccessed,AppId,AppIdDescription,EntryName,TargetCreated,TargetModified,TargetAccessed,FileSize,RelativePath,WorkingDirectory,FileAttributes,HeaderFlags,DriveType,VolumeSerialNumber,VolumeLabel,LocalPath,CommonPath,TargetIDAbsolutePath,TargetMFTEntryNumber,TargetMFTSequenceNumber,MachineID,MachineMACAddress,TrackerCreatedOn,ExtraBlocksPresent,Arguments",
                "User,SourceFile,SourceCreated,SourceModified,SourceAccessed,AppId,AppIdDescription,EntryName,TargetCreated,TargetModified,TargetAccessed,FileSize,RelativePath,WorkingDirectory,FileAttributes,HeaderFlags,DriveType,VolumeSerialNumber,VolumeLabel,LocalPath,CommonPath,TargetIDAbsolutePath,TargetMFTEntryNumber,TargetMFTSequenceNumber,MachineID,MachineMACAddress,TrackerCreatedOn,ExtraBlocksPresent,Arguments"
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
            CsvLoader.Load<JleCmdCustomData>(filename, DataList, TaggedLines);
        }
    }

    public class EvtxECmdData : IFileSpecData
    {
        public ulong RecordNumber { get; set; }
        public ulong EventRecordId { get; set; }
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

        //added in the 2026.10 versions of the tool
        public string RenderingInfo { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return
                $"{RecordNumber} {EventRecordId} {TimeCreated} {EventId} {Level} {Provider} {Channel} {ProcessId} {ThreadId} {Computer} {UserId} {MapDescription} {UserName} {RemoteHost} {PayloadData1} {PayloadData2} {PayloadData3} {PayloadData4} {PayloadData5} {PayloadData6} {ExecutableInfo} {SourceFile} {Keywords} {HiddenRecord} {Payload} {RenderingInfo}";
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
                "RecordNumber,EventRecordId,TimeCreated,EventId,Level,Provider,Channel,ProcessId,ThreadId,Computer,ChunkNumber,UserId,MapDescription,UserName,RemoteHost,PayloadData1,PayloadData2,PayloadData3,PayloadData4,PayloadData5,PayloadData6,ExecutableInfo,HiddenRecord,SourceFile,Keywords,ExtraDataOffset,Payload",
                "RecordNumber,EventRecordId,TimeCreated,EventId,Level,Provider,Channel,ProcessId,ThreadId,Computer,ChunkNumber,UserId,MapDescription,UserName,RemoteHost,PayloadData1,PayloadData2,PayloadData3,PayloadData4,PayloadData5,PayloadData6,ExecutableInfo,HiddenRecord,SourceFile,Keywords,ExtraDataOffset,Payload,RenderingInfo"
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
            CsvLoader.Load<EvtxECmdData>(filename, DataList, TaggedLines, configure: config => { config.ShouldUseConstructorParameters = _ => false; });
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
            CsvLoader.Load<RecmdBatchData>(filename, DataList, TaggedLines);
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
        //added in the 2026.10 versions of the tool
        public bool? FromSlack { get; set; }

        public int Line { get; set; }

        public bool Tag { get; set; }

        public override string ToString()
        {
            return
                $"{ControlSet} {CacheEntryPosition} {Path} {LastModifiedTimeUTC} {Executed} {Duplicate} {SourceFile} {FromSlack}";
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
                "ControlSet,CacheEntryPosition,Path,LastModifiedTimeUTC,Executed,Duplicate,SourceFile",
                "ControlSet,CacheEntryPosition,Path,LastModifiedTimeUTC,Executed,Duplicate,SourceFile,FromSlack"
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
            CsvLoader.Load<AppCompatCacheData>(filename, DataList, TaggedLines);
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
            CsvLoader.Load<ActivityPackageData>(filename, DataList, TaggedLines, map => map.Map(m => m.Expires).TypeConverterOption.DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal));
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

        //added in the 2026.10 versions of the tool
        public string AppActivityId { get; set; }
        public string ClipboardText { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return
                $"{Id} {Executable} {DisplayText} {Payload} {ClipboardPayload} {ContentInfo} {LastModifiedTime} {ExpirationTime} {CreatedInCloud} {StartTime} {EndTime} {Duration} {LastModifiedOnClient} {OriginalLastModifiedOnClient} {ActivityType} {ActivityTypeOrg} {IsLocalOnly} {ETag} {PackageIdHash} {PlatformDeviceId} {DevicePlatform} {TimeZone} {AppActivityId} {ClipboardText}";
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
                "Id,ActivityTypeOrg,ActivityType,Executable,DisplayText,ContentInfo,Payload,ClipboardPayload,StartTime,EndTime,Duration,LastModifiedTime,LastModifiedOnClient,OriginalLastModifiedOnClient,ExpirationTime,CreatedInCloud,IsLocalOnly,ETag,PackageIdHash,PlatformDeviceId,DevicePlatform,TimeZone",
                "Id,ActivityTypeOrg,ActivityType,Executable,DisplayText,ContentInfo,Payload,ClipboardPayload,StartTime,EndTime,Duration,LastModifiedTime,LastModifiedOnClient,OriginalLastModifiedOnClient,ExpirationTime,CreatedInCloud,IsLocalOnly,ETag,PackageIdHash,PlatformDeviceId,DevicePlatform,TimeZone,AppActivityId,ClipboardText"
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
            CsvLoader.Load<ActivityData>(filename, DataList, TaggedLines, map =>
            {
                map.Map(m => m.CreatedInCloud).TypeConverterOption.DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);
                map.Map(m => m.EndTime).TypeConverterOption.DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);
                map.Map(m => m.ExpirationTime).TypeConverterOption.DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);
                map.Map(m => m.LastModifiedOnClient).TypeConverterOption.DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);
                map.Map(m => m.LastModifiedTime).TypeConverterOption.DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);
                map.Map(m => m.OriginalLastModifiedOnClient).TypeConverterOption.DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);
                map.Map(m => m.StartTime).TypeConverterOption.DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);
            });
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

        //added in the 2026.10 versions of the tool
        public string AppActivityId { get; set; }
        public string ClipboardText { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return
                $"{OperationOrder} {OperationType} {AppId} {Executable} {Description} {StartTime} {DisplayText} {ClipboardPayload} {ContentInfo} {DevicePlatform} {TimeZone} {ActivityTypeOrg} {ActivityType} {Duration} {LastModifiedTime} {ExpirationTime} {Payload} {CreatedTime} {EndTime} {LastModifiedTimeOnClient} {OperationExpirationTime} {PlatformDeviceId} {AppActivityId} {ClipboardText}";
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
                "Id,ActivityTypeOrg,ActivityType,Executable,DisplayText,ContentInfo,Payload,ClipboardPayload,StartTime,EndTime,Duration,LastModifiedTime,LastModifiedTimeOnClient,CreatedTime,ExpirationTime,OperationExpirationTime,OperationOrder,AppId,OperationType,Description,PlatformDeviceId,DevicePlatform,TimeZone",
                "Id,ActivityTypeOrg,ActivityType,Executable,DisplayText,ContentInfo,Payload,ClipboardPayload,StartTime,EndTime,Duration,LastModifiedTime,LastModifiedTimeOnClient,CreatedTime,ExpirationTime,OperationExpirationTime,OperationOrder,AppId,OperationType,Description,PlatformDeviceId,DevicePlatform,TimeZone,AppActivityId,ClipboardText"
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
            CsvLoader.Load<ActivityOperationData>(filename, DataList, TaggedLines, map =>
            {
                map.Map(m => m.CreatedTime).TypeConverterOption.DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);
                map.Map(m => m.EndTime).TypeConverterOption.DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);
                map.Map(m => m.ExpirationTime).TypeConverterOption.DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);
                map.Map(m => m.LastModifiedTimeOnClient).TypeConverterOption.DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);
                map.Map(m => m.LastModifiedTime).TypeConverterOption.DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);
                map.Map(m => m.OperationExpirationTime).TypeConverterOption.DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);
                map.Map(m => m.StartTime).TypeConverterOption.DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);
            });
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
                "ProgramID,LastWriteTimestamp,ProgramName_0,ProgramVersion_1,VendorName_2,InstallDateEpoch_a,InstallDateEpoch_b,LanguageCode_3,InstallSource_6,UninstallRegistryKey_7,PathsList_d,UnknownGuid_10,UnknownGuid_12,UninstallGuid_11,UnknownDword_5,UnknownDword_13,UnknownDword_14,UnknownDword_15,UnknownBytes_16,UnknownQWord_17,UnknownDword_18,UninstallGuid_f",
                //the old format's programs CSV as AmcacheParser writes it (2026.5 too)
                "ProgramID,LastWriteTimestamp,ProgramName_0,ProgramVersion_1,VendorName_2,InstallDateEpoch_a,UnknownGuid_10,InstallDateEpoch_b,LanguageCode_3,UnknownGuid_12,InstallSource_6,UninstallGuid_11,UninstallRegistryKey_7,UnknownDword_5,PathsList_d,UnknownDword_13,UnknownDword_14,UnknownDword_15,UnknownBytes_16,UnknownQWord_17,UnknownDword_18,UninstallGuid_f"
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
            CsvLoader.Load<AmcacheParserProgramsData>(filename, DataList, TaggedLines);
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
                "ProgramName,ProgramID,VolumeID,VolumeIDLastWriteTimestamp,FileID,FileIDLastWriteTimestamp,SHA1,FullPath,FileExtension,MFTEntryNumber,MFTSequenceNumber,FileSize,FileVersionString,FileVersionNumber,FileDescription,SizeOfImage,PEHeaderHash,PEHeaderChecksum,BinProductVersion,BinFileVersion,LinkerVersion,BinaryType,IsLocal,GuessProgramID,Created,LastModified,LastModifiedStore,LinkDate,LanguageID,ProductName,CompanyName,SwitchBackContext",
                //the old format's associated files CSV (its columns are in another order than the unassociated one)
                "ProgramName,ProgramID,ProductName,VolumeID,CompanyName,VolumeIDLastWriteTimestamp,FileID,FileIDLastWriteTimestamp,SHA1,FullPath,FileExtension,MFTEntryNumber,MFTSequenceNumber,FileSize,FileVersionString,FileVersionNumber,FileDescription,SwitchBackContext,SizeOfImage,PEHeaderHash,PEHeaderChecksum,BinProductVersion,BinFileVersion,LinkerVersion,BinaryType,IsLocal,GuessProgramID,Created,LastModified,LastModifiedStore,LinkDate,LanguageID"
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
            CsvLoader.Load<AmcacheParserFilesData>(filename, DataList, TaggedLines);
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
        public string Description { get; set; }

        //added in the 2026.10 versions of the tool
        public string Publisher { get; set; }
        public string Sha256 { get; set; }
        public string KeyName { get; set; }
        public string AppxPackageFullName { get; set; }
        public string AppxPackageRelativeId { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return
                $"{ApplicationName} {ProgramId} {FileKeyLastWriteTimestamp} {SHA1} {IsOsComponent} {FullPath} {Name} {FileExtension} {LinkDate} {ProductName} {Size} {Version} {ProductVersion} {LongPathHash} {BinaryType} {IsPeFile} {BinFileVersion} {BinProductVersion} {Usn} {Language} {Description} {Publisher} {Sha256} {KeyName} {AppxPackageFullName} {AppxPackageRelativeId}";
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
                "ApplicationName,ProgramId,FileKeyLastWriteTimestamp,SHA1,IsOsComponent,FullPath,Name,FileExtension,LinkDate,ProductName,Size,Version,ProductVersion,LongPathHash,BinaryType,IsPeFile,BinFileVersion,BinProductVersion,Usn,Language,Description",
                "ApplicationName,ProgramId,FileKeyLastWriteTimestamp,SHA1,IsOsComponent,FullPath,Name,FileExtension,LinkDate,ProductName,Size,Version,ProductVersion,LongPathHash,BinaryType,IsPeFile,BinFileVersion,BinProductVersion,Usn,Language,Description,OriginalFileName",
                "ApplicationName,ProgramId,FileKeyLastWriteTimestamp,SHA1,IsOsComponent,FullPath,Name,FileExtension,LinkDate,ProductName,Size,Version,ProductVersion,LongPathHash,BinaryType,IsPeFile,BinFileVersion,BinProductVersion,Usn,Language,Description,OriginalFileName,Publisher,Sha256,KeyName,AppxPackageFullName,AppxPackageRelativeId"
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
            CsvLoader.Load<AmcacheParserNewFilesData>(filename, DataList, TaggedLines, map => map.Map(t => t.Usn).Ignore());
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
        //added in the 2026.10 versions of the tool
        public string DeviceState { get; set; }
        public string InstallDate { get; set; }
        public string FirstInstallDate { get; set; }
        public string ExtendedInfs { get; set; }
        public string UpperClassFilters { get; set; }
        public string LowerClassFilters { get; set; }
        public string UpperFilters { get; set; }
        public string LowerFilters { get; set; }
        public string DeviceInterfaceClasses { get; set; }
        public string LocationPaths { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return
                $"{KeyName} {KeyLastWriteTimestamp} {BusReportedDescription} {Class} {ClassGuid} {Compid} {ContainerId} {Description} {DriverId} {DriverPackageStrongName} {DriverName} {DriverVerDate} {DriverVerVersion} {Enumerator} {HWID} {Inf} {InstallState} {Manufacturer} {MatchingId} {Model} {ParentId} {ProblemCode} {Provider} {Service} {Stackid} {DeviceState} {InstallDate} {FirstInstallDate} {ExtendedInfs} {UpperClassFilters} {LowerClassFilters} {UpperFilters} {LowerFilters} {DeviceInterfaceClasses} {LocationPaths}";
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
                "KeyName,KeyLastWriteTimestamp,BusReportedDescription,Class,ClassGuid,Compid,ContainerId,Description,DriverId,DriverPackageStrongName,DriverName,DriverVerDate,DriverVerVersion,Enumerator,HWID,Inf,InstallState,Manufacturer,MatchingId,Model,ParentId,ProblemCode,Provider,Service,Stackid",
                "KeyName,KeyLastWriteTimestamp,BusReportedDescription,Class,ClassGuid,Compid,ContainerId,Description,DriverId,DriverPackageStrongName,DriverName,DriverVerDate,DriverVerVersion,Enumerator,HWID,Inf,InstallState,Manufacturer,MatchingId,Model,ParentId,ProblemCode,Provider,Service,Stackid,DeviceState,InstallDate,FirstInstallDate,ExtendedInfs,UpperClassFilters,LowerClassFilters,UpperFilters,LowerFilters,DeviceInterfaceClasses,LocationPaths"
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
            CsvLoader.Load<AmcacheParserNewDevicePnPData>(filename, DataList, TaggedLines);
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
        public int? State { get; set; }
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
            CsvLoader.Load<AmcacheParserDeviceContainerData>(filename, DataList, TaggedLines);
        }
    }

    public class AmcacheParserDriverPackageData : IFileSpecData
    {
        public string KeyName { get; set; }
        public DateTime KeyLastWriteTimestamp { get; set; }
        public DateTime? Date { get; set; }
        public string Class { get; set; }
        public string Directory { get; set; }
        public bool DriverInBox { get; set; }
        public string Hwids { get; set; }
        public string Inf { get; set; }
        public string Provider { get; set; }
        public string SubmissionId { get; set; }
        public string SYSFILE { get; set; }
        public string Version { get; set; }
        //added in the 2026.10 versions of the tool
        public string ClassGuid { get; set; }
        public string DriverPackageStrongName { get; set; }
        public string FlightIds { get; set; }
        public string RecoveryIds { get; set; }
        public string IsActive { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return
                $"{KeyName} {KeyLastWriteTimestamp} {Date} {Class} {Directory} {DriverInBox} {Hwids} {Inf} {Provider} {SubmissionId} {SYSFILE} {Version} {ClassGuid} {DriverPackageStrongName} {FlightIds} {RecoveryIds} {IsActive}";
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
                "KeyName,KeyLastWriteTimestamp,Date,Class,Directory,DriverInBox,Hwids,Inf,Provider,SubmissionId,SYSFILE,Version",
                "KeyName,KeyLastWriteTimestamp,Date,Class,Directory,DriverInBox,Hwids,Inf,Provider,SubmissionId,SYSFILE,Version,ClassGuid,DriverPackageStrongName,FlightIds,RecoveryIds,IsActive"
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
            CsvLoader.Load<AmcacheParserDriverPackageData>(filename, DataList, TaggedLines);
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
            CsvLoader.Load<AmcacheParserDriverBinaryData>(filename, DataList, TaggedLines);
        }
    }


    public class AmcacheParserNewShortcutData : IFileSpecData
    {
        public string KeyName { get; set; }
        public string LnkName { get; set; }
        public DateTime KeyLastWriteTimestamp { get; set; }
        //added in the 2026.10 versions of the tool
        public string ShortcutTargetPath { get; set; }
        public string ShortcutAumid { get; set; }
        public string ShortcutProgramId { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{KeyName} {LnkName} {KeyLastWriteTimestamp} {ShortcutTargetPath} {ShortcutAumid} {ShortcutProgramId}";
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
                "KeyName,LnkName,KeyLastWriteTimestamp",
                "KeyName,LnkName,KeyLastWriteTimestamp,ShortcutTargetPath,ShortcutAumid,ShortcutProgramId"
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
            CsvLoader.Load<AmcacheParserNewShortcutData>(filename, DataList, TaggedLines);
        }
    }

    public class AmcacheParserNewProgramData : IFileSpecData
    {
        public string ProgramId { get; set; }
        public DateTime KeyLastWriteTimestamp { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public string Publisher { get; set; }
        public DateTime? InstallDateArpLastModified { get; set; }
        public DateTime? InstallDate { get; set; }
        public DateTime? InstallDateMsi { get; set; }
        public string OSVersionAtInstallTime { get; set; }
        public DateTime? InstallDateFromLinkFile { get; set; }
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
        public string Manufacturer { get; set; }
        //added in the 2026.10 versions of the tool
        public string KeyName { get; set; }
        public string UserSid { get; set; }
        public DateTime? MsiInstallDate { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return
                $"{ProgramId} {KeyLastWriteTimestamp} {Name} {Version} {Publisher} {InstallDateArpLastModified} {InstallDate} {InstallDateMsi} {OSVersionAtInstallTime} {InstallDateFromLinkFile} {BundleManifestPath} {HiddenArp} {InboxModernApp} {Language} {ManifestPath} {MsiPackageCode} {MsiProductCode} {PackageFullName} {ProgramInstanceId} {RegistryKeyPath} {RootDirPath} {Type} {Source} {StoreAppType} {UninstallString} {Manufacturer} {KeyName} {UserSid} {MsiInstallDate}";
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
                "ProgramId,KeyLastWriteTimestamp,Name,Version,Publisher,InstallDateArpLastModified,InstallDate,InstallDateMsi,OSVersionAtInstallTime,InstallDateFromLinkFile,BundleManifestPath,HiddenArp,InboxModernApp,Language,ManifestPath,MsiPackageCode,MsiProductCode,PackageFullName,ProgramInstanceId,RegistryKeyPath,RootDirPath,Type,Source,StoreAppType,UninstallString,Manufacturer",
                "ProgramId,KeyLastWriteTimestamp,Name,Version,Publisher,InstallDateArpLastModified,InstallDate,InstallDateMsi,OSVersionAtInstallTime,InstallDateFromLinkFile,BundleManifestPath,HiddenArp,InboxModernApp,Language,ManifestPath,MsiPackageCode,MsiProductCode,PackageFullName,ProgramInstanceId,RegistryKeyPath,RootDirPath,Type,Source,StoreAppType,UninstallString,Manufacturer,KeyName,UserSid,MsiInstallDate"
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
            CsvLoader.Load<AmcacheParserNewProgramData>(filename, DataList, TaggedLines);
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
            CsvLoader.Load<NetworkUsageData>(filename, DataList, TaggedLines);
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
            CsvLoader.Load<PushNotificationData>(filename, DataList, TaggedLines);
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
            CsvLoader.Load<NetworkConnectionData>(filename, DataList, TaggedLines);
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
            CsvLoader.Load<AppResourceUseInfoData>(filename, DataList, TaggedLines);
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
            CsvLoader.Load<UnknownD8FData>(filename, DataList, TaggedLines);
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

        //added in the 2026.10 versions of the tool
        public int? InFocusS { get; set; }
        public int? PsmForegroundS { get; set; }
        public int? UserInputS { get; set; }
        public int? KeyboardInputS { get; set; }
        public int? MouseInputS { get; set; }
        public int? DisplayRequiredS { get; set; }
        public int? AudioInS { get; set; }
        public int? AudioOutS { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return
                $"{Timestamp} {Id} {ExeInfo} {ExeInfoDescription} {ExeTimestamp} {SidType} {Sid} {UserName} {UserId} {AppId} {EndTime} {DurationMs} {InFocusS} {PsmForegroundS} {UserInputS} {KeyboardInputS} {MouseInputS} {DisplayRequiredS} {AudioInS} {AudioOutS}";
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
                "Id,Timestamp,ExeInfo,ExeInfoDescription,ExeTimestamp,SidType,Sid,UserName,UserId,AppId,EndTime,DurationMs",
                "Id,Timestamp,ExeInfo,ExeInfoDescription,ExeTimestamp,SidType,Sid,UserName,UserId,AppId,EndTime,DurationMs,InFocusS,PsmForegroundS,UserInputS,KeyboardInputS,MouseInputS,DisplayRequiredS,AudioInS,AudioOutS"
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
            CsvLoader.Load<Unknown312Data>(filename, DataList, TaggedLines);
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
            CsvLoader.Load<EnergyUsageData>(filename, DataList, TaggedLines);
        }
    }

     public class MFTFileListingData : IFileSpecData
    {
        public string FullPath { get; set; }
        public string Extension { get; set; }

        public bool IsDirectory { get; set; }
        public ulong FileSize { get; set; }
        public DateTime? Created0x10 { get; set; }
        public DateTime? LastModified0x10 { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return
                $"{FullPath} {Extension} {FileSize} {Created0x10} {LastModified0x10}";
        }
    }

    public class MFTFileListing : IFileSpec
    {
        public MFTFileListing()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<MFTFileListingData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "FullPath,Extension,IsDirectory,FileSize,Created0x10,LastModified0x10"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from MFTECmd --fl mode";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "40dd7405-22cf-1612-b180-1a011d0a9952";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<MFTFileListingData>(filename, DataList, TaggedLines, map =>
            {
                map.Map(m => m.Created0x10).TypeConverterOption.DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);
                map.Map(m => m.LastModified0x10).TypeConverterOption.DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);
            });
        }
    }


     public class SumChainedDbData : IFileSpecData
    {
        public int Year { get; set; }
        public string FileName { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return
                $"{Year} {FileName}";
        }
    }

    public class SumChainedDb : IFileSpec
    {
        public SumChainedDb()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<SumChainedDbData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Year,FileName"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from SumECmd Chained Db Info";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "40ee6415-22ce-1312-b180-1b011d0a9952";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<SumChainedDbData>(filename, DataList, TaggedLines);
        }
    }


     public class SumSystemIdentInfoData : IFileSpecData
    {
        public DateTime? CreationTime { get; set; }

        public int OsMajor { get; set; }
        public int OsMinor { get; set; }

        public int OsBuild { get; set; }

        //added in the 2026.10 versions of the tool
        public string SystemDnsHostName { get; set; }
        public string SystemDomainName { get; set; }
        public string SystemManufacturer { get; set; }
        public string SystemProductName { get; set; }
        public string SystemSerialNumber { get; set; }
        public string SystemSmbiosUuid { get; set; }
        public string OsSerialNumber { get; set; }
        public string OsLastBootUpTime { get; set; }
        public string OsCountryCode { get; set; }
        public int? OsCurrentTimeZone { get; set; }
        public bool? OsDaylightInEffect { get; set; }
        public int? OsProductType { get; set; }
        public int? OsSuiteMask { get; set; }
        public int? OsPlatformId { get; set; }
        public int? ServicePackMajor { get; set; }
        public int? ServicePackMinor { get; set; }
        public int? PhysicalProcessorCount { get; set; }
        public int? CoresPerPhysicalProcessor { get; set; }
        public int? LogicalProcessorsPerPhysicalProcessor { get; set; }
        public long? MaximumMemory { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return
                $"{CreationTime} {OsMajor} {OsMinor} {OsBuild} {SystemDnsHostName} {SystemDomainName} {SystemManufacturer} {SystemProductName} {SystemSerialNumber} {SystemSmbiosUuid} {OsSerialNumber} {OsLastBootUpTime} {OsCountryCode} {OsCurrentTimeZone} {OsDaylightInEffect} {OsProductType} {OsSuiteMask} {OsPlatformId} {ServicePackMajor} {ServicePackMinor} {PhysicalProcessorCount} {CoresPerPhysicalProcessor} {LogicalProcessorsPerPhysicalProcessor} {MaximumMemory}";
        }
    }

    public class SumSystemIdentInfo : IFileSpec
    {
        public SumSystemIdentInfo()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<SumSystemIdentInfoData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "CreationTime,OsMajor,OsMinor,OsBuild",
                "CreationTime,OsMajor,OsMinor,OsBuild,SystemDnsHostName,SystemDomainName,SystemManufacturer,SystemProductName,SystemSerialNumber,SystemSmbiosUuid,OsSerialNumber,OsLastBootUpTime,OsCountryCode,OsCurrentTimeZone,OsDaylightInEffect,OsProductType,OsSuiteMask,OsPlatformId,ServicePackMajor,ServicePackMinor,PhysicalProcessorCount,CoresPerPhysicalProcessor,LogicalProcessorsPerPhysicalProcessor,MaximumMemory"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from SumECmd System Ident Info";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "40ee6414-22df-2723-b280-1a021d0a9952";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<SumSystemIdentInfoData>(filename, DataList, TaggedLines, map => map.Map(m => m.CreationTime).TypeConverterOption.DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal));
        }
    }



    
     public class SumRoleInfoData : IFileSpecData
    {
        public string RoleGuid { get; set; }
        public string RoleName { get; set; }

        public string ProductName { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return
                $"{RoleGuid} {RoleName} {ProductName}";
        }
    }

    public class SumRoleInfo : IFileSpec
    {
        public SumRoleInfo()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<SumRoleInfoData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "RoleGuid,RoleName,ProductName"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from SumECmd Role Info";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "40ee6414-22df-2723-b312-2a134d0a9952";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<SumRoleInfoData>(filename, DataList, TaggedLines);
        }
    }


    
     public class SumRoleAccessData : IFileSpecData
    {
        public string RoleGuid { get; set; }
        public string RoleDescription { get; set; }

        public DateTime FirstSeen { get;set; }
        public DateTime LastSeen { get;set; }

        public string SourceFile { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return
                $"{RoleGuid} {RoleDescription} {FirstSeen} {LastSeen} {SourceFile}";
        }
    }

    public class SumRoleAccess : IFileSpec
    {
        public SumRoleAccess()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<SumRoleAccessData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "RoleGuid,RoleDescription,FirstSeen,LastSeen,SourceFile"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from SumECmd Role Access";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "51ab5323-22df-2723-b312-2a134d0a9952";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<SumRoleAccessData>(filename, DataList, TaggedLines, map =>
            {
                map.Map(m => m.FirstSeen).TypeConverterOption.DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);
                map.Map(m => m.LastSeen).TypeConverterOption.DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);
            });
        }
    }


      public class SumClientData : IFileSpecData
    {
        public string RoleGuid { get; set; }
        public string RoleDescription { get; set; }
        public string AuthenticatedUserName { get; set; }
        public int TotalAccesses { get; set; }

        public DateTime InsertDate { get;set; }
        public DateTime LastAccess { get;set; }

        public string IpAddress { get; set; }
        public string ClientName { get; set; }
        public string TenantId { get; set; }
        public string SourceFile { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return
                $"{RoleGuid} {RoleDescription} {AuthenticatedUserName} {TotalAccesses} {InsertDate} {LastAccess} {IpAddress} {ClientName} {TenantId} {SourceFile}";
        }
    }

    public class SumClient : IFileSpec
    {
        public SumClient()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<SumClientData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "RoleGuid,RoleDescription,AuthenticatedUserName,TotalAccesses,InsertDate,LastAccess,IpAddress,ClientName,TenantId,SourceFile"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from SumECmd Client";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "12cb4452-81ef-2723-b312-2a134d0a9952";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<SumClientData>(filename, DataList, TaggedLines, map =>
            {
                map.Map(m => m.InsertDate).TypeConverterOption.DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);
                map.Map(m => m.LastAccess).TypeConverterOption.DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);
            });
        }
    }

    

      public class SumClientDetailedData : IFileSpecData
    {
        public DateTime Date { get;set; }
        public int Count { get; set; }
        public int DayNumber { get; set; }


        public string RoleGuid { get; set; }
        public string RoleDescription { get; set; }
        public string AuthenticatedUserName { get; set; }
        public int TotalAccesses { get; set; }

        public DateTime InsertDate { get;set; }
        public DateTime LastAccess { get;set; }

        public string IpAddress { get; set; }
        public string ClientName { get; set; }
        public string TenantId { get; set; }
        public string SourceFile { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return
                $"{Date} {Count} {DayNumber} {RoleGuid} {RoleDescription} {AuthenticatedUserName} {TotalAccesses} {InsertDate} {LastAccess} {IpAddress} {ClientName} {TenantId} {SourceFile}";
        }
    }

    public class SumClientDetailed : IFileSpec
    {
        public SumClientDetailed()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<SumClientDetailedData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Date,Count,DayNumber,RoleGuid,RoleDescription,AuthenticatedUserName,TotalAccesses,InsertDate,LastAccess,IpAddress,ClientName,TenantId,SourceFile"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from SumECmd Client Detailed";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "12cb4452-81ef-2723-b312-3b234e1a9952";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<SumClientDetailedData>(filename, DataList, TaggedLines, map =>
            {
                map.Map(m => m.Date).TypeConverterOption.DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);
                map.Map(m => m.InsertDate).TypeConverterOption.DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);
                map.Map(m => m.LastAccess).TypeConverterOption.DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);
            });
        }
    }

    public class SumDnsInfoData : IFileSpecData
    {
        public string HostName { get; set; }
        public string Address { get; set; }
        public DateTime LastSeen { get; set; }
        public string SourceFile{ get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return
                $"{HostName} {Address} {LastSeen} {SourceFile}";
        }
    }

    public class SumDnsInfo : IFileSpec
    {
        public SumDnsInfo()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<SumDnsInfoData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "HostName,Address,LastSeen,SourceFile"
            };
        }

        public string Author => "Andrew Rathbun";
        public string FileDescription => "CSV generated from SumECmd DNS Info";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "12cb4452-81ef-2723-b313-3b234e1a9679";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<SumDnsInfoData>(filename, DataList, TaggedLines, map => map.Map(m => m.LastSeen).TypeConverterOption.DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal));
        }
    }

    public class LogFileData : IFileSpecData
    {
        public long Lsn { get; set; }
        public long? TransactionId { get; set; }
        public long? TransactionFirstLsn { get; set; }
        public string RedoOperation { get; set; }
        public string UndoOperation { get; set; }
        public long? TargetEntryNumber { get; set; }
        public int? TargetSequenceNumber { get; set; }
        public string TargetAttributeType { get; set; }
        public string TargetAttributeName { get; set; }
        public string TargetPath { get; set; }
        public long? EntryNumber { get; set; }
        public int? SequenceNumber { get; set; }
        public string FileName { get; set; }
        public long? ParentEntryNumber { get; set; }
        public int? ParentSequenceNumber { get; set; }
        public string ParentPath { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? ContentModifiedOn { get; set; }
        public DateTime? RecordModifiedOn { get; set; }
        public DateTime? LastAccessedOn { get; set; }
        public long? FileSize { get; set; }
        public string FileAttributes { get; set; }
        public string AttributeType { get; set; }
        public string Details { get; set; }
        public long? PreviousLsn { get; set; }
        public long? UndoNextLsn { get; set; }
        public string RecordType { get; set; }
        public int? TargetAttribute { get; set; }
        public bool? TargetInferred { get; set; }
        public long? TargetVcn { get; set; }
        public string Lcns { get; set; }
        public int? RecordOffset { get; set; }
        public int? AttributeOffset { get; set; }
        public int? ClusterBlockOffset { get; set; }
        public int? RedoLength { get; set; }
        public int? UndoLength { get; set; }
        public bool? Truncated { get; set; }
        public long? FileOffset { get; set; }
        public string SourceFile { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{Lsn} {TransactionId} {TransactionFirstLsn} {RedoOperation} {UndoOperation} {TargetEntryNumber} {TargetSequenceNumber} {TargetAttributeType} {TargetAttributeName} {TargetPath} {EntryNumber} {SequenceNumber} {FileName} {ParentEntryNumber} {ParentSequenceNumber} {ParentPath} {CreatedOn} {ContentModifiedOn} {RecordModifiedOn} {LastAccessedOn} {FileSize} {FileAttributes} {AttributeType} {Details} {PreviousLsn} {UndoNextLsn} {RecordType} {TargetAttribute} {TargetInferred} {TargetVcn} {Lcns} {RecordOffset} {AttributeOffset} {ClusterBlockOffset} {RedoLength} {UndoLength} {Truncated} {FileOffset} {SourceFile}";
        }
    }

    public class LogFile : IFileSpec
    {
        public LogFile()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<LogFileData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Lsn,TransactionId,TransactionFirstLsn,RedoOperation,UndoOperation,TargetEntryNumber,TargetSequenceNumber,TargetAttributeType,TargetAttributeName,TargetPath,EntryNumber,SequenceNumber,FileName,ParentEntryNumber,ParentSequenceNumber,ParentPath,CreatedOn,ContentModifiedOn,RecordModifiedOn,LastAccessedOn,FileSize,FileAttributes,AttributeType,Details,PreviousLsn,UndoNextLsn,RecordType,TargetAttribute,TargetInferred,TargetVcn,Lcns,RecordOffset,AttributeOffset,ClusterBlockOffset,RedoLength,UndoLength,Truncated,FileOffset,SourceFile"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from MFTECmd for $LogFile";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "c4704fdc-0b19-4b10-a790-47bfe77e2906";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<LogFileData>(filename, DataList, TaggedLines);
        }
    }

    public class SrumProcessorTimeData : IFileSpecData
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
        public long ProcessorTime { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{Id} {Timestamp} {ExeInfo} {ExeInfoDescription} {ExeTimestamp} {SidType} {Sid} {UserName} {UserId} {AppId} {ProcessorTime}";
        }
    }

    public class SrumProcessorTime : IFileSpec
    {
        public SrumProcessorTime()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<SrumProcessorTimeData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Id,Timestamp,ExeInfo,ExeInfoDescription,ExeTimestamp,SidType,Sid,UserName,UserId,AppId,ProcessorTime"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from SrumECmd for ProcessorTime (Server 2022)";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "6aa715f1-e7c5-4555-a238-3b481f0893c0";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<SrumProcessorTimeData>(filename, DataList, TaggedLines);
        }
    }

    public class SrumSizeInBytesData : IFileSpecData
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
        public long SizeInBytes { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{Id} {Timestamp} {ExeInfo} {ExeInfoDescription} {ExeTimestamp} {SidType} {Sid} {UserName} {UserId} {AppId} {SizeInBytes}";
        }
    }

    public class SrumSizeInBytes : IFileSpec
    {
        public SrumSizeInBytes()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<SrumSizeInBytesData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Id,Timestamp,ExeInfo,ExeInfoDescription,ExeTimestamp,SidType,Sid,UserName,UserId,AppId,SizeInBytes"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from SrumECmd for SizeInBytes (Server 2022)";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "271eba23-64e5-4d1f-8db3-3702ea55b8e1";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<SrumSizeInBytesData>(filename, DataList, TaggedLines);
        }
    }

    public class SrumTotalUsedData : IFileSpecData
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
        public long Total { get; set; }
        public long Used { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{Id} {Timestamp} {ExeInfo} {ExeInfoDescription} {ExeTimestamp} {SidType} {Sid} {UserName} {UserId} {AppId} {Total} {Used}";
        }
    }

    public class SrumTotalUsed : IFileSpec
    {
        public SrumTotalUsed()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<SrumTotalUsedData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Id,Timestamp,ExeInfo,ExeInfoDescription,ExeTimestamp,SidType,Sid,UserName,UserId,AppId,Total,Used"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from SrumECmd for TotalUsed (Server 2022)";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "b6da69fa-9815-4cff-93ed-4f6611ab8b7d";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<SrumTotalUsedData>(filename, DataList, TaggedLines);
        }
    }

    public class SrumNetworkBytesData : IFileSpecData
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
        public long BytesInBound { get; set; }
        public long BytesOutBound { get; set; }
        public long BytesTotal { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{Id} {Timestamp} {ExeInfo} {ExeInfoDescription} {ExeTimestamp} {SidType} {Sid} {UserName} {UserId} {AppId} {BytesInBound} {BytesOutBound} {BytesTotal}";
        }
    }

    public class SrumNetworkBytes : IFileSpec
    {
        public SrumNetworkBytes()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<SrumNetworkBytesData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Id,Timestamp,ExeInfo,ExeInfoDescription,ExeTimestamp,SidType,Sid,UserName,UserId,AppId,BytesInBound,BytesOutBound,BytesTotal"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from SrumECmd for NetworkBytes (Server 2022)";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "bca5a4e8-6459-4ad7-8277-ee299b5d1d44";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<SrumNetworkBytesData>(filename, DataList, TaggedLines);
        }
    }

    public class SumVmInfoData : IFileSpecData
    {
        public string VmGuid { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime LastSeenActive { get; set; }
        public string BiosGuid { get; set; }
        public string SerialNumber { get; set; }
        public string SourceFile { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{VmGuid} {CreationTime} {LastSeenActive} {BiosGuid} {SerialNumber} {SourceFile}";
        }
    }

    public class SumVmInfo : IFileSpec
    {
        public SumVmInfo()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<SumVmInfoData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "VmGuid,CreationTime,LastSeenActive,BiosGuid,SerialNumber,SourceFile"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from SumECmd VM Info";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "346bf2d1-21f6-48fa-841b-98a5e964094c";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<SumVmInfoData>(filename, DataList, TaggedLines);
        }
    }

    public class AmcacheParserDeviceCensusData : IFileSpecData
    {
        public string ItemKey { get; set; }
        public string Data { get; set; }
        public DateTime? LastModified { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{ItemKey} {Data} {LastModified}";
        }
    }

    /// <summary>
    ///     AmcacheParser's DeviceCensus CSV: the database table as it is (the columns are those of the Windows 11 26H2
    ///     Inventory databases; another schema is shown as a generic CSV)
    /// </summary>
    public class AmcacheParserDeviceCensus : IFileSpec
    {
        public AmcacheParserDeviceCensus()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<AmcacheParserDeviceCensusData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "ItemKey,Data,LastModified"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from AmcacheParser for the DeviceCensus database table";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "07594958-889d-4cd7-8e45-3e88fd5d1d9c";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<AmcacheParserDeviceCensusData>(filename, DataList, TaggedLines);
        }
    }

    public class AmcacheParserInventoryAcpiPhatHealthRecordData : IFileSpecData
    {
        public string ItemKey { get; set; }
        public string AmHealthy { get; set; }
        public string DevicePathType { get; set; }
        public string DevicePathSubtype { get; set; }
        public DateTime? LastModified { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{ItemKey} {AmHealthy} {DevicePathType} {DevicePathSubtype} {LastModified}";
        }
    }

    /// <summary>
    ///     AmcacheParser's InventoryAcpiPhatHealthRecord CSV: the database table as it is (the columns are those of the Windows 11 26H2
    ///     Inventory databases; another schema is shown as a generic CSV)
    /// </summary>
    public class AmcacheParserInventoryAcpiPhatHealthRecord : IFileSpec
    {
        public AmcacheParserInventoryAcpiPhatHealthRecord()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<AmcacheParserInventoryAcpiPhatHealthRecordData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "ItemKey,AmHealthy,DevicePathType,DevicePathSubtype,LastModified"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from AmcacheParser for the InventoryAcpiPhatHealthRecord database table";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "fd3f395c-ca7f-4f98-b933-00c101dd70e7";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<AmcacheParserInventoryAcpiPhatHealthRecordData>(filename, DataList, TaggedLines);
        }
    }

    public class AmcacheParserInventoryAcpiPhatVersionElementData : IFileSpecData
    {
        public string ItemKey { get; set; }
        public string VersionValue { get; set; }
        public string ProducerId { get; set; }
        public DateTime? LastModified { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{ItemKey} {VersionValue} {ProducerId} {LastModified}";
        }
    }

    /// <summary>
    ///     AmcacheParser's InventoryAcpiPhatVersionElement CSV: the database table as it is (the columns are those of the Windows 11 26H2
    ///     Inventory databases; another schema is shown as a generic CSV)
    /// </summary>
    public class AmcacheParserInventoryAcpiPhatVersionElement : IFileSpec
    {
        public AmcacheParserInventoryAcpiPhatVersionElement()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<AmcacheParserInventoryAcpiPhatVersionElementData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "ItemKey,VersionValue,ProducerId,LastModified"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from AmcacheParser for the InventoryAcpiPhatVersionElement database table";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "560a7ab8-74fb-42cb-a6cb-1904299aa9d9";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<AmcacheParserInventoryAcpiPhatVersionElementData>(filename, DataList, TaggedLines);
        }
    }

    public class AmcacheParserInventoryApplicationAppVData : IFileSpecData
    {
        public string ItemKey { get; set; }
        public string ProgramIds { get; set; }
        public DateTime? LastModified { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{ItemKey} {ProgramIds} {LastModified}";
        }
    }

    /// <summary>
    ///     AmcacheParser's InventoryApplicationAppV CSV: the database table as it is (the columns are those of the Windows 11 26H2
    ///     Inventory databases; another schema is shown as a generic CSV)
    /// </summary>
    public class AmcacheParserInventoryApplicationAppV : IFileSpec
    {
        public AmcacheParserInventoryApplicationAppV()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<AmcacheParserInventoryApplicationAppVData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "ItemKey,ProgramIds,LastModified"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from AmcacheParser for the InventoryApplicationAppV database table";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "03ccc1b0-bb72-41fd-a417-ae5aaaf75bb9";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<AmcacheParserInventoryApplicationAppVData>(filename, DataList, TaggedLines);
        }
    }

    public class AmcacheParserInventoryApplicationFrameworkData : IFileSpecData
    {
        public string ItemKey { get; set; }
        public string Frameworks { get; set; }
        public DateTime? LastModified { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{ItemKey} {Frameworks} {LastModified}";
        }
    }

    /// <summary>
    ///     AmcacheParser's InventoryApplicationFramework CSV: the database table as it is (the columns are those of the Windows 11 26H2
    ///     Inventory databases; another schema is shown as a generic CSV)
    /// </summary>
    public class AmcacheParserInventoryApplicationFramework : IFileSpec
    {
        public AmcacheParserInventoryApplicationFramework()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<AmcacheParserInventoryApplicationFrameworkData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "ItemKey,Frameworks,LastModified"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from AmcacheParser for the InventoryApplicationFramework database table";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "032ccb81-0b35-400f-b3c4-bdc2eac435e6";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<AmcacheParserInventoryApplicationFrameworkData>(filename, DataList, TaggedLines);
        }
    }

    public class AmcacheParserInventoryDeviceInterfaceData : IFileSpecData
    {
        public string ItemKey { get; set; }
        public string Accelerometer3D { get; set; }
        public string ActivityDetection { get; set; }
        public string AmbientLight { get; set; }
        public string Barometer { get; set; }
        public string Custom { get; set; }
        public string FloorElevation { get; set; }
        public string GeomagneticOrientation { get; set; }
        public string GravityVector { get; set; }
        public string Gyrometer3D { get; set; }
        public string Humidity { get; set; }
        public string LinearAccelerometer { get; set; }
        public string Magnetometer3D { get; set; }
        public string Orientation { get; set; }
        public string Pedometer { get; set; }
        public string Proximity { get; set; }
        public string RelativeOrientation { get; set; }
        public string SimpleDeviceOrientation { get; set; }
        public string Temperature { get; set; }
        public string EnergyMeter { get; set; }
        public string HingeAngle { get; set; }
        public string PresenceCapabilities { get; set; }
        public DateTime? LastModified { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{ItemKey} {Accelerometer3D} {ActivityDetection} {AmbientLight} {Barometer} {Custom} {FloorElevation} {GeomagneticOrientation} {GravityVector} {Gyrometer3D} {Humidity} {LinearAccelerometer} {Magnetometer3D} {Orientation} {Pedometer} {Proximity} {RelativeOrientation} {SimpleDeviceOrientation} {Temperature} {EnergyMeter} {HingeAngle} {PresenceCapabilities} {LastModified}";
        }
    }

    /// <summary>
    ///     AmcacheParser's InventoryDeviceInterface CSV: the database table as it is (the columns are those of the Windows 11 26H2
    ///     Inventory databases; another schema is shown as a generic CSV)
    /// </summary>
    public class AmcacheParserInventoryDeviceInterface : IFileSpec
    {
        public AmcacheParserInventoryDeviceInterface()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<AmcacheParserInventoryDeviceInterfaceData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "ItemKey,Accelerometer3D,ActivityDetection,AmbientLight,Barometer,Custom,FloorElevation,GeomagneticOrientation,GravityVector,Gyrometer3D,Humidity,LinearAccelerometer,Magnetometer3D,Orientation,Pedometer,Proximity,RelativeOrientation,SimpleDeviceOrientation,Temperature,EnergyMeter,HingeAngle,PresenceCapabilities,LastModified"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from AmcacheParser for the InventoryDeviceInterface database table";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "dd14c3c5-e05d-4c99-b3be-4c3642be4b64";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<AmcacheParserInventoryDeviceInterfaceData>(filename, DataList, TaggedLines);
        }
    }

    public class AmcacheParserInventoryDeviceMediaClassData : IFileSpecData
    {
        public string ItemKey { get; set; }
        public string Audio_RenderDriver { get; set; }
        public string Audio_CaptureDriver { get; set; }
        public DateTime? LastModified { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{ItemKey} {Audio_RenderDriver} {Audio_CaptureDriver} {LastModified}";
        }
    }

    /// <summary>
    ///     AmcacheParser's InventoryDeviceMediaClass CSV: the database table as it is (the columns are those of the Windows 11 26H2
    ///     Inventory databases; another schema is shown as a generic CSV)
    /// </summary>
    public class AmcacheParserInventoryDeviceMediaClass : IFileSpec
    {
        public AmcacheParserInventoryDeviceMediaClass()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<AmcacheParserInventoryDeviceMediaClassData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "ItemKey,Audio_RenderDriver,Audio_CaptureDriver,LastModified"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from AmcacheParser for the InventoryDeviceMediaClass database table";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "0de76279-40a9-4804-8d94-e886de4ef326";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<AmcacheParserInventoryDeviceMediaClassData>(filename, DataList, TaggedLines);
        }
    }

    public class AmcacheParserInventoryDevicePciData : IFileSpecData
    {
        public string ItemKey { get; set; }
        public string IsTunneledDevice { get; set; }
        public DateTime? LastModified { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{ItemKey} {IsTunneledDevice} {LastModified}";
        }
    }

    /// <summary>
    ///     AmcacheParser's InventoryDevicePci CSV: the database table as it is (the columns are those of the Windows 11 26H2
    ///     Inventory databases; another schema is shown as a generic CSV)
    /// </summary>
    public class AmcacheParserInventoryDevicePci : IFileSpec
    {
        public AmcacheParserInventoryDevicePci()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<AmcacheParserInventoryDevicePciData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "ItemKey,IsTunneledDevice,LastModified"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from AmcacheParser for the InventoryDevicePci database table";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "e0d47a8f-5ada-4fad-94d7-1af03246cc07";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<AmcacheParserInventoryDevicePciData>(filename, DataList, TaggedLines);
        }
    }

    public class AmcacheParserInventoryDeviceSensorData : IFileSpecData
    {
        public string ItemKey { get; set; }
        public string Manufacturer { get; set; }
        public DateTime? LastModified { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{ItemKey} {Manufacturer} {LastModified}";
        }
    }

    /// <summary>
    ///     AmcacheParser's InventoryDeviceSensor CSV: the database table as it is (the columns are those of the Windows 11 26H2
    ///     Inventory databases; another schema is shown as a generic CSV)
    /// </summary>
    public class AmcacheParserInventoryDeviceSensor : IFileSpec
    {
        public AmcacheParserInventoryDeviceSensor()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<AmcacheParserInventoryDeviceSensorData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "ItemKey,Manufacturer,LastModified"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from AmcacheParser for the InventoryDeviceSensor database table";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "d6437cd5-80c5-4e65-8081-0dda49b58706";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<AmcacheParserInventoryDeviceSensorData>(filename, DataList, TaggedLines);
        }
    }

    public class AmcacheParserInventoryDeviceUsbHubClassData : IFileSpecData
    {
        public string ItemKey { get; set; }
        public string TotalUserConnectablePorts { get; set; }
        public string TotalUserConnectableTypeCPorts { get; set; }
        public DateTime? LastModified { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{ItemKey} {TotalUserConnectablePorts} {TotalUserConnectableTypeCPorts} {LastModified}";
        }
    }

    /// <summary>
    ///     AmcacheParser's InventoryDeviceUsbHubClass CSV: the database table as it is (the columns are those of the Windows 11 26H2
    ///     Inventory databases; another schema is shown as a generic CSV)
    /// </summary>
    public class AmcacheParserInventoryDeviceUsbHubClass : IFileSpec
    {
        public AmcacheParserInventoryDeviceUsbHubClass()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<AmcacheParserInventoryDeviceUsbHubClassData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "ItemKey,TotalUserConnectablePorts,TotalUserConnectableTypeCPorts,LastModified"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from AmcacheParser for the InventoryDeviceUsbHubClass database table";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "cfb56500-36e9-462c-a6f4-686f27199fe9";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<AmcacheParserInventoryDeviceUsbHubClassData>(filename, DataList, TaggedLines);
        }
    }

    public class AmcacheParserInventoryInstallerData : IFileSpecData
    {
        public string ItemKey { get; set; }
        public string InstallerId { get; set; }
        public string InstallerName { get; set; }
        public string InstallerVersion { get; set; }
        public string InstallerPublisher { get; set; }
        public DateTime? LastModified { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{ItemKey} {InstallerId} {InstallerName} {InstallerVersion} {InstallerPublisher} {LastModified}";
        }
    }

    /// <summary>
    ///     AmcacheParser's InventoryInstaller CSV: the database table as it is (the columns are those of the Windows 11 26H2
    ///     Inventory databases; another schema is shown as a generic CSV)
    /// </summary>
    public class AmcacheParserInventoryInstaller : IFileSpec
    {
        public AmcacheParserInventoryInstaller()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<AmcacheParserInventoryInstallerData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "ItemKey,InstallerId,InstallerName,InstallerVersion,InstallerPublisher,LastModified"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from AmcacheParser for the InventoryInstaller database table";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "4a2f68e8-1f3e-4d2b-b676-fbb1d272bc01";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<AmcacheParserInventoryInstallerData>(filename, DataList, TaggedLines);
        }
    }

    public class AmcacheParserInventoryInstallerApplicationData : IFileSpecData
    {
        public string ItemKey { get; set; }
        public string ProgramId { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public string Publisher { get; set; }
        public string Language { get; set; }
        public string UninstallKey { get; set; }
        public string InstallerId { get; set; }
        public DateTime? LastModified { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{ItemKey} {ProgramId} {Name} {Version} {Publisher} {Language} {UninstallKey} {InstallerId} {LastModified}";
        }
    }

    /// <summary>
    ///     AmcacheParser's InventoryInstallerApplication CSV: the database table as it is (the columns are those of the Windows 11 26H2
    ///     Inventory databases; another schema is shown as a generic CSV)
    /// </summary>
    public class AmcacheParserInventoryInstallerApplication : IFileSpec
    {
        public AmcacheParserInventoryInstallerApplication()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<AmcacheParserInventoryInstallerApplicationData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "ItemKey,ProgramId,Name,Version,Publisher,Language,UninstallKey,InstallerId,LastModified"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from AmcacheParser for the InventoryInstallerApplication database table";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "b9a32a28-c1d8-420d-b5fa-7d4e1f82970d";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<AmcacheParserInventoryInstallerApplicationData>(filename, DataList, TaggedLines);
        }
    }

    public class AmcacheParserInventoryInstallerApplicationFileData : IFileSpecData
    {
        public string ItemKey { get; set; }
        public string InstallerId { get; set; }
        public string FilePath { get; set; }
        public string FileId { get; set; }
        public string ProgramId { get; set; }
        public DateTime? LastModified { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{ItemKey} {InstallerId} {FilePath} {FileId} {ProgramId} {LastModified}";
        }
    }

    /// <summary>
    ///     AmcacheParser's InventoryInstallerApplicationFile CSV: the database table as it is (the columns are those of the Windows 11 26H2
    ///     Inventory databases; another schema is shown as a generic CSV)
    /// </summary>
    public class AmcacheParserInventoryInstallerApplicationFile : IFileSpec
    {
        public AmcacheParserInventoryInstallerApplicationFile()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<AmcacheParserInventoryInstallerApplicationFileData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "ItemKey,InstallerId,FilePath,FileId,ProgramId,LastModified"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from AmcacheParser for the InventoryInstallerApplicationFile database table";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "4291d9e7-5dd8-4bfb-9962-c540d73b9e6b";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<AmcacheParserInventoryInstallerApplicationFileData>(filename, DataList, TaggedLines);
        }
    }

    public class AmcacheParserInventoryMiscellaneousData : IFileSpecData
    {
        public string ItemKey { get; set; }
        public string Exists { get; set; }
        public string Value { get; set; }
        public DateTime? LastModified { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{ItemKey} {Exists} {Value} {LastModified}";
        }
    }

    /// <summary>
    ///     AmcacheParser's InventoryMiscellaneous CSV: the database table as it is (the columns are those of the Windows 11 26H2
    ///     Inventory databases; another schema is shown as a generic CSV)
    /// </summary>
    public class AmcacheParserInventoryMiscellaneous : IFileSpec
    {
        public AmcacheParserInventoryMiscellaneous()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<AmcacheParserInventoryMiscellaneousData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "ItemKey,Exists,Value,LastModified"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from AmcacheParser for the InventoryMiscellaneous database table";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "22b7d1ea-535b-45c8-88e2-835fec32eb52";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<AmcacheParserInventoryMiscellaneousData>(filename, DataList, TaggedLines);
        }
    }

    public class AmcacheParserInventoryMiscellaneousMemorySlotArrayInfoData : IFileSpecData
    {
        public string ItemKey { get; set; }
        public string Slot { get; set; }
        public string Type { get; set; }
        public string TypeDetails { get; set; }
        public string Speed { get; set; }
        public string Capacity { get; set; }
        public string Model { get; set; }
        public string Manufacturer { get; set; }
        public string TotalWidth { get; set; }
        public string DataWidth { get; set; }
        public string MemoryErrorCorrection { get; set; }
        public DateTime? LastModified { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{ItemKey} {Slot} {Type} {TypeDetails} {Speed} {Capacity} {Model} {Manufacturer} {TotalWidth} {DataWidth} {MemoryErrorCorrection} {LastModified}";
        }
    }

    /// <summary>
    ///     AmcacheParser's InventoryMiscellaneousMemorySlotArrayInfo CSV: the database table as it is (the columns are those of the Windows 11 26H2
    ///     Inventory databases; another schema is shown as a generic CSV)
    /// </summary>
    public class AmcacheParserInventoryMiscellaneousMemorySlotArrayInfo : IFileSpec
    {
        public AmcacheParserInventoryMiscellaneousMemorySlotArrayInfo()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<AmcacheParserInventoryMiscellaneousMemorySlotArrayInfoData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "ItemKey,Slot,Type,TypeDetails,Speed,Capacity,Model,Manufacturer,TotalWidth,DataWidth,MemoryErrorCorrection,LastModified"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from AmcacheParser for the InventoryMiscellaneousMemorySlotArrayInfo database table";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "63342ace-0207-42d3-a975-21c82e03d4e5";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<AmcacheParserInventoryMiscellaneousMemorySlotArrayInfoData>(filename, DataList, TaggedLines);
        }
    }

    public class AmcacheParserInventoryMiscellaneousUUPInfoData : IFileSpecData
    {
        public string ItemKey { get; set; }
        public string Identifier { get; set; }
        public string Version { get; set; }
        public string Source { get; set; }
        public string PreviousVersion { get; set; }
        public string LastActivatedVersion { get; set; }
        public DateTime? LastModified { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{ItemKey} {Identifier} {Version} {Source} {PreviousVersion} {LastActivatedVersion} {LastModified}";
        }
    }

    /// <summary>
    ///     AmcacheParser's InventoryMiscellaneousUUPInfo CSV: the database table as it is (the columns are those of the Windows 11 26H2
    ///     Inventory databases; another schema is shown as a generic CSV)
    /// </summary>
    public class AmcacheParserInventoryMiscellaneousUUPInfo : IFileSpec
    {
        public AmcacheParserInventoryMiscellaneousUUPInfo()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<AmcacheParserInventoryMiscellaneousUUPInfoData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "ItemKey,Identifier,Version,Source,PreviousVersion,LastActivatedVersion,LastModified"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from AmcacheParser for the InventoryMiscellaneousUUPInfo database table";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "c83436f4-86bb-454e-bccb-209d8f7a835f";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<AmcacheParserInventoryMiscellaneousUUPInfoData>(filename, DataList, TaggedLines);
        }
    }

    public class AmcacheParserInventoryMiscellaneousUserData : IFileSpecData
    {
        public string ItemKey { get; set; }
        public string OriginalName { get; set; }
        public string Exists { get; set; }
        public string Value { get; set; }
        public string UserId { get; set; }
        public string StandardUserHash { get; set; }
        public DateTime? LastModified { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{ItemKey} {OriginalName} {Exists} {Value} {UserId} {StandardUserHash} {LastModified}";
        }
    }

    /// <summary>
    ///     AmcacheParser's InventoryMiscellaneousUser CSV: the database table as it is (the columns are those of the Windows 11 26H2
    ///     Inventory databases; another schema is shown as a generic CSV)
    /// </summary>
    public class AmcacheParserInventoryMiscellaneousUser : IFileSpec
    {
        public AmcacheParserInventoryMiscellaneousUser()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<AmcacheParserInventoryMiscellaneousUserData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "ItemKey,OriginalName,Exists,Value,UserId,StandardUserHash,LastModified"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from AmcacheParser for the InventoryMiscellaneousUser database table";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "b458caf0-2433-4a69-a438-1797c39447b1";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<AmcacheParserInventoryMiscellaneousUserData>(filename, DataList, TaggedLines);
        }
    }

    public class AmcacheParserMareData : IFileSpecData
    {
        public string ItemKey { get; set; }
        public string Path { get; set; }
        public string ProgramId { get; set; }
        public string Flags { get; set; }
        public string Far { get; set; }
        public string Restore { get; set; }
        public string Sdbentryguid { get; set; }
        public DateTime? LastModified { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{ItemKey} {Path} {ProgramId} {Flags} {Far} {Restore} {Sdbentryguid} {LastModified}";
        }
    }

    /// <summary>
    ///     AmcacheParser's Mare CSV: the database table as it is (the columns are those of the Windows 11 26H2
    ///     Inventory databases; another schema is shown as a generic CSV)
    /// </summary>
    public class AmcacheParserMare : IFileSpec
    {
        public AmcacheParserMare()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<AmcacheParserMareData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "ItemKey,path,programId,flags,far,restore,sdbentryguid,LastModified"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from AmcacheParser for the Mare database table";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "4b3f0a76-ad3d-440d-b645-de0d4ecf8c4a";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<AmcacheParserMareData>(filename, DataList, TaggedLines);
        }
    }

    public class AmcacheParserMareBackupAppsData : IFileSpecData
    {
        public string ItemKey { get; set; }
        public string Hash { get; set; }
        public string Aumids { get; set; }
        public string SidState { get; set; }
        public DateTime? LastModified { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{ItemKey} {Hash} {Aumids} {SidState} {LastModified}";
        }
    }

    /// <summary>
    ///     AmcacheParser's MareBackupApps CSV: the database table as it is (the columns are those of the Windows 11 26H2
    ///     Inventory databases; another schema is shown as a generic CSV)
    /// </summary>
    public class AmcacheParserMareBackupApps : IFileSpec
    {
        public AmcacheParserMareBackupApps()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<AmcacheParserMareBackupAppsData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "ItemKey,Hash,Aumids,SidState,LastModified"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from AmcacheParser for the MareBackupApps database table";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "6121a9ed-ab76-4e8a-8ee0-7338dd61328d";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<AmcacheParserMareBackupAppsData>(filename, DataList, TaggedLines);
        }
    }

    public class AmcacheParserMetadataData : IFileSpecData
    {
        public string Database { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{Database} {Name} {Value}";
        }
    }

    /// <summary>
    ///     AmcacheParser's Metadata CSV: the database table as it is (the columns are those of the Windows 11 26H2
    ///     Inventory databases; another schema is shown as a generic CSV)
    /// </summary>
    public class AmcacheParserMetadata : IFileSpec
    {
        public AmcacheParserMetadata()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<AmcacheParserMetadataData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Database,Name,Value"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from AmcacheParser for the Metadata database table";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "21f6ae62-974a-4f7f-af6c-87802d582411";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<AmcacheParserMetadataData>(filename, DataList, TaggedLines);
        }
    }

    public class RecmdAdobeData : IFileSpecData
    {
        public string ProductName { get; set; }
        public string BatchKeyPath { get; set; }
        public string ProductVersion { get; set; }
        public string BatchValueName { get; set; }
        public string FullPath { get; set; }
        public DateTime? LastOpened { get; set; }
        public string FileName { get; set; }
        public long? FileSize { get; set; }
        public string FileSource { get; set; }
        public long? PageCount { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{ProductName} {BatchKeyPath} {ProductVersion} {BatchValueName} {FullPath} {LastOpened} {FileName} {FileSize} {FileSource} {PageCount}";
        }
    }

    /// <summary>
    ///     RECmd's CSV for Adobe program information (RegistryPlugin.Adobe.dll)
    /// </summary>
    public class RecmdAdobe : IFileSpec
    {
        public RecmdAdobe()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<RecmdAdobeData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "ProductName,BatchKeyPath,ProductVersion,BatchValueName,FullPath,LastOpened,FileName,FileSize,FileSource,PageCount"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from RECmd for the Adobe registry plugin";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "e9bc8029-e59a-45a8-9352-dc89eacdd8d8";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<RecmdAdobeData>(filename, DataList, TaggedLines, map =>
            {
                map.Map(t => t.LastOpened).TypeConverter<LenientDateTimeConverter>();
            });
        }
    }

    public class RecmdAdobeTrustManagerData : IFileSpecData
    {
        public string Website { get; set; }
        public string BatchKeyPath { get; set; }
        public string UserChoice { get; set; }
        public string BatchValueName { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{Website} {BatchKeyPath} {UserChoice} {BatchValueName}";
        }
    }

    /// <summary>
    ///     RECmd's CSV for Adobe User Accessed Websites (RegistryPlugin.Adobe.TrustManger-Websites.dll)
    /// </summary>
    public class RecmdAdobeTrustManager : IFileSpec
    {
        public RecmdAdobeTrustManager()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<RecmdAdobeTrustManagerData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Website,BatchKeyPath,UserChoice,BatchValueName"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from RECmd for the AdobeTrustManager registry plugin";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "2abb96f1-bfde-4ad3-9ec1-433a36d4d65f";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<RecmdAdobeTrustManagerData>(filename, DataList, TaggedLines);
        }
    }

    public class RecmdAppCompatData : IFileSpecData
    {
        public long? CacheEntryPosition { get; set; }
        public string BatchKeyPath { get; set; }
        public string ProgramName { get; set; }
        public string BatchValueName { get; set; }
        public DateTime? ModifiedTime { get; set; }
        public string Executed { get; set; }
        public bool? FromSlack { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{CacheEntryPosition} {BatchKeyPath} {ProgramName} {BatchValueName} {ModifiedTime} {Executed} {FromSlack}";
        }
    }

    /// <summary>
    ///     RECmd's CSV for AppCompatCache (RegistryPlugin.AppCompatCache.dll)
    /// </summary>
    public class RecmdAppCompat : IFileSpec
    {
        public RecmdAppCompat()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<RecmdAppCompatData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "CacheEntryPosition,BatchKeyPath,ProgramName,BatchValueName,ModifiedTime,Executed,FromSlack"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from RECmd for the AppCompat registry plugin";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "7710a603-3afe-4402-8245-fd917ff02d6b";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<RecmdAppCompatData>(filename, DataList, TaggedLines, map =>
            {
                map.Map(t => t.ModifiedTime).TypeConverter<LenientDateTimeConverter>();
            });
        }
    }

    public class RecmdAppCompatFlagsData : IFileSpecData
    {
        public string Executable { get; set; }
        public string BatchKeyPath { get; set; }
        public string ValueName { get; set; }
        public string BatchValueName { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{Executable} {BatchKeyPath} {ValueName} {BatchValueName}";
        }
    }

    /// <summary>
    ///     RECmd's CSV for AppCompatFlags (RegistryPlugin.AppCompatFlags.dll)
    /// </summary>
    public class RecmdAppCompatFlags : IFileSpec
    {
        public RecmdAppCompatFlags()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<RecmdAppCompatFlagsData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Executable,BatchKeyPath,ValueName,BatchValueName"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from RECmd for the AppCompatFlags registry plugin";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "fc757ccc-39ce-4e49-87bb-f5b97ffcf285";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<RecmdAppCompatFlagsData>(filename, DataList, TaggedLines);
        }
    }

    public class RecmdAppCompatFlags2WinRARData : IFileSpecData
    {
        public string Path { get; set; }
        public string BatchKeyPath { get; set; }
        public string BatchValueName { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{Path} {BatchKeyPath} {BatchValueName}";
        }
    }

    /// <summary>
    ///     RECmd's CSV for AppCompatFlags2 (RegistryPlugin.AppCompatFlags2.dll) and WinRAR (RegistryPlugin.WinRAR.dll)
    /// </summary>
    public class RecmdAppCompatFlags2WinRAR : IFileSpec
    {
        public RecmdAppCompatFlags2WinRAR()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<RecmdAppCompatFlags2WinRARData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Path,BatchKeyPath,BatchValueName"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from RECmd for the AppCompatFlags2 and WinRAR registry plugins";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "dd4ef5ac-6f1e-47c2-bdd3-d619e1b50941";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<RecmdAppCompatFlags2WinRARData>(filename, DataList, TaggedLines);
        }
    }

    public class RecmdAppPathsData : IFileSpecData
    {
        public DateTime? Timestamp { get; set; }
        public string BatchKeyPath { get; set; }
        public string FileName { get; set; }
        public string BatchValueName { get; set; }
        public string Path1 { get; set; }
        public string Path2 { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{Timestamp} {BatchKeyPath} {FileName} {BatchValueName} {Path1} {Path2}";
        }
    }

    /// <summary>
    ///     RECmd's CSV for AppPaths (RegistryPlugin.AppPaths.dll)
    /// </summary>
    public class RecmdAppPaths : IFileSpec
    {
        public RecmdAppPaths()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<RecmdAppPathsData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Timestamp,BatchKeyPath,FileName,BatchValueName,Path1,Path2"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from RECmd for the AppPaths registry plugin";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "32d9229a-63f2-4927-a9d8-9614a75202d4";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<RecmdAppPathsData>(filename, DataList, TaggedLines, map =>
            {
                map.Map(t => t.Timestamp).TypeConverter<LenientDateTimeConverter>();
            });
        }
    }

    public class RecmdApplicationSettingsContainerData : IFileSpecData
    {
        public string ValueName { get; set; }
        public string BatchKeyPath { get; set; }
        public string ValueType { get; set; }
        public string BatchValueName { get; set; }
        public string Value { get; set; }
        public string Notes { get; set; }
        public DateTime? UTCTimestamp { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{ValueName} {BatchKeyPath} {ValueType} {BatchValueName} {Value} {Notes} {UTCTimestamp}";
        }
    }

    /// <summary>
    ///     RECmd's CSV for ApplicationSettingsContainer (RegistryPlugin.ApplicationSettingsContainer.dll)
    /// </summary>
    public class RecmdApplicationSettingsContainer : IFileSpec
    {
        public RecmdApplicationSettingsContainer()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<RecmdApplicationSettingsContainerData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "ValueName,BatchKeyPath,ValueType,BatchValueName,Value,Notes,UTCTimestamp"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from RECmd for the ApplicationSettingsContainer registry plugin";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "0381eccf-d804-435f-9404-8a50cc5f161d";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<RecmdApplicationSettingsContainerData>(filename, DataList, TaggedLines, map =>
            {
                map.Map(t => t.UTCTimestamp).TypeConverter<LenientDateTimeConverter>();
            });
        }
    }

    public class RecmdAresData : IFileSpecData
    {
        public string PropertyName { get; set; }
        public string BatchKeyPath { get; set; }
        public string PropertyValue { get; set; }
        public string BatchValueName { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{PropertyName} {BatchKeyPath} {PropertyValue} {BatchValueName}";
        }
    }

    /// <summary>
    ///     RECmd's CSV for Ares P2P information (RegistryPlugin.Ares.dll)
    /// </summary>
    public class RecmdAres : IFileSpec
    {
        public RecmdAres()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<RecmdAresData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "PropertyName,BatchKeyPath,PropertyValue,BatchValueName"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from RECmd for the Ares registry plugin";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "b9bdd360-69e3-4aae-934f-9ce9f57d6eed";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<RecmdAresData>(filename, DataList, TaggedLines);
        }
    }

    public class RecmdBamDamData : IFileSpecData
    {
        public string Program { get; set; }
        public string BatchKeyPath { get; set; }
        public DateTime? ExecutionTime { get; set; }
        public string BatchValueName { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{Program} {BatchKeyPath} {ExecutionTime} {BatchValueName}";
        }
    }

    /// <summary>
    ///     RECmd's CSV for BamDam (RegistryPlugin.BamDam.dll)
    /// </summary>
    public class RecmdBamDam : IFileSpec
    {
        public RecmdBamDam()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<RecmdBamDamData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Program,BatchKeyPath,ExecutionTime,BatchValueName"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from RECmd for the BamDam registry plugin";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "f3d3df10-1474-4217-86ef-6673d1d283a9";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<RecmdBamDamData>(filename, DataList, TaggedLines, map =>
            {
                map.Map(t => t.ExecutionTime).TypeConverter<LenientDateTimeConverter>();
            });
        }
    }

    public class RecmdBluetoothBthPortData : IFileSpecData
    {
        public string Name { get; set; }
        public string BatchKeyPath { get; set; }
        public string Address { get; set; }
        public string BatchValueName { get; set; }
        public DateTime? LastSeen { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{Name} {BatchKeyPath} {Address} {BatchValueName} {LastSeen}";
        }
    }

    /// <summary>
    ///     RECmd's CSV for BluetoothBthPort (RegistryPlugin.BluetoothServicesBthPort.dll)
    /// </summary>
    public class RecmdBluetoothBthPort : IFileSpec
    {
        public RecmdBluetoothBthPort()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<RecmdBluetoothBthPortData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Name,BatchKeyPath,Address,BatchValueName,LastSeen"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from RECmd for the BluetoothBthPort registry plugin";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "4430562e-60c5-4fcd-8508-0c037e31a783";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<RecmdBluetoothBthPortData>(filename, DataList, TaggedLines, map =>
            {
                map.Map(t => t.LastSeen).TypeConverter<LenientDateTimeConverter>();
            });
        }
    }

    public class RecmdCIDSizeMRUData : IFileSpecData
    {
        public string Executable { get; set; }
        public string BatchKeyPath { get; set; }
        public long? MRUPosition { get; set; }
        public string BatchValueName { get; set; }
        public DateTime? OpenedOn { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{Executable} {BatchKeyPath} {MRUPosition} {BatchValueName} {OpenedOn}";
        }
    }

    /// <summary>
    ///     RECmd's CSV for ComDlg32 CIDSizeMRU (RegistryPlugin.CIDSizeMRU.dll)
    /// </summary>
    public class RecmdCIDSizeMRU : IFileSpec
    {
        public RecmdCIDSizeMRU()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<RecmdCIDSizeMRUData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Executable,BatchKeyPath,MRUPosition,BatchValueName,OpenedOn"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from RECmd for the CIDSizeMRU registry plugin";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "03fc1579-4c4b-44f9-8578-da8bace3d77d";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<RecmdCIDSizeMRUData>(filename, DataList, TaggedLines, map =>
            {
                map.Map(t => t.OpenedOn).TypeConverter<LenientDateTimeConverter>();
            });
        }
    }

    public class RecmdDHCPNetworkHintData : IFileSpecData
    {
        public string NetworkHint { get; set; }
        public string BatchKeyPath { get; set; }
        public string DHCPAddress { get; set; }
        public string BatchValueName { get; set; }
        public string DHCPServer { get; set; }
        public string DHCPDomain { get; set; }
        public DateTime? LeaseObtained { get; set; }
        public DateTime? LeaseExpires { get; set; }
        public string DefaultGateway { get; set; }
        public string Interface { get; set; }
        public string InterfaceSubkey { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{NetworkHint} {BatchKeyPath} {DHCPAddress} {BatchValueName} {DHCPServer} {DHCPDomain} {LeaseObtained} {LeaseExpires} {DefaultGateway} {Interface} {InterfaceSubkey}";
        }
    }

    /// <summary>
    ///     RECmd's CSV for DHCPNetworkHints (RegistryPlugin.DHCPNetworkHint.dll)
    /// </summary>
    public class RecmdDHCPNetworkHint : IFileSpec
    {
        public RecmdDHCPNetworkHint()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<RecmdDHCPNetworkHintData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "NetworkHint,BatchKeyPath,DHCPAddress,BatchValueName,DHCPServer,DHCPDomain,LeaseObtained,LeaseExpires,DefaultGateway,Interface,InterfaceSubkey"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from RECmd for the DHCPNetworkHint registry plugin";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "d6371362-7996-4416-91b5-d59f2699bcb7";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<RecmdDHCPNetworkHintData>(filename, DataList, TaggedLines, map =>
            {
                map.Map(t => t.LeaseObtained).TypeConverter<LenientDateTimeConverter>();
                map.Map(t => t.LeaseExpires).TypeConverter<LenientDateTimeConverter>();
            });
        }
    }

    public class RecmdDeviceClassesData : IFileSpecData
    {
        public DateTime? Timestamp { get; set; }
        public string BatchKeyPath { get; set; }
        public string GuidFolder { get; set; }
        public string BatchValueName { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string SerialNumber { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{Timestamp} {BatchKeyPath} {GuidFolder} {BatchValueName} {Type} {Name} {SerialNumber}";
        }
    }

    /// <summary>
    ///     RECmd's CSV for DeviceClasses (RegistryPlugin.DeviceClasses.dll)
    /// </summary>
    public class RecmdDeviceClasses : IFileSpec
    {
        public RecmdDeviceClasses()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<RecmdDeviceClassesData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Timestamp,BatchKeyPath,GuidFolder,BatchValueName,Type,Name,SerialNumber"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from RECmd for the DeviceClasses registry plugin";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "2de7f5b6-3814-4488-9026-223ccce28892";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<RecmdDeviceClassesData>(filename, DataList, TaggedLines, map =>
            {
                map.Map(t => t.Timestamp).TypeConverter<LenientDateTimeConverter>();
            });
        }
    }

    public class RecmdDsrmAdminLogonBehaviourData : IFileSpecData
    {
        public string ValueName { get; set; }
        public string BatchKeyPath { get; set; }
        public string ValueData { get; set; }
        public string BatchValueName { get; set; }
        public DateTime? LastWriteTime { get; set; }
        public bool? PersistenceDetection { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{ValueName} {BatchKeyPath} {ValueData} {BatchValueName} {LastWriteTime} {PersistenceDetection}";
        }
    }

    /// <summary>
    ///     RECmd's CSV for DsrmAdminLogonBehaviour Value (RegistryPlugin.DsrmAdminLogonBehaviour.dll)
    /// </summary>
    public class RecmdDsrmAdminLogonBehaviour : IFileSpec
    {
        public RecmdDsrmAdminLogonBehaviour()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<RecmdDsrmAdminLogonBehaviourData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "ValueName,BatchKeyPath,ValueData,BatchValueName,LastWriteTime,PersistenceDetection"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from RECmd for the DsrmAdminLogonBehaviour registry plugin";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "1f2c15c4-a1a2-4a9a-b238-39eaa4acfbfd";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<RecmdDsrmAdminLogonBehaviourData>(filename, DataList, TaggedLines, map =>
            {
                map.Map(t => t.LastWriteTime).TypeConverter<LenientDateTimeConverter>();
            });
        }
    }

    public class RecmdETWData : IFileSpecData
    {
        public string LastWriteTimestamp { get; set; }
        public string BatchKeyPath { get; set; }
        public string Guid { get; set; }
        public string BatchValueName { get; set; }
        public string Provider { get; set; }
        public string Enabled { get; set; }
        public string EnabledProperty { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{LastWriteTimestamp} {BatchKeyPath} {Guid} {BatchValueName} {Provider} {Enabled} {EnabledProperty}";
        }
    }

    /// <summary>
    ///     RECmd's CSV for ETW (RegistryPlugin.ETW.dll)
    /// </summary>
    public class RecmdETW : IFileSpec
    {
        public RecmdETW()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<RecmdETWData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "LastWriteTimestamp,BatchKeyPath,Guid,BatchValueName,Provider,Enabled,EnabledProperty"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from RECmd for the ETW registry plugin";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "9d39ef35-ac83-4f8c-b532-27958c493e30";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<RecmdETWData>(filename, DataList, TaggedLines);
        }
    }

    public class RecmdFeatureUsageData : IFileSpecData
    {
        public string KeyPath { get; set; }
        public string BatchKeyPath { get; set; }
        public string ValueName { get; set; }
        public string BatchValueName { get; set; }
        public string Count { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{KeyPath} {BatchKeyPath} {ValueName} {BatchValueName} {Count}";
        }
    }

    /// <summary>
    ///     RECmd's CSV for FeatureUsage (RegistryPlugin.FeatureUsage.dll)
    /// </summary>
    public class RecmdFeatureUsage : IFileSpec
    {
        public RecmdFeatureUsage()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<RecmdFeatureUsageData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "KeyPath,BatchKeyPath,ValueName,BatchValueName,Count"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from RECmd for the FeatureUsage registry plugin";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "3d6e23a1-f2c8-4a85-a763-d9193198822c";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<RecmdFeatureUsageData>(filename, DataList, TaggedLines);
        }
    }

    public class RecmdFileExtsData : IFileSpecData
    {
        public string Extension { get; set; }
        public string BatchKeyPath { get; set; }
        public string OpensWithExecutables { get; set; }
        public string BatchValueName { get; set; }
        public string OpensWithProgramIDs { get; set; }
        public string UserChoice { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{Extension} {BatchKeyPath} {OpensWithExecutables} {BatchValueName} {OpensWithProgramIDs} {UserChoice}";
        }
    }

    /// <summary>
    ///     RECmd's CSV for File Extensions (RegistryPlugin.FileExts.dll)
    /// </summary>
    public class RecmdFileExts : IFileSpec
    {
        public RecmdFileExts()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<RecmdFileExtsData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Extension,BatchKeyPath,OpensWithExecutables,BatchValueName,OpensWithProgramIDs,UserChoice"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from RECmd for the FileExts registry plugin";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "82e5e16d-8b28-4c83-8023-5c3d296c2c8a";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<RecmdFileExtsData>(filename, DataList, TaggedLines);
        }
    }

    public class RecmdFirewallRulesData : IFileSpecData
    {
        public string Action { get; set; }
        public string BatchKeyPath { get; set; }
        public string Active { get; set; }
        public string BatchValueName { get; set; }
        public string Dir { get; set; }
        public string Protocol { get; set; }
        public string LPort { get; set; }
        public string RPort { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }
        public string App { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{Action} {BatchKeyPath} {Active} {BatchValueName} {Dir} {Protocol} {LPort} {RPort} {Name} {Desc} {App}";
        }
    }

    /// <summary>
    ///     RECmd's CSV for FirewallRules (RegistryPlugin.FirewallRules.dll)
    /// </summary>
    public class RecmdFirewallRules : IFileSpec
    {
        public RecmdFirewallRules()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<RecmdFirewallRulesData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Action,BatchKeyPath,Active,BatchValueName,Dir,Protocol,LPort,RPort,Name,Desc,App"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from RECmd for the FirewallRules registry plugin";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "6a54e24d-e14b-464a-b29d-4eb3ad3c56c9";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<RecmdFirewallRulesData>(filename, DataList, TaggedLines);
        }
    }

    public class RecmdFirstFolderData : IFileSpecData
    {
        public string Executable { get; set; }
        public string BatchKeyPath { get; set; }
        public string FolderName { get; set; }
        public string BatchValueName { get; set; }
        public long? MRUPosition { get; set; }
        public DateTime? OpenedOn { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{Executable} {BatchKeyPath} {FolderName} {BatchValueName} {MRUPosition} {OpenedOn}";
        }
    }

    /// <summary>
    ///     RECmd's CSV for First folder (RegistryPlugin.FirstFolder.dll)
    /// </summary>
    public class RecmdFirstFolder : IFileSpec
    {
        public RecmdFirstFolder()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<RecmdFirstFolderData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Executable,BatchKeyPath,FolderName,BatchValueName,MRUPosition,OpenedOn"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from RECmd for the FirstFolder registry plugin";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "bac57f91-5456-4e41-bd1a-1b00cdbfa6e0";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<RecmdFirstFolderData>(filename, DataList, TaggedLines, map =>
            {
                map.Map(t => t.OpenedOn).TypeConverter<LenientDateTimeConverter>();
            });
        }
    }

    public class RecmdIconLayoutsData : IFileSpecData
    {
        public string Name { get; set; }
        public string BatchKeyPath { get; set; }
        public string BatchValueName { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{Name} {BatchKeyPath} {BatchValueName}";
        }
    }

    /// <summary>
    ///     RECmd's CSV for IconLayouts (RegistryPlugin.IconLayouts.dll)
    /// </summary>
    public class RecmdIconLayouts : IFileSpec
    {
        public RecmdIconLayouts()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<RecmdIconLayoutsData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Name,BatchKeyPath,BatchValueName"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from RECmd for the IconLayouts registry plugin";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "41a951dc-4733-481b-bb77-4ef5857e313a";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<RecmdIconLayoutsData>(filename, DataList, TaggedLines);
        }
    }

    public class RecmdInventoryApplicationData : IFileSpecData
    {
        public DateTime? Timestamp { get; set; }
        public string BatchKeyPath { get; set; }
        public string Name { get; set; }
        public string BatchValueName { get; set; }
        public string Version { get; set; }
        public string Publisher { get; set; }
        public string Source { get; set; }
        public string RootDirPath { get; set; }
        public string UninstallString { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{Timestamp} {BatchKeyPath} {Name} {BatchValueName} {Version} {Publisher} {Source} {RootDirPath} {UninstallString}";
        }
    }

    /// <summary>
    ///     RECmd's CSV for Amcache-InventoryApplication (RegistryPlugin.Amcache-InventoryApplication.dll)
    /// </summary>
    public class RecmdInventoryApplication : IFileSpec
    {
        public RecmdInventoryApplication()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<RecmdInventoryApplicationData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Timestamp,BatchKeyPath,Name,BatchValueName,Version,Publisher,Source,RootDirPath,UninstallString"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from RECmd for the InventoryApplication registry plugin";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "c6e8f79f-1179-432c-b0ad-92dc55a0eebf";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<RecmdInventoryApplicationData>(filename, DataList, TaggedLines, map =>
            {
                map.Map(t => t.Timestamp).TypeConverter<LenientDateTimeConverter>();
            });
        }
    }

    public class RecmdInventoryApplicationFileData : IFileSpecData
    {
        public DateTime? Timestamp { get; set; }
        public string BatchKeyPath { get; set; }
        public string Path { get; set; }
        public string BatchValueName { get; set; }
        public string Name { get; set; }
        public string ProductName { get; set; }
        public string Publisher { get; set; }
        public string Version { get; set; }
        public string SHA1 { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{Timestamp} {BatchKeyPath} {Path} {BatchValueName} {Name} {ProductName} {Publisher} {Version} {SHA1}";
        }
    }

    /// <summary>
    ///     RECmd's CSV for Amcache-InventoryApplicationFile (RegistryPlugin.Amcache-InventoryApplicationFile.dll)
    /// </summary>
    public class RecmdInventoryApplicationFile : IFileSpec
    {
        public RecmdInventoryApplicationFile()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<RecmdInventoryApplicationFileData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Timestamp,BatchKeyPath,Path,BatchValueName,Name,ProductName,Publisher,Version,SHA1"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from RECmd for the InventoryApplicationFile registry plugin";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "cd2bad0f-8662-42ba-8ba0-58333f5cd638";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<RecmdInventoryApplicationFileData>(filename, DataList, TaggedLines, map =>
            {
                map.Map(t => t.Timestamp).TypeConverter<LenientDateTimeConverter>();
            });
        }
    }

    public class RecmdInventoryApplicationShortcutData : IFileSpecData
    {
        public DateTime? Timestamp { get; set; }
        public string BatchKeyPath { get; set; }
        public string Path { get; set; }
        public string BatchValueName { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{Timestamp} {BatchKeyPath} {Path} {BatchValueName}";
        }
    }

    /// <summary>
    ///     RECmd's CSV for Amcache-InventoryApplicationShortcut (RegistryPlugin.Amcache-InventoryApplicationShortcut.dll)
    /// </summary>
    public class RecmdInventoryApplicationShortcut : IFileSpec
    {
        public RecmdInventoryApplicationShortcut()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<RecmdInventoryApplicationShortcutData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Timestamp,BatchKeyPath,Path,BatchValueName"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from RECmd for the InventoryApplicationShortcut registry plugin";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "f2108427-dc33-4e73-948a-71123b58f05b";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<RecmdInventoryApplicationShortcutData>(filename, DataList, TaggedLines, map =>
            {
                map.Map(t => t.Timestamp).TypeConverter<LenientDateTimeConverter>();
            });
        }
    }

    public class RecmdInventoryDeviceContainerData : IFileSpecData
    {
        public DateTime? Timestamp { get; set; }
        public string BatchKeyPath { get; set; }
        public string ModelName { get; set; }
        public string BatchValueName { get; set; }
        public string FriendlyName { get; set; }
        public string ModelNumber { get; set; }
        public string Manufacturer { get; set; }
        public string PrimaryCategory { get; set; }
        public string ModelID { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{Timestamp} {BatchKeyPath} {ModelName} {BatchValueName} {FriendlyName} {ModelNumber} {Manufacturer} {PrimaryCategory} {ModelID}";
        }
    }

    /// <summary>
    ///     RECmd's CSV for Amcache-InventoryDeviceContainer (RegistryPlugin.Amcache-InventoryDeviceContainer.dll)
    /// </summary>
    public class RecmdInventoryDeviceContainer : IFileSpec
    {
        public RecmdInventoryDeviceContainer()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<RecmdInventoryDeviceContainerData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Timestamp,BatchKeyPath,ModelName,BatchValueName,FriendlyName,ModelNumber,Manufacturer,PrimaryCategory,ModelID"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from RECmd for the InventoryDeviceContainer registry plugin";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "3f7a42d2-a332-4413-81c8-4454fffb172f";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<RecmdInventoryDeviceContainerData>(filename, DataList, TaggedLines, map =>
            {
                map.Map(t => t.Timestamp).TypeConverter<LenientDateTimeConverter>();
            });
        }
    }

    public class RecmdInventoryDevicePnpData : IFileSpecData
    {
        public DateTime? Timestamp { get; set; }
        public string BatchKeyPath { get; set; }
        public string Model { get; set; }
        public string BatchValueName { get; set; }
        public string Manufacturer { get; set; }
        public string Description { get; set; }
        public string InstallDate { get; set; }
        public string ParentID { get; set; }
        public string MatchingID { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{Timestamp} {BatchKeyPath} {Model} {BatchValueName} {Manufacturer} {Description} {InstallDate} {ParentID} {MatchingID}";
        }
    }

    /// <summary>
    ///     RECmd's CSV for Amcache-InventoryDevicePnp (RegistryPlugin.Amcache-InventoryDevicePnp.dll)
    /// </summary>
    public class RecmdInventoryDevicePnp : IFileSpec
    {
        public RecmdInventoryDevicePnp()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<RecmdInventoryDevicePnpData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Timestamp,BatchKeyPath,Model,BatchValueName,Manufacturer,Description,InstallDate,ParentID,MatchingID"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from RECmd for the InventoryDevicePnp registry plugin";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "5311f216-9785-4df4-bf4c-af99afa576a2";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<RecmdInventoryDevicePnpData>(filename, DataList, TaggedLines, map =>
            {
                map.Map(t => t.Timestamp).TypeConverter<LenientDateTimeConverter>();
            });
        }
    }

    public class RecmdInventoryDriverBinaryData : IFileSpecData
    {
        public DateTime? Timestamp { get; set; }
        public string BatchKeyPath { get; set; }
        public string DriverLastWriteTime { get; set; }
        public string BatchValueName { get; set; }
        public string DriverCompany { get; set; }
        public string Product { get; set; }
        public string ProductVersion { get; set; }
        public string DriverName { get; set; }
        public string DriverVersion { get; set; }
        public string Path { get; set; }
        public string SHA1 { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{Timestamp} {BatchKeyPath} {DriverLastWriteTime} {BatchValueName} {DriverCompany} {Product} {ProductVersion} {DriverName} {DriverVersion} {Path} {SHA1}";
        }
    }

    /// <summary>
    ///     RECmd's CSV for Amcache-InventoryDriverBinary (RegistryPlugin.Amcache-InventoryDriverBinary.dll)
    /// </summary>
    public class RecmdInventoryDriverBinary : IFileSpec
    {
        public RecmdInventoryDriverBinary()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<RecmdInventoryDriverBinaryData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Timestamp,BatchKeyPath,DriverLastWriteTime,BatchValueName,DriverCompany,Product,ProductVersion,DriverName,DriverVersion,Path,SHA1"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from RECmd for the InventoryDriverBinary registry plugin";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "90a43ec0-42b3-4434-acc2-37f72efdfc69";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<RecmdInventoryDriverBinaryData>(filename, DataList, TaggedLines, map =>
            {
                map.Map(t => t.Timestamp).TypeConverter<LenientDateTimeConverter>();
            });
        }
    }

    public class RecmdJumplistDataData : IFileSpecData
    {
        public string JumpListName { get; set; }
        public string BatchKeyPath { get; set; }
        public DateTime? ExecutedOn { get; set; }
        public string BatchValueName { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{JumpListName} {BatchKeyPath} {ExecutedOn} {BatchValueName}";
        }
    }

    /// <summary>
    ///     RECmd's CSV for JumplistData (RegistryPlugin.JumplistData.dll)
    /// </summary>
    public class RecmdJumplistData : IFileSpec
    {
        public RecmdJumplistData()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<RecmdJumplistDataData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "JumpListName,BatchKeyPath,ExecutedOn,BatchValueName"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from RECmd for the JumplistData registry plugin";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "4f97b5db-9040-4e20-9f11-394d8aa50761";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<RecmdJumplistDataData>(filename, DataList, TaggedLines, map =>
            {
                map.Map(t => t.ExecutedOn).TypeConverter<LenientDateTimeConverter>();
            });
        }
    }

    public class RecmdKnownNetworksData : IFileSpecData
    {
        public string FirstNetwork { get; set; }
        public string BatchKeyPath { get; set; }
        public string NetworkName { get; set; }
        public string BatchValueName { get; set; }
        public string NameType { get; set; }
        public DateTime? FirstConnectLOCAL { get; set; }
        public DateTime? LastConnectedLOCAL { get; set; }
        public bool? Managed { get; set; }
        public string DNSSuffix { get; set; }
        public string GatewayMacAddress { get; set; }
        public string ProfileGUID { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{FirstNetwork} {BatchKeyPath} {NetworkName} {BatchValueName} {NameType} {FirstConnectLOCAL} {LastConnectedLOCAL} {Managed} {DNSSuffix} {GatewayMacAddress} {ProfileGUID}";
        }
    }

    /// <summary>
    ///     RECmd's CSV for Known networks (RegistryPlugin.KnownNetworks.dll)
    /// </summary>
    public class RecmdKnownNetworks : IFileSpec
    {
        public RecmdKnownNetworks()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<RecmdKnownNetworksData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "FirstNetwork,BatchKeyPath,NetworkName,BatchValueName,NameType,FirstConnectLOCAL,LastConnectedLOCAL,Managed,DNSSuffix,GatewayMacAddress,ProfileGUID"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from RECmd for the KnownNetworks registry plugin";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "9077881f-7ade-401c-8283-b062a5075186";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<RecmdKnownNetworksData>(filename, DataList, TaggedLines, map =>
            {
                map.Map(t => t.FirstConnectLOCAL).TypeConverter<LenientDateTimeConverter>();
                map.Map(t => t.LastConnectedLOCAL).TypeConverter<LenientDateTimeConverter>();
            });
        }
    }

    public class RecmdLastVisitedMRUData : IFileSpecData
    {
        public string ValueName { get; set; }
        public string BatchKeyPath { get; set; }
        public long? MruPosition { get; set; }
        public string BatchValueName { get; set; }
        public string Executable { get; set; }
        public string Directory { get; set; }
        public DateTime? OpenedOn { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{ValueName} {BatchKeyPath} {MruPosition} {BatchValueName} {Executable} {Directory} {OpenedOn}";
        }
    }

    /// <summary>
    ///     RECmd's CSV for ComDlg32 LastVisitedMRU (RegistryPlugin.LastVisitedMRU.dll)
    /// </summary>
    public class RecmdLastVisitedMRU : IFileSpec
    {
        public RecmdLastVisitedMRU()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<RecmdLastVisitedMRUData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "ValueName,BatchKeyPath,MruPosition,BatchValueName,Executable,Directory,OpenedOn"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from RECmd for the LastVisitedMRU registry plugin";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "16fd846d-a315-4cfd-905a-26ddeecaf057";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<RecmdLastVisitedMRUData>(filename, DataList, TaggedLines, map =>
            {
                map.Map(t => t.OpenedOn).TypeConverter<LenientDateTimeConverter>();
            });
        }
    }

    public class RecmdLastVisitedPidlMRUData : IFileSpecData
    {
        public string ValueName { get; set; }
        public string BatchKeyPath { get; set; }
        public long? MruPosition { get; set; }
        public string BatchValueName { get; set; }
        public string Executable { get; set; }
        public string AbsolutePath { get; set; }
        public DateTime? OpenedOn { get; set; }
        public string Details { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{ValueName} {BatchKeyPath} {MruPosition} {BatchValueName} {Executable} {AbsolutePath} {OpenedOn} {Details}";
        }
    }

    /// <summary>
    ///     RECmd's CSV for ComDlg32 LastVisitedPidlMRU (RegistryPlugin.LastVisitedPidlMRU.dll)
    /// </summary>
    public class RecmdLastVisitedPidlMRU : IFileSpec
    {
        public RecmdLastVisitedPidlMRU()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<RecmdLastVisitedPidlMRUData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "ValueName,BatchKeyPath,MruPosition,BatchValueName,Executable,AbsolutePath,OpenedOn,Details"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from RECmd for the LastVisitedPidlMRU registry plugin";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "c1abc5e1-d239-4ade-ba40-24719b92c72f";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<RecmdLastVisitedPidlMRUData>(filename, DataList, TaggedLines, map =>
            {
                map.Map(t => t.OpenedOn).TypeConverter<LenientDateTimeConverter>();
            });
        }
    }

    public class RecmdMareData : IFileSpecData
    {
        public string Unknown { get; set; }
        public string BatchKeyPath { get; set; }
        public string Path { get; set; }
        public string BatchValueName { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{Unknown} {BatchKeyPath} {Path} {BatchValueName}";
        }
    }

    /// <summary>
    ///     RECmd's CSV for Amcache-Mare (RegistryPlugin.Amcache-Mare.dll)
    /// </summary>
    public class RecmdMare : IFileSpec
    {
        public RecmdMare()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<RecmdMareData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Unknown,BatchKeyPath,Path,BatchValueName"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from RECmd for the Mare registry plugin";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "82b07beb-cdc4-43f1-9a69-ed72883982c3";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<RecmdMareData>(filename, DataList, TaggedLines);
        }
    }

    public class RecmdMountedDevicesData : IFileSpecData
    {
        public string DeviceName { get; set; }
        public string BatchKeyPath { get; set; }
        public string DeviceData { get; set; }
        public string BatchValueName { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{DeviceName} {BatchKeyPath} {DeviceData} {BatchValueName}";
        }
    }

    /// <summary>
    ///     RECmd's CSV for MountedDevices (RegistryPlugin.MountedDevices.dll)
    /// </summary>
    public class RecmdMountedDevices : IFileSpec
    {
        public RecmdMountedDevices()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<RecmdMountedDevicesData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "DeviceName,BatchKeyPath,DeviceData,BatchValueName"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from RECmd for the MountedDevices registry plugin";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "89a8b2f1-7d18-47eb-b06a-069848ca916e";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<RecmdMountedDevicesData>(filename, DataList, TaggedLines);
        }
    }

    public class RecmdNetworkAdaptersData : IFileSpecData
    {
        public DateTime? Timestamp { get; set; }
        public string BatchKeyPath { get; set; }
        public string DriverDesc { get; set; }
        public string BatchValueName { get; set; }
        public string DriverDate { get; set; }
        public string DriverVersion { get; set; }
        public string ProviderName { get; set; }
        public string DeviceInstanceid { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{Timestamp} {BatchKeyPath} {DriverDesc} {BatchValueName} {DriverDate} {DriverVersion} {ProviderName} {DeviceInstanceid}";
        }
    }

    /// <summary>
    ///     RECmd's CSV for NetworkAdapters (RegistryPlugin.NetworkAdapters.dll)
    /// </summary>
    public class RecmdNetworkAdapters : IFileSpec
    {
        public RecmdNetworkAdapters()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<RecmdNetworkAdaptersData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Timestamp,BatchKeyPath,DriverDesc,BatchValueName,DriverDate,DriverVersion,ProviderName,DeviceInstanceid"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from RECmd for the NetworkAdapters registry plugin";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "47d49dab-e410-4b03-8852-19d944e7f632";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<RecmdNetworkAdaptersData>(filename, DataList, TaggedLines, map =>
            {
                map.Map(t => t.Timestamp).TypeConverter<LenientDateTimeConverter>();
            });
        }
    }

    public class RecmdNetworkSettingsData : IFileSpecData
    {
        public string IPAddress { get; set; }
        public string BatchKeyPath { get; set; }
        public string SubnetMask { get; set; }
        public string BatchValueName { get; set; }
        public string DHCPSubnetMask { get; set; }
        public string DHCPServer { get; set; }
        public string DHCPNameServer { get; set; }
        public string DHCPIPAddress { get; set; }
        public string DHCPDefaultGateway { get; set; }
        public DateTime? LeaseObtainedTime { get; set; }
        public DateTime? LeaseTerminatesTime { get; set; }
        public bool? EnabledDHCP { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{IPAddress} {BatchKeyPath} {SubnetMask} {BatchValueName} {DHCPSubnetMask} {DHCPServer} {DHCPNameServer} {DHCPIPAddress} {DHCPDefaultGateway} {LeaseObtainedTime} {LeaseTerminatesTime} {EnabledDHCP}";
        }
    }

    /// <summary>
    ///     RECmd's CSV for NetworkSettings (RegistryPlugin.NetworkSettings.dll)
    /// </summary>
    public class RecmdNetworkSettings : IFileSpec
    {
        public RecmdNetworkSettings()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<RecmdNetworkSettingsData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "IPAddress,BatchKeyPath,SubnetMask,BatchValueName,DHCPSubnetMask,DHCPServer,DHCPNameServer,DHCPIPAddress,DHCPDefaultGateway,LeaseObtainedTime,LeaseTerminatesTime,EnabledDHCP"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from RECmd for the NetworkSettings registry plugin";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "84a73f46-4fc3-44d3-a04b-de63d5a220ce";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<RecmdNetworkSettingsData>(filename, DataList, TaggedLines, map =>
            {
                map.Map(t => t.LeaseObtainedTime).TypeConverter<LenientDateTimeConverter>();
                map.Map(t => t.LeaseTerminatesTime).TypeConverter<LenientDateTimeConverter>();
            });
        }
    }

    public class RecmdNetworkSetup2Data : IFileSpecData
    {
        public string ProtocolList { get; set; }
        public string BatchKeyPath { get; set; }
        public string Alias { get; set; }
        public string BatchValueName { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string PermanentAddress { get; set; }
        public string CurrentAddress { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{ProtocolList} {BatchKeyPath} {Alias} {BatchValueName} {Description} {Type} {PermanentAddress} {CurrentAddress}";
        }
    }

    /// <summary>
    ///     RECmd's CSV for NetworkSetup2 (RegistryPlugin.NetworkSetup2.dll)
    /// </summary>
    public class RecmdNetworkSetup2 : IFileSpec
    {
        public RecmdNetworkSetup2()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<RecmdNetworkSetup2Data>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "ProtocolList,BatchKeyPath,Alias,BatchValueName,Description,Type,PermanentAddress,CurrentAddress"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from RECmd for the NetworkSetup2 registry plugin";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "c8226f48-fbc9-4d98-b8c8-6aaa7633f9e9";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<RecmdNetworkSetup2Data>(filename, DataList, TaggedLines);
        }
    }

    public class RecmdOfficeMRUData : IFileSpecData
    {
        public string ValueName { get; set; }
        public string BatchKeyPath { get; set; }
        public DateTime? LastOpened { get; set; }
        public string BatchValueName { get; set; }
        public DateTime? LastClosed { get; set; }
        public string FileName { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{ValueName} {BatchKeyPath} {LastOpened} {BatchValueName} {LastClosed} {FileName}";
        }
    }

    /// <summary>
    ///     RECmd's CSV for Office MRU (RegistryPlugin.OfficeMRU.dll)
    /// </summary>
    public class RecmdOfficeMRU : IFileSpec
    {
        public RecmdOfficeMRU()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<RecmdOfficeMRUData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "ValueName,BatchKeyPath,LastOpened,BatchValueName,LastClosed,FileName"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from RECmd for the OfficeMRU registry plugin";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "7a9535ba-78de-4a80-9620-1195bcbfae2e";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<RecmdOfficeMRUData>(filename, DataList, TaggedLines, map =>
            {
                map.Map(t => t.LastOpened).TypeConverter<LenientDateTimeConverter>();
                map.Map(t => t.LastClosed).TypeConverter<LenientDateTimeConverter>();
            });
        }
    }

    public class RecmdOpenSaveMRUData : IFileSpecData
    {
        public string Extension { get; set; }
        public string BatchKeyPath { get; set; }
        public string ValueName { get; set; }
        public string BatchValueName { get; set; }
        public long? MruPosition { get; set; }
        public string Filename { get; set; }
        public DateTime? OpenedOn { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{Extension} {BatchKeyPath} {ValueName} {BatchValueName} {MruPosition} {Filename} {OpenedOn}";
        }
    }

    /// <summary>
    ///     RECmd's CSV for ComDlg32 OpenSaveMRU (RegistryPlugin.OpenSaveMRU.dll)
    /// </summary>
    public class RecmdOpenSaveMRU : IFileSpec
    {
        public RecmdOpenSaveMRU()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<RecmdOpenSaveMRUData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Extension,BatchKeyPath,ValueName,BatchValueName,MruPosition,Filename,OpenedOn"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from RECmd for the OpenSaveMRU registry plugin";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "ec401471-b016-45d3-91f5-a02887c67656";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<RecmdOpenSaveMRUData>(filename, DataList, TaggedLines, map =>
            {
                map.Map(t => t.OpenedOn).TypeConverter<LenientDateTimeConverter>();
            });
        }
    }

    public class RecmdOpenSavePidlMRUData : IFileSpecData
    {
        public string Extension { get; set; }
        public string BatchKeyPath { get; set; }
        public string ValueName { get; set; }
        public string BatchValueName { get; set; }
        public long? MruPosition { get; set; }
        public string AbsolutePath { get; set; }
        public DateTime? OpenedOn { get; set; }
        public string Details { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{Extension} {BatchKeyPath} {ValueName} {BatchValueName} {MruPosition} {AbsolutePath} {OpenedOn} {Details}";
        }
    }

    /// <summary>
    ///     RECmd's CSV for ComDlg32 OpenSavePidlMRU (RegistryPlugin.OpenSavePidlMRU.dll)
    /// </summary>
    public class RecmdOpenSavePidlMRU : IFileSpec
    {
        public RecmdOpenSavePidlMRU()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<RecmdOpenSavePidlMRUData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Extension,BatchKeyPath,ValueName,BatchValueName,MruPosition,AbsolutePath,OpenedOn,Details"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from RECmd for the OpenSavePidlMRU registry plugin";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "466f2618-46c5-42ab-8cfa-73a17eb7d770";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<RecmdOpenSavePidlMRUData>(filename, DataList, TaggedLines, map =>
            {
                map.Map(t => t.OpenedOn).TypeConverter<LenientDateTimeConverter>();
            });
        }
    }

    public class RecmdProductsData : IFileSpecData
    {
        public DateTime? Timestamp { get; set; }
        public string BatchKeyPath { get; set; }
        public string DisplayName { get; set; }
        public string BatchValueName { get; set; }
        public string PackageName { get; set; }
        public string InstallDate { get; set; }
        public string Publisher { get; set; }
        public string Language { get; set; }
        public string InstallLocation { get; set; }
        public string InstallSource { get; set; }
        public string Comments { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{Timestamp} {BatchKeyPath} {DisplayName} {BatchValueName} {PackageName} {InstallDate} {Publisher} {Language} {InstallLocation} {InstallSource} {Comments}";
        }
    }

    /// <summary>
    ///     RECmd's CSV for Products (RegistryPlugin.Products.dll)
    /// </summary>
    public class RecmdProducts : IFileSpec
    {
        public RecmdProducts()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<RecmdProductsData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Timestamp,BatchKeyPath,DisplayName,BatchValueName,PackageName,InstallDate,Publisher,Language,InstallLocation,InstallSource,Comments"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from RECmd for the Products registry plugin";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "c9f320c3-c75e-4407-be0a-1bcf3b0e72e5";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<RecmdProductsData>(filename, DataList, TaggedLines, map =>
            {
                map.Map(t => t.Timestamp).TypeConverter<LenientDateTimeConverter>();
            });
        }
    }

    public class RecmdProfileListData : IFileSpecData
    {
        public DateTime? Timestamp { get; set; }
        public string BatchKeyPath { get; set; }
        public string KeyName { get; set; }
        public string BatchValueName { get; set; }
        public string ProfileImagePath { get; set; }
        public DateTime? LastLogonTime { get; set; }
        public DateTime? LastLogoffTime { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{Timestamp} {BatchKeyPath} {KeyName} {BatchValueName} {ProfileImagePath} {LastLogonTime} {LastLogoffTime}";
        }
    }

    /// <summary>
    ///     RECmd's CSV for ProfileList (RegistryPlugin.ProfileList.dll)
    /// </summary>
    public class RecmdProfileList : IFileSpec
    {
        public RecmdProfileList()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<RecmdProfileListData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Timestamp,BatchKeyPath,KeyName,BatchValueName,ProfileImagePath,LastLogonTime,LastLogoffTime"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from RECmd for the ProfileList registry plugin";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "2703272d-4d3d-4ece-ba41-e86cc1b2aa73";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<RecmdProfileListData>(filename, DataList, TaggedLines, map =>
            {
                map.Map(t => t.Timestamp).TypeConverter<LenientDateTimeConverter>();
                map.Map(t => t.LastLogonTime).TypeConverter<LenientDateTimeConverter>();
                map.Map(t => t.LastLogoffTime).TypeConverter<LenientDateTimeConverter>();
            });
        }
    }

    public class RecmdRADARData : IFileSpecData
    {
        public string KeyName { get; set; }
        public string BatchKeyPath { get; set; }
        public string Filename { get; set; }
        public string BatchValueName { get; set; }
        public DateTime? LastDetectionTime { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{KeyName} {BatchKeyPath} {Filename} {BatchValueName} {LastDetectionTime}";
        }
    }

    /// <summary>
    ///     RECmd's CSV for RADAR (RegistryPlugin.RADAR.dll)
    /// </summary>
    public class RecmdRADAR : IFileSpec
    {
        public RecmdRADAR()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<RecmdRADARData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "KeyName,BatchKeyPath,Filename,BatchValueName,LastDetectionTime"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from RECmd for the RADAR registry plugin";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "510d303e-16f8-4661-a37f-c4abfbe6a895";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<RecmdRADARData>(filename, DataList, TaggedLines, map =>
            {
                map.Map(t => t.LastDetectionTime).TypeConverter<LenientDateTimeConverter>();
            });
        }
    }

    public class RecmdRecentAppsData : IFileSpecData
    {
        public string KeyName { get; set; }
        public string BatchKeyPath { get; set; }
        public string AppId { get; set; }
        public string BatchValueName { get; set; }
        public string AppPath { get; set; }
        public DateTime? LastAccessed { get; set; }
        public long? LaunchCount { get; set; }
        public string RecentDocs { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{KeyName} {BatchKeyPath} {AppId} {BatchValueName} {AppPath} {LastAccessed} {LaunchCount} {RecentDocs}";
        }
    }

    /// <summary>
    ///     RECmd's CSV for RecentApps (RegistryPlugin.RecentApps.dll)
    /// </summary>
    public class RecmdRecentApps : IFileSpec
    {
        public RecmdRecentApps()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<RecmdRecentAppsData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "KeyName,BatchKeyPath,AppId,BatchValueName,AppPath,LastAccessed,LaunchCount,RecentDocs"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from RECmd for the RecentApps registry plugin";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "a413a3a2-9d1f-48d5-922c-f59fd226cc72";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<RecmdRecentAppsData>(filename, DataList, TaggedLines, map =>
            {
                map.Map(t => t.LastAccessed).TypeConverter<LenientDateTimeConverter>();
            });
        }
    }

    public class RecmdRecentDocsData : IFileSpecData
    {
        public string Extension { get; set; }
        public string BatchKeyPath { get; set; }
        public string ValueName { get; set; }
        public string BatchValueName { get; set; }
        public string TargetName { get; set; }
        public string LnkName { get; set; }
        public long? MruPosition { get; set; }
        public DateTime? OpenedOn { get; set; }
        public DateTime? ExtensionLastOpened { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{Extension} {BatchKeyPath} {ValueName} {BatchValueName} {TargetName} {LnkName} {MruPosition} {OpenedOn} {ExtensionLastOpened}";
        }
    }

    /// <summary>
    ///     RECmd's CSV for Recent documents (RegistryPlugin.RecentDocs.dll)
    /// </summary>
    public class RecmdRecentDocs : IFileSpec
    {
        public RecmdRecentDocs()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<RecmdRecentDocsData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Extension,BatchKeyPath,ValueName,BatchValueName,TargetName,LnkName,MruPosition,OpenedOn,ExtensionLastOpened"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from RECmd for the RecentDocs registry plugin";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "5239f79b-d06e-4087-a8dc-7e76db469d55";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<RecmdRecentDocsData>(filename, DataList, TaggedLines, map =>
            {
                map.Map(t => t.OpenedOn).TypeConverter<LenientDateTimeConverter>();
                map.Map(t => t.ExtensionLastOpened).TypeConverter<LenientDateTimeConverter>();
            });
        }
    }

    public class RecmdRunMRUData : IFileSpecData
    {
        public string ValueName { get; set; }
        public string BatchKeyPath { get; set; }
        public long? MruPosition { get; set; }
        public string BatchValueName { get; set; }
        public string Executable { get; set; }
        public DateTime? OpenedOn { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{ValueName} {BatchKeyPath} {MruPosition} {BatchValueName} {Executable} {OpenedOn}";
        }
    }

    /// <summary>
    ///     RECmd's CSV for RunMRU (RegistryPlugin.RunMRU.dll)
    /// </summary>
    public class RecmdRunMRU : IFileSpec
    {
        public RecmdRunMRU()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<RecmdRunMRUData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "ValueName,BatchKeyPath,MruPosition,BatchValueName,Executable,OpenedOn"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from RECmd for the RunMRU registry plugin";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "44e44576-7473-4f81-9141-c0a9e436d927";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<RecmdRunMRUData>(filename, DataList, TaggedLines, map =>
            {
                map.Map(t => t.OpenedOn).TypeConverter<LenientDateTimeConverter>();
            });
        }
    }

    public class RecmdSAMBuiltinData : IFileSpecData
    {
        public string GroupName { get; set; }
        public string BatchKeyPath { get; set; }
        public string Comment { get; set; }
        public string BatchValueName { get; set; }
        public string Users { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{GroupName} {BatchKeyPath} {Comment} {BatchValueName} {Users}";
        }
    }

    /// <summary>
    ///     RECmd's CSV for SAMBuiltin (RegistryPlugin.SAMBuiltin.dll)
    /// </summary>
    public class RecmdSAMBuiltin : IFileSpec
    {
        public RecmdSAMBuiltin()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<RecmdSAMBuiltinData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "GroupName,BatchKeyPath,Comment,BatchValueName,Users"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from RECmd for the SAMBuiltin registry plugin";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "a857522e-b57f-4f01-85d0-223104fae5cf";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<RecmdSAMBuiltinData>(filename, DataList, TaggedLines);
        }
    }

    public class RecmdSCSIData : IFileSpecData
    {
        public DateTime? Timestamp { get; set; }
        public string BatchKeyPath { get; set; }
        public string Manufacturer { get; set; }
        public string BatchValueName { get; set; }
        public string Title { get; set; }
        public string SerialNumber { get; set; }
        public string DeviceName { get; set; }
        public string DiskId { get; set; }
        public DateTime? InitialTimestamp { get; set; }
        public DateTime? Installed { get; set; }
        public DateTime? FirstInstalled { get; set; }
        public DateTime? LastConnected { get; set; }
        public DateTime? LastRemoved { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{Timestamp} {BatchKeyPath} {Manufacturer} {BatchValueName} {Title} {SerialNumber} {DeviceName} {DiskId} {InitialTimestamp} {Installed} {FirstInstalled} {LastConnected} {LastRemoved}";
        }
    }

    /// <summary>
    ///     RECmd's CSV for SCSI (RegistryPlugin.SCSI.dll)
    /// </summary>
    public class RecmdSCSI : IFileSpec
    {
        public RecmdSCSI()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<RecmdSCSIData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Timestamp,BatchKeyPath,Manufacturer,BatchValueName,Title,SerialNumber,DeviceName,DiskId,InitialTimestamp,Installed,FirstInstalled,LastConnected,LastRemoved"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from RECmd for the SCSI registry plugin";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "346f05e5-3c05-4d41-a885-d6da6a21af4c";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<RecmdSCSIData>(filename, DataList, TaggedLines, map =>
            {
                map.Map(t => t.Timestamp).TypeConverter<LenientDateTimeConverter>();
                map.Map(t => t.InitialTimestamp).TypeConverter<LenientDateTimeConverter>();
                map.Map(t => t.Installed).TypeConverter<LenientDateTimeConverter>();
                map.Map(t => t.FirstInstalled).TypeConverter<LenientDateTimeConverter>();
                map.Map(t => t.LastConnected).TypeConverter<LenientDateTimeConverter>();
                map.Map(t => t.LastRemoved).TypeConverter<LenientDateTimeConverter>();
            });
        }
    }

    public class RecmdSWDData : IFileSpecData
    {
        public string Type { get; set; }
        public string BatchKeyPath { get; set; }
        public string KeyName { get; set; }
        public string BatchValueName { get; set; }
        public string Service { get; set; }
        public string DeviceDesc { get; set; }
        public string FriendlyName { get; set; }
        public DateTime? Installed { get; set; }
        public DateTime? FirstInstalled { get; set; }
        public DateTime? LastConnected { get; set; }
        public DateTime? LastRemoved { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{Type} {BatchKeyPath} {KeyName} {BatchValueName} {Service} {DeviceDesc} {FriendlyName} {Installed} {FirstInstalled} {LastConnected} {LastRemoved}";
        }
    }

    /// <summary>
    ///     RECmd's CSV for SWD (RegistryPlugin.SWD.dll)
    /// </summary>
    public class RecmdSWD : IFileSpec
    {
        public RecmdSWD()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<RecmdSWDData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Type,BatchKeyPath,KeyName,BatchValueName,Service,DeviceDesc,FriendlyName,Installed,FirstInstalled,LastConnected,LastRemoved"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from RECmd for the SWD registry plugin";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "62bf6bb0-f8d0-4501-92e2-435f727be135";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<RecmdSWDData>(filename, DataList, TaggedLines, map =>
            {
                map.Map(t => t.Installed).TypeConverter<LenientDateTimeConverter>();
                map.Map(t => t.FirstInstalled).TypeConverter<LenientDateTimeConverter>();
                map.Map(t => t.LastConnected).TypeConverter<LenientDateTimeConverter>();
                map.Map(t => t.LastRemoved).TypeConverter<LenientDateTimeConverter>();
            });
        }
    }

    public class RecmdServicesData : IFileSpecData
    {
        public string Name { get; set; }
        public string BatchKeyPath { get; set; }
        public string Description { get; set; }
        public string BatchValueName { get; set; }
        public string DisplayName { get; set; }
        public string StartMode { get; set; }
        public string ServiceType { get; set; }
        public DateTime? NameKeyLastWrite { get; set; }
        public DateTime? ParametersKeyLastWrite { get; set; }
        public string Group { get; set; }
        public string ImagePath { get; set; }
        public string ServiceDLL { get; set; }
        public string RequiredPrivileges { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{Name} {BatchKeyPath} {Description} {BatchValueName} {DisplayName} {StartMode} {ServiceType} {NameKeyLastWrite} {ParametersKeyLastWrite} {Group} {ImagePath} {ServiceDLL} {RequiredPrivileges}";
        }
    }

    /// <summary>
    ///     RECmd's CSV for Services (RegistryPlugin.Services.dll)
    /// </summary>
    public class RecmdServices : IFileSpec
    {
        public RecmdServices()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<RecmdServicesData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Name,BatchKeyPath,Description,BatchValueName,DisplayName,StartMode,ServiceType,NameKeyLastWrite,ParametersKeyLastWrite,Group,ImagePath,ServiceDLL,RequiredPrivileges"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from RECmd for the Services registry plugin";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "0d2578d5-8d1c-41ab-995b-a71ca61a75a4";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<RecmdServicesData>(filename, DataList, TaggedLines, map =>
            {
                map.Map(t => t.NameKeyLastWrite).TypeConverter<LenientDateTimeConverter>();
                map.Map(t => t.ParametersKeyLastWrite).TypeConverter<LenientDateTimeConverter>();
            });
        }
    }

    public class RecmdSevenZipData : IFileSpecData
    {
        public string ArchiveName { get; set; }
        public string BatchKeyPath { get; set; }
        public string BatchValueName { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{ArchiveName} {BatchKeyPath} {BatchValueName}";
        }
    }

    /// <summary>
    ///     RECmd's CSV for 7-Zip archive history (RegistryPlugin.7-ZipHistory.dll)
    /// </summary>
    public class RecmdSevenZip : IFileSpec
    {
        public RecmdSevenZip()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<RecmdSevenZipData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "ArchiveName,BatchKeyPath,BatchValueName"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from RECmd for the SevenZip registry plugin";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "cd4603d6-3fe1-48f1-87ff-2139f1c99e13";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<RecmdSevenZipData>(filename, DataList, TaggedLines);
        }
    }

    public class RecmdSevenZipCopyHistoryData : IFileSpecData
    {
        public string CopyPath { get; set; }
        public string BatchKeyPath { get; set; }
        public string BatchValueName { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{CopyPath} {BatchKeyPath} {BatchValueName}";
        }
    }

    /// <summary>
    ///     RECmd's CSV for 7-Zip copy history (RegistryPlugin.7-ZipCopyHistory.dll)
    /// </summary>
    public class RecmdSevenZipCopyHistory : IFileSpec
    {
        public RecmdSevenZipCopyHistory()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<RecmdSevenZipCopyHistoryData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "CopyPath,BatchKeyPath,BatchValueName"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from RECmd for the SevenZipCopyHistory registry plugin";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "791d183c-b523-47af-b65f-66601c6726eb";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<RecmdSevenZipCopyHistoryData>(filename, DataList, TaggedLines);
        }
    }

    public class RecmdSevenZipFolderHistoryData : IFileSpecData
    {
        public string FolderPath { get; set; }
        public string BatchKeyPath { get; set; }
        public string BatchValueName { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{FolderPath} {BatchKeyPath} {BatchValueName}";
        }
    }

    /// <summary>
    ///     RECmd's CSV for 7-Zip folder history (RegistryPlugin.7-ZipFolderHistory.dll)
    /// </summary>
    public class RecmdSevenZipFolderHistory : IFileSpec
    {
        public RecmdSevenZipFolderHistory()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<RecmdSevenZipFolderHistoryData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "FolderPath,BatchKeyPath,BatchValueName"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from RECmd for the SevenZipFolderHistory registry plugin";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "ba9ee8db-789f-433d-8015-ddcaee9dcf67";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<RecmdSevenZipFolderHistoryData>(filename, DataList, TaggedLines);
        }
    }

    public class RecmdSevenZipPathHistoryData : IFileSpecData
    {
        public string ExtractionPath { get; set; }
        public string BatchKeyPath { get; set; }
        public string BatchValueName { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{ExtractionPath} {BatchKeyPath} {BatchValueName}";
        }
    }

    /// <summary>
    ///     RECmd's CSV for 7-Zip extraction path history (RegistryPlugin.7-ZipPathHistory.dll)
    /// </summary>
    public class RecmdSevenZipPathHistory : IFileSpec
    {
        public RecmdSevenZipPathHistory()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<RecmdSevenZipPathHistoryData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "ExtractionPath,BatchKeyPath,BatchValueName"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from RECmd for the SevenZipPathHistory registry plugin";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "d20f3a3b-272a-4e9a-a931-0ade3801cbec";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<RecmdSevenZipPathHistoryData>(filename, DataList, TaggedLines);
        }
    }

    public class RecmdSyscacheObjectTableData : IFileSpecData
    {
        public string KeyPath { get; set; }
        public string BatchKeyPath { get; set; }
        public long? ObjectId { get; set; }
        public string BatchValueName { get; set; }
        public long? ObjectLru { get; set; }
        public long? Usn { get; set; }
        public long? UsnJournalId { get; set; }
        public long? MftEntryNumber { get; set; }
        public long? MftSequenceNumber { get; set; }
        public string Sha1 { get; set; }
        public string AeProgramId { get; set; }
        public DateTime? LastWriteTime { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{KeyPath} {BatchKeyPath} {ObjectId} {BatchValueName} {ObjectLru} {Usn} {UsnJournalId} {MftEntryNumber} {MftSequenceNumber} {Sha1} {AeProgramId} {LastWriteTime}";
        }
    }

    /// <summary>
    ///     RECmd's CSV for Syscache.hve ObjectTable (RegistryPlugin.SyscacheObjectTable.dll)
    /// </summary>
    public class RecmdSyscacheObjectTable : IFileSpec
    {
        public RecmdSyscacheObjectTable()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<RecmdSyscacheObjectTableData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "KeyPath,BatchKeyPath,ObjectId,BatchValueName,ObjectLru,Usn,UsnJournalId,MftEntryNumber,MftSequenceNumber,Sha1,AeProgramId,LastWriteTime"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from RECmd for the SyscacheObjectTable registry plugin";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "f5e0c6e1-ec30-4cbf-b160-3331bc69f70d";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<RecmdSyscacheObjectTableData>(filename, DataList, TaggedLines, map =>
            {
                map.Map(t => t.LastWriteTime).TypeConverter<LenientDateTimeConverter>();
            });
        }
    }

    public class RecmdTaskCacheData : IFileSpecData
    {
        public long? Version { get; set; }
        public string BatchKeyPath { get; set; }
        public string KeyName { get; set; }
        public string BatchValueName { get; set; }
        public string Path { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? LastStart { get; set; }
        public DateTime? LastStop { get; set; }
        public long? TaskState { get; set; }
        public long? LastActionResult { get; set; }
        public string Source { get; set; }
        public string Description { get; set; }
        public string Command { get; set; }
        public string Arguments { get; set; }
        public string SecurityDescriptor { get; set; }
        public string Author { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{Version} {BatchKeyPath} {KeyName} {BatchValueName} {Path} {CreatedOn} {LastStart} {LastStop} {TaskState} {LastActionResult} {Source} {Description} {Command} {Arguments} {SecurityDescriptor} {Author}";
        }
    }

    /// <summary>
    ///     RECmd's CSV for TaskCache (RegistryPlugin.TaskCache.dll)
    /// </summary>
    public class RecmdTaskCache : IFileSpec
    {
        public RecmdTaskCache()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<RecmdTaskCacheData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Version,BatchKeyPath,KeyName,BatchValueName,Path,CreatedOn,LastStart,LastStop,TaskState,LastActionResult,Source,Description,Command,Arguments,SecurityDescriptor,Author"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from RECmd for the TaskCache registry plugin";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "c9ee7add-7c71-4f4d-be14-0005e300596c";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<RecmdTaskCacheData>(filename, DataList, TaggedLines, map =>
            {
                map.Map(t => t.CreatedOn).TypeConverter<LenientDateTimeConverter>();
                map.Map(t => t.LastStart).TypeConverter<LenientDateTimeConverter>();
                map.Map(t => t.LastStop).TypeConverter<LenientDateTimeConverter>();
            });
        }
    }

    public class RecmdTaskFlowShellActivitiesData : IFileSpecData
    {
        public long? Position { get; set; }
        public string BatchKeyPath { get; set; }
        public string FullPath { get; set; }
        public string BatchValueName { get; set; }
        public string ExeName { get; set; }
        public string WindowTitle { get; set; }
        public string Other { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{Position} {BatchKeyPath} {FullPath} {BatchValueName} {ExeName} {WindowTitle} {Other}";
        }
    }

    /// <summary>
    ///     RECmd's CSV for Taskflow Shell Activities (RegistryPlugin.TaskFlowShellActivities.dll)
    /// </summary>
    public class RecmdTaskFlowShellActivities : IFileSpec
    {
        public RecmdTaskFlowShellActivities()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<RecmdTaskFlowShellActivitiesData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Position,BatchKeyPath,FullPath,BatchValueName,ExeName,WindowTitle,Other"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from RECmd for the TaskFlowShellActivities registry plugin";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "f22f39a2-c99f-4352-acea-346e005533a3";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<RecmdTaskFlowShellActivitiesData>(filename, DataList, TaggedLines);
        }
    }

    public class RecmdTaskbandData : IFileSpecData
    {
        public string LnkName { get; set; }
        public string BatchKeyPath { get; set; }
        public string Executable { get; set; }
        public string BatchValueName { get; set; }
        public string PinType { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{LnkName} {BatchKeyPath} {Executable} {BatchValueName} {PinType}";
        }
    }

    /// <summary>
    ///     RECmd's CSV for Taskband (RegistryPlugin.Taskband.dll)
    /// </summary>
    public class RecmdTaskband : IFileSpec
    {
        public RecmdTaskband()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<RecmdTaskbandData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "LnkName,BatchKeyPath,Executable,BatchValueName,PinType"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from RECmd for the Taskband registry plugin";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "66347204-960c-4c57-b964-bd9ce64b67a6";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<RecmdTaskbandData>(filename, DataList, TaggedLines);
        }
    }

    public class RecmdTerminalServerClientData : IFileSpecData
    {
        public string HostName { get; set; }
        public string BatchKeyPath { get; set; }
        public string Username { get; set; }
        public string BatchValueName { get; set; }
        public long? MRUPosition { get; set; }
        public DateTime? LastModified { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{HostName} {BatchKeyPath} {Username} {BatchValueName} {MRUPosition} {LastModified}";
        }
    }

    /// <summary>
    ///     RECmd's CSV for TerminalServerClient (RegistryPlugin.TerminalServerClient.dll)
    /// </summary>
    public class RecmdTerminalServerClient : IFileSpec
    {
        public RecmdTerminalServerClient()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<RecmdTerminalServerClientData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "HostName,BatchKeyPath,Username,BatchValueName,MRUPosition,LastModified"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from RECmd for the TerminalServerClient registry plugin";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "c7c4cfec-71b5-44aa-947a-5c2eb87a7384";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<RecmdTerminalServerClientData>(filename, DataList, TaggedLines, map =>
            {
                map.Map(t => t.LastModified).TypeConverter<LenientDateTimeConverter>();
            });
        }
    }

    public class RecmdTimeZoneInfoData : IFileSpecData
    {
        public string ValueName { get; set; }
        public string BatchKeyPath { get; set; }
        public string ValueData { get; set; }
        public string BatchValueName { get; set; }
        public string ValueDataRaw { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{ValueName} {BatchKeyPath} {ValueData} {BatchValueName} {ValueDataRaw}";
        }
    }

    /// <summary>
    ///     RECmd's CSV for TimeZoneInformation (RegistryPlugin.TimeZoneInformation.dll)
    /// </summary>
    public class RecmdTimeZoneInfo : IFileSpec
    {
        public RecmdTimeZoneInfo()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<RecmdTimeZoneInfoData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "ValueName,BatchKeyPath,ValueData,BatchValueName,ValueDataRaw"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from RECmd for the TimeZoneInfo registry plugin";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "29ff1f1b-938d-44f8-b626-9a87fe290fc2";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<RecmdTimeZoneInfoData>(filename, DataList, TaggedLines);
        }
    }

    public class RecmdTrustedDocumentsData : IFileSpecData
    {
        public string EventType { get; set; }
        public string BatchKeyPath { get; set; }
        public DateTime? Timestamp { get; set; }
        public string BatchValueName { get; set; }
        public string FileName { get; set; }
        public string Username { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{EventType} {BatchKeyPath} {Timestamp} {BatchValueName} {FileName} {Username}";
        }
    }

    /// <summary>
    ///     RECmd's CSV for TrustedDocuments (RegistryPlugin.TrustedDocuments.dll)
    /// </summary>
    public class RecmdTrustedDocuments : IFileSpec
    {
        public RecmdTrustedDocuments()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<RecmdTrustedDocumentsData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "EventType,BatchKeyPath,Timestamp,BatchValueName,FileName,Username"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from RECmd for the TrustedDocuments registry plugin";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "8803c9e1-4bc3-480b-a432-28bf23968bbc";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<RecmdTrustedDocumentsData>(filename, DataList, TaggedLines, map =>
            {
                map.Map(t => t.Timestamp).TypeConverter<LenientDateTimeConverter>();
            });
        }
    }

    public class RecmdTypedURLsData : IFileSpecData
    {
        public DateTime? Timestamp { get; set; }
        public string BatchKeyPath { get; set; }
        public string Url { get; set; }
        public string BatchValueName { get; set; }
        public string Slack { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{Timestamp} {BatchKeyPath} {Url} {BatchValueName} {Slack}";
        }
    }

    /// <summary>
    ///     RECmd's CSV for TypedURLs (RegistryPlugin.TypedURLs.dll)
    /// </summary>
    public class RecmdTypedURLs : IFileSpec
    {
        public RecmdTypedURLs()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<RecmdTypedURLsData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Timestamp,BatchKeyPath,Url,BatchValueName,Slack"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from RECmd for the TypedURLs registry plugin";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "d553da21-e182-4817-b102-87021cc8ac25";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<RecmdTypedURLsData>(filename, DataList, TaggedLines, map =>
            {
                map.Map(t => t.Timestamp).TypeConverter<LenientDateTimeConverter>();
            });
        }
    }

    public class RecmdUSBData : IFileSpecData
    {
        public string KeyName { get; set; }
        public string BatchKeyPath { get; set; }
        public string SerialNumber { get; set; }
        public string BatchValueName { get; set; }
        public string ParentidPrefix { get; set; }
        public string Service { get; set; }
        public string DeviceDesc { get; set; }
        public string FriendlyName { get; set; }
        public string DeviceName { get; set; }
        public string LocationInformation { get; set; }
        public DateTime? Installed { get; set; }
        public DateTime? FirstInstalled { get; set; }
        public DateTime? LastConnected { get; set; }
        public DateTime? LastRemoved { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{KeyName} {BatchKeyPath} {SerialNumber} {BatchValueName} {ParentidPrefix} {Service} {DeviceDesc} {FriendlyName} {DeviceName} {LocationInformation} {Installed} {FirstInstalled} {LastConnected} {LastRemoved}";
        }
    }

    /// <summary>
    ///     RECmd's CSV for USB (RegistryPlugin.USB.dll)
    /// </summary>
    public class RecmdUSB : IFileSpec
    {
        public RecmdUSB()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<RecmdUSBData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "KeyName,BatchKeyPath,SerialNumber,BatchValueName,ParentidPrefix,Service,DeviceDesc,FriendlyName,DeviceName,LocationInformation,Installed,FirstInstalled,LastConnected,LastRemoved"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from RECmd for the USB registry plugin";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "e0d8b778-c9ac-4bd9-9f08-0afb358aa660";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<RecmdUSBData>(filename, DataList, TaggedLines, map =>
            {
                map.Map(t => t.Installed).TypeConverter<LenientDateTimeConverter>();
                map.Map(t => t.FirstInstalled).TypeConverter<LenientDateTimeConverter>();
                map.Map(t => t.LastConnected).TypeConverter<LenientDateTimeConverter>();
                map.Map(t => t.LastRemoved).TypeConverter<LenientDateTimeConverter>();
            });
        }
    }

    public class RecmdUSBSTORData : IFileSpecData
    {
        public DateTime? Timestamp { get; set; }
        public string BatchKeyPath { get; set; }
        public string Manufacturer { get; set; }
        public string BatchValueName { get; set; }
        public string Title { get; set; }
        public string Version { get; set; }
        public string SerialNumber { get; set; }
        public string DeviceName { get; set; }
        public string DiskId { get; set; }
        public DateTime? Installed { get; set; }
        public DateTime? FirstInstalled { get; set; }
        public DateTime? LastConnected { get; set; }
        public DateTime? LastRemoved { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{Timestamp} {BatchKeyPath} {Manufacturer} {BatchValueName} {Title} {Version} {SerialNumber} {DeviceName} {DiskId} {Installed} {FirstInstalled} {LastConnected} {LastRemoved}";
        }
    }

    /// <summary>
    ///     RECmd's CSV for USBSTOR (RegistryPlugin.USBSTOR.dll)
    /// </summary>
    public class RecmdUSBSTOR : IFileSpec
    {
        public RecmdUSBSTOR()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<RecmdUSBSTORData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Timestamp,BatchKeyPath,Manufacturer,BatchValueName,Title,Version,SerialNumber,DeviceName,DiskId,Installed,FirstInstalled,LastConnected,LastRemoved"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from RECmd for the USBSTOR registry plugin";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "32f83eb0-a49b-476b-8b9a-6cb1afb941a5";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<RecmdUSBSTORData>(filename, DataList, TaggedLines, map =>
            {
                map.Map(t => t.Timestamp).TypeConverter<LenientDateTimeConverter>();
                map.Map(t => t.Installed).TypeConverter<LenientDateTimeConverter>();
                map.Map(t => t.FirstInstalled).TypeConverter<LenientDateTimeConverter>();
                map.Map(t => t.LastConnected).TypeConverter<LenientDateTimeConverter>();
                map.Map(t => t.LastRemoved).TypeConverter<LenientDateTimeConverter>();
            });
        }
    }

    public class RecmdUnInstallData : IFileSpecData
    {
        public DateTime? Timestamp { get; set; }
        public string BatchKeyPath { get; set; }
        public string KeyName { get; set; }
        public string BatchValueName { get; set; }
        public string DisplayName { get; set; }
        public string DisplayVersion { get; set; }
        public string Publisher { get; set; }
        public string InstallDate { get; set; }
        public string InstallSource { get; set; }
        public string InstallLocation { get; set; }
        public string UninstallString { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{Timestamp} {BatchKeyPath} {KeyName} {BatchValueName} {DisplayName} {DisplayVersion} {Publisher} {InstallDate} {InstallSource} {InstallLocation} {UninstallString}";
        }
    }

    /// <summary>
    ///     RECmd's CSV for Uninstall (RegistryPlugin.Uninstall.dll)
    /// </summary>
    public class RecmdUnInstall : IFileSpec
    {
        public RecmdUnInstall()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<RecmdUnInstallData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Timestamp,BatchKeyPath,KeyName,BatchValueName,DisplayName,DisplayVersion,Publisher,InstallDate,InstallSource,InstallLocation,UninstallString"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from RECmd for the UnInstall registry plugin";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "23c98e51-2431-429b-ae92-377435ddc912";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<RecmdUnInstallData>(filename, DataList, TaggedLines, map =>
            {
                map.Map(t => t.Timestamp).TypeConverter<LenientDateTimeConverter>();
            });
        }
    }

    public class RecmdUserAccountsData : IFileSpecData
    {
        public bool? ValidUserId { get; set; }
        public string BatchKeyPath { get; set; }
        public string BatchValueName { get; set; }
        public long? UserId { get; set; }
        public long? InvalidLoginCount { get; set; }
        public long? TotalLoginCount { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? LastLoginTime { get; set; }
        public DateTime? LastPasswordChange { get; set; }
        public DateTime? LastIncorrectPassword { get; set; }
        public DateTime? ExpiresOn { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string PasswordHint { get; set; }
        public string Groups { get; set; }
        public string Comment { get; set; }
        public string UserComment { get; set; }
        public string HomeDirectory { get; set; }
        public string InternetUserName { get; set; }
        public string ResetData { get; set; }
        public bool? AccountDisabled { get; set; }
        public bool? HomeDirectoryRequired { get; set; }
        public bool? PasswordNotRequired { get; set; }
        public bool? TempDuplicateAccount { get; set; }
        public bool? NormalUserAccount { get; set; }
        public bool? MnsLogonAccount { get; set; }
        public bool? InterdomainTrustAccount { get; set; }
        public bool? WorkstationTrustAccount { get; set; }
        public bool? ServerTrustAccount { get; set; }
        public bool? PasswordDoesNotExpire { get; set; }
        public bool? AutoLocked { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{ValidUserId} {BatchKeyPath} {BatchValueName} {UserId} {InvalidLoginCount} {TotalLoginCount} {CreatedOn} {LastLoginTime} {LastPasswordChange} {LastIncorrectPassword} {ExpiresOn} {UserName} {FullName} {PasswordHint} {Groups} {Comment} {UserComment} {HomeDirectory} {InternetUserName} {ResetData} {AccountDisabled} {HomeDirectoryRequired} {PasswordNotRequired} {TempDuplicateAccount} {NormalUserAccount} {MnsLogonAccount} {InterdomainTrustAccount} {WorkstationTrustAccount} {ServerTrustAccount} {PasswordDoesNotExpire} {AutoLocked}";
        }
    }

    /// <summary>
    ///     RECmd's CSV for User accounts (RegistryPlugin.SAM.dll)
    /// </summary>
    public class RecmdUserAccounts : IFileSpec
    {
        public RecmdUserAccounts()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<RecmdUserAccountsData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "ValidUserId,BatchKeyPath,BatchValueName,UserId,InvalidLoginCount,TotalLoginCount,CreatedOn,LastLoginTime,LastPasswordChange,LastIncorrectPassword,ExpiresOn,UserName,FullName,PasswordHint,Groups,Comment,UserComment,HomeDirectory,InternetUserName,ResetData,AccountDisabled,HomeDirectoryRequired,PasswordNotRequired,TempDuplicateAccount,NormalUserAccount,MnsLogonAccount,InterdomainTrustAccount,WorkstationTrustAccount,ServerTrustAccount,PasswordDoesNotExpire,AutoLocked"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from RECmd for the UserAccounts registry plugin";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "d127b1cd-2870-44ce-ac95-df3d4a30dea6";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<RecmdUserAccountsData>(filename, DataList, TaggedLines, map =>
            {
                map.Map(t => t.CreatedOn).TypeConverter<LenientDateTimeConverter>();
                map.Map(t => t.LastLoginTime).TypeConverter<LenientDateTimeConverter>();
                map.Map(t => t.LastPasswordChange).TypeConverter<LenientDateTimeConverter>();
                map.Map(t => t.LastIncorrectPassword).TypeConverter<LenientDateTimeConverter>();
                map.Map(t => t.ExpiresOn).TypeConverter<LenientDateTimeConverter>();
            });
        }
    }

    public class RecmdUserAssistData : IFileSpecData
    {
        public string BatchKeyPath { get; set; }
        public string BatchValueName { get; set; }
        public string ProgramName { get; set; }
        public long? RunCounter { get; set; }
        public long? FocusCount { get; set; }
        public string FocusTime { get; set; }
        public DateTime? LastExecuted { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{BatchKeyPath} {BatchValueName} {ProgramName} {RunCounter} {FocusCount} {FocusTime} {LastExecuted}";
        }
    }

    /// <summary>
    ///     RECmd's CSV for UserAssist (RegistryPlugin.UserAssist.dll)
    /// </summary>
    public class RecmdUserAssist : IFileSpec
    {
        public RecmdUserAssist()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<RecmdUserAssistData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "BatchKeyPath,BatchValueName,ProgramName,RunCounter,FocusCount,FocusTime,LastExecuted"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from RECmd for the UserAssist registry plugin";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "214a8d2a-defc-4866-a575-a046a0ef1312";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<RecmdUserAssistData>(filename, DataList, TaggedLines, map =>
            {
                map.Map(t => t.LastExecuted).TypeConverter<LenientDateTimeConverter>();
            });
        }
    }

    public class RecmdVolumeInfoCacheData : IFileSpecData
    {
        public DateTime? Timestamp { get; set; }
        public string BatchKeyPath { get; set; }
        public string DriveName { get; set; }
        public string BatchValueName { get; set; }
        public string VolumeLabel { get; set; }
        public string DriveType { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{Timestamp} {BatchKeyPath} {DriveName} {BatchValueName} {VolumeLabel} {DriveType}";
        }
    }

    /// <summary>
    ///     RECmd's CSV for VolumeInfoCache (RegistryPlugin.VolumeInfoCache.dll)
    /// </summary>
    public class RecmdVolumeInfoCache : IFileSpec
    {
        public RecmdVolumeInfoCache()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<RecmdVolumeInfoCacheData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Timestamp,BatchKeyPath,DriveName,BatchValueName,VolumeLabel,DriveType"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from RECmd for the VolumeInfoCache registry plugin";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "089239e2-7864-4d30-ac1f-36c8ca2944d7";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<RecmdVolumeInfoCacheData>(filename, DataList, TaggedLines, map =>
            {
                map.Map(t => t.Timestamp).TypeConverter<LenientDateTimeConverter>();
            });
        }
    }

    public class RecmdWindowsAppData : IFileSpecData
    {
        public string KeyName { get; set; }
        public string BatchKeyPath { get; set; }
        public string DisplayName { get; set; }
        public string BatchValueName { get; set; }
        public DateTime? InstallTime { get; set; }
        public string PackageRootFolder { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{KeyName} {BatchKeyPath} {DisplayName} {BatchValueName} {InstallTime} {PackageRootFolder}";
        }
    }

    /// <summary>
    ///     RECmd's CSV for Windows App (RegistryPlugin.WindowsApp.dll)
    /// </summary>
    public class RecmdWindowsApp : IFileSpec
    {
        public RecmdWindowsApp()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<RecmdWindowsAppData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "KeyName,BatchKeyPath,DisplayName,BatchValueName,InstallTime,PackageRootFolder"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from RECmd for the WindowsApp registry plugin";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "e6c25f21-5724-4877-97b3-e1a51e7919f2";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<RecmdWindowsAppData>(filename, DataList, TaggedLines, map =>
            {
                map.Map(t => t.InstallTime).TypeConverter<LenientDateTimeConverter>();
            });
        }
    }

    public class RecmdWindowsPortableDevicesData : IFileSpecData
    {
        public DateTime? Timestamp { get; set; }
        public string BatchKeyPath { get; set; }
        public string Device { get; set; }
        public string BatchValueName { get; set; }
        public string SerialNumber { get; set; }
        public string Guid { get; set; }
        public string FriendlyName { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{Timestamp} {BatchKeyPath} {Device} {BatchValueName} {SerialNumber} {Guid} {FriendlyName}";
        }
    }

    /// <summary>
    ///     RECmd's CSV for Windows Portable Devices (RegistryPlugin.WindowsPortableDevices.dll)
    /// </summary>
    public class RecmdWindowsPortableDevices : IFileSpec
    {
        public RecmdWindowsPortableDevices()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<RecmdWindowsPortableDevicesData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Timestamp,BatchKeyPath,Device,BatchValueName,SerialNumber,Guid,FriendlyName"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from RECmd for the WindowsPortableDevices registry plugin";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "0f53345a-bfc9-4e93-ae97-0d0e90218fa2";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<RecmdWindowsPortableDevicesData>(filename, DataList, TaggedLines, map =>
            {
                map.Map(t => t.Timestamp).TypeConverter<LenientDateTimeConverter>();
            });
        }
    }

    public class RecmdWordWheelQueryData : IFileSpecData
    {
        public string SearchTerm { get; set; }
        public string BatchKeyPath { get; set; }
        public long? MruPosition { get; set; }
        public string BatchValueName { get; set; }
        public string KeyName { get; set; }
        public DateTime? LastWriteTimestamp { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{SearchTerm} {BatchKeyPath} {MruPosition} {BatchValueName} {KeyName} {LastWriteTimestamp}";
        }
    }

    /// <summary>
    ///     RECmd's CSV for WordWheelQuery (RegistryPlugin.WordWheelQuery.dll)
    /// </summary>
    public class RecmdWordWheelQuery : IFileSpec
    {
        public RecmdWordWheelQuery()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<RecmdWordWheelQueryData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "SearchTerm,BatchKeyPath,MruPosition,BatchValueName,KeyName,LastWriteTimestamp"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from RECmd for the WordWheelQuery registry plugin";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "3ae35795-fb94-4cb9-972a-d7ded47a3297";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<RecmdWordWheelQueryData>(filename, DataList, TaggedLines, map =>
            {
                map.Map(t => t.LastWriteTimestamp).TypeConverter<LenientDateTimeConverter>();
            });
        }
    }
}
