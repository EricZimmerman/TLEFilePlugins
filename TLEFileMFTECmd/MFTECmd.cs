using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.TypeConversion;
using ITLEFileSpec;

namespace MFTECmd
{
    public class BootOutData : IFileSpecData
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
    ///     contains is defined in BootOutData.
    /// </summary>
    public class BootOut : IFileSpec
    {
        public BootOut()
        {
            //Initialize collections here, one for TaggedLines TLE can add values to, and the collection that TLE will display
            TaggedLines = new List<int>();

            DataList = new BindingList<BootOutData>();

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
                csv.Configuration.Delimiter = ",";
                var o = new TypeConverterOptions
                {
                    DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal
                };
                csv.Configuration.TypeConverterOptionsCache.AddOptions<BootOutData>(o);

                var foo = csv.Configuration.AutoMap<BootOutData>();

                foo.Map(t => t.Line).Ignore();
                foo.Map(t => t.Tag).Ignore();

                csv.Configuration.RegisterClassMap(foo);

                var records = csv.GetRecords<BootOutData>();

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

    public class SdsOutData : IFileSpecData
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

    public class SdsOut : IFileSpec
    {
        public SdsOut()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<SdsOutData>();

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

        public void ProcessFile(string filename)
        {
            DataList.Clear();
            using (var fileReader = File.OpenText(filename))
            {
                var csv = new CsvReader(fileReader, CultureInfo.InvariantCulture);
                csv.Configuration.HasHeaderRecord = true;
                csv.Configuration.Delimiter = ",";
                var o = new TypeConverterOptions
                {
                    DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal
                };
                csv.Configuration.TypeConverterOptionsCache.AddOptions<SdsOutData>(o);

                var foo = csv.Configuration.AutoMap<SdsOutData>();

                foo.Map(t => t.Line).Ignore();
                foo.Map(t => t.Tag).Ignore();

                csv.Configuration.RegisterClassMap(foo);

                var records = csv.GetRecords<SdsOutData>();

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

    public class JOutData : IFileSpecData
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

    public class JOut : IFileSpec
    {
        public JOut()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<JOutData>();

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

        public void ProcessFile(string filename)
        {
            DataList.Clear();

            using (var fileReader = File.OpenText(filename))
            {
                var csv = new CsvReader(fileReader, CultureInfo.InvariantCulture);
                csv.Configuration.HasHeaderRecord = true;
                csv.Configuration.Delimiter = ",";
                var o = new TypeConverterOptions
                {
                    DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal
                };
                csv.Configuration.TypeConverterOptionsCache.AddOptions<JOutData>(o);

                var foo = csv.Configuration.AutoMap<JOutData>();

                foo.Map(m => m.UpdateTimestamp).TypeConverterOption
                    .DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);

                foo.Map(t => t.Line).Ignore();
                foo.Map(t => t.Tag).Ignore();

                csv.Configuration.RegisterClassMap(foo);

                var records = csv.GetRecords<JOutData>();

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


    public class MFTOutData : IFileSpecData
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

    public class MFTOut : IFileSpec
    {
        public MFTOut()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<MFTOutData>();

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

        public void ProcessFile(string filename)
        {
            DataList.Clear();

            using (var fileReader = File.OpenText(filename))
            {
                var csv = new CsvReader(fileReader, CultureInfo.InvariantCulture);
                csv.Configuration.HasHeaderRecord = true;
                csv.Configuration.Delimiter = ",";
                var foo = csv.Configuration.AutoMap<MFTOutData>();

                var o = new TypeConverterOptions
                {
                    DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal,
                    NullValues = {"=\"\""}
                };
                csv.Configuration.TypeConverterOptionsCache.AddOptions<MFTOutData>(o);

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

                var records = csv.GetRecords<MFTOutData>();

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