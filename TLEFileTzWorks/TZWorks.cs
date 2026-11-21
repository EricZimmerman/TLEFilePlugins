using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using CsvHelper;
using ITLEFileSpec;
using Serilog;
using TLEFilePlugins.Shared;
using Serilog.Events;
using ServiceStack;

namespace TLEFileTzWorks
{
    public class PeScanOutData : IFileSpecData
    {
        public DateTime? CompiledTime { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime AccessedTime { get; set; }
        public DateTime ModifiedTime { get; set; }

        public int Size { get; set; }
        public string FileType { get; set; }
        public string CPU { get; set; }

        public float Linker { get; set; }
        public string CodeSize { get; set; }
        public string InitData { get; set; }
        public string UInitData { get; set; }
        public string ImageVer { get; set; }
        public string SubsysVer { get; set; }
        public string MinimumOS { get; set; }

        public int EntryOva { get; set; }

        public int EntryFAddress { get; set; }
        public ulong ImageBase { get; set; }
        public bool Signed { get; set; }
        public bool CheckSum { get; set; }

        public string MD5 { get; set; }
        public string Company { get; set; }
        public string FilePath { get; set; }

        public int Rating { get; set; }
        public string Notes { get; set; }
        public string PeId { get; set; }
        public string SymbolFile { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return
                $"{CompiledTime} {CreatedTime} {AccessedTime} {ModifiedTime} {Size} {FileType} {CPU} {Linker} {CodeSize} {InitData} {UInitData} {ImageVer} {SubsysVer} {MinimumOS} {EntryOva} {EntryFAddress} {ImageBase} {Signed} {CheckSum} {MD5} {Company} {FilePath} {Rating} {Notes} {PeId} {SymbolFile}";
        }
    }


    public class PeScan : IFileSpec
    {
        public PeScan()
        {
            //Initialize collections here, one for TaggedLines TLE can add values to, and the collection that TLE will display
            TaggedLines = new List<int>();

            DataList = new BindingList<PeScanOutData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "compiled,time-UTC,created,time-UTC,access,time-UTC,modify,time-UTC,size,type,cpu,linker,code size,init data,uninit data,image ver,subsys ver,min OS,entry rva,entryfaddr,imagebase,cert,chksum,md5,company,file,rating,notes,PEiD,symbol file"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from pescan";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "30877d57-540d-4460-bc4e-859fee4b0bbc";

        public void ProcessFile(string filename)
        {
            //(TaggedLines.Contains was a list scan for every row)
            var tagged = new HashSet<int>(TaggedLines);

            DataList.Clear();

            //fields by index: the header is read as a record and skipped (comma separated: the preamble would confuse a
            //detected separator); bad quoting is recovered from
            using var csv = CsvReader.FromFile(filename, CsvLoader.Options.With((nameof(CsvReaderOptions.HasHeader), false)));

            //the header line
            csv.Read();

            if (csv.Current.ToString().StartsWith("pescan - "))
            {
                //dumb ass non-csv, so skip it: the header is the fifth record
                csv.Read();
                csv.Read();
                csv.Read();
                csv.Read();
            }

        

            var ln11 = 1;
            //(RawRecord was built for every row even with debug messages off)
            var debug = Log.IsEnabled(LogEventLevel.Debug);

            //one reset at the end instead of a ListChanged event for every row
            using var quiet = CsvLoader.QuietList<PeScanOutData>(DataList);

            while (csv.Read())
            {
                if (debug)
                {
                    Log.Debug("Line # {Line}, Record: {Record}", ln11, csv.Current.ToString());
                }

                var compiled = csv.GetString(0);
                var compTime = csv.GetString(1);
                var created = csv.GetString(2);
                var createdTime = csv.GetString(3);
                var accessd = csv.GetString(4);
                var accessedTime = csv.GetString(5);
                var modified = csv.GetString(6);
                var modTime = csv.GetString(7);
                var size = csv.GetString(8);
                var fileType = csv.GetString(9);
                var cpu = csv.GetString(10);
                var linker = csv.GetString(11);
                var codeSize = csv.GetString(12);
                var initData = csv.GetString(13);
                var uinitData = csv.GetString(14);
                var imageVer = csv.GetString(15);
                var subsysVer = csv.GetString(16);
                var minOs = csv.GetString(17);
                var entryRva = csv.GetString(18);
                var entryfAddr = csv.GetString(19);
                var imagebase = csv.GetString(20);
                var cert = csv.GetString(21);
                var checksum = csv.GetString(22);
                var md5 = csv.GetString(23);
                var company = csv.GetString(24);
                var file = csv.GetString(25);
                var rating = csv.GetString(26);
                var notes = csv.GetString(27);
                var peid = csv.GetString(28);
                var symbolFile = csv.GetString(29);

                var pe = new PeScanOutData {Line = ln11};
                pe.Tag = tagged.Contains(ln11);
                if (compiled.Length > 0)
                {
                    pe.CompiledTime = DateTime.Parse($"{compiled} {compTime}", CultureInfo.InvariantCulture,
                            DateTimeStyles.AssumeUniversal)
                        .ToUniversalTime();
                }

                pe.CreatedTime = DateTime.Parse($"{created} {createdTime}", CultureInfo.InvariantCulture,
                        DateTimeStyles.AssumeUniversal)
                    .ToUniversalTime();
                pe.AccessedTime = DateTime.Parse($"{accessd} {accessedTime}", CultureInfo.InvariantCulture,
                        DateTimeStyles.AssumeUniversal)
                    .ToUniversalTime();
                pe.ModifiedTime = DateTime
                    .Parse($"{modified} {modTime}", null, DateTimeStyles.AssumeUniversal)
                    .ToUniversalTime();

                pe.Size = int.Parse(size);
                pe.FileType = fileType;
                pe.CPU = cpu;
                if (linker.Length > 0)
                {
                    pe.Linker = float.Parse(linker);
                }

                pe.CodeSize = codeSize;
                pe.InitData = initData;
                pe.UInitData = uinitData;
                pe.ImageVer = imageVer;
                pe.SubsysVer = subsysVer;
                pe.SymbolFile = symbolFile;
                pe.PeId = peid;

                pe.MinimumOS = minOs;
                if (entryRva.Length > 0)
                {
                    pe.EntryOva = int.Parse(entryRva);
                }

                if (entryfAddr.Length > 0)
                {
                    pe.EntryFAddress = int.Parse(entryfAddr);
                }

                if (imagebase.Length > 0)
                {
                    pe.ImageBase = ulong.Parse(imagebase);
                }

                pe.Signed = cert.Equals("yes");
                pe.CheckSum = checksum.Equals("yes");
                pe.MD5 = md5;
                pe.Company = company;
                pe.FilePath = file;
                pe.Rating = int.Parse(rating);
                pe.Notes = notes;

                DataList.Add(pe);
                ln11 += 1;
            }
        }
    }

    public class WispOutData : IFileSpecData
    {
        public ulong? MftEntry { get; set; }
        public ulong? MftSequence { get; set; }
        public ulong ParentEntry { get; set; }
        public ulong ParentSequence { get; set; }
        public string EntryType { get; set; }
        public DateTime? AccessedTime { get; set; }
        public DateTime? ModifiedTime { get; set; }
        public DateTime? CreatedTime { get; set; }
        public DateTime? RecordChangedTime { get; set; }
        public string FileType { get; set; }
        public ulong SizeReserved { get; set; }
        public ulong SizeUsed { get; set; }
        public string Flags { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }


        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return
                $"{MftEntry} {MftSequence} {ParentEntry} {ParentSequence} {EntryType} {AccessedTime} {ModifiedTime} {CreatedTime} {RecordChangedTime} {FileType} {SizeReserved} {SizeUsed} {Flags} {Name} {Comment}";
        }
    }

    public class Wisp : IFileSpec
    {
        public Wisp()
        {
            //Initialize collections here, one for TaggedLines TLE can add values to, and the collection that TLE will display
            TaggedLines = new List<int>();

            DataList = new BindingList<WispOutData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "mft entry,mft seq,parent mft,seqnum,type,file mdate,time-UTC,file adate,time-UTC,mftdate,time-UTC,file cdate,time-UTC,dir/file,size resv,size used,flags,name,slack comment"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from Wisp";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "30877d57-120e-4460-bc4e-859fee4b0aac";

        public void ProcessFile(string filename)
        {
            //(TaggedLines.Contains was a list scan for every row)
            var tagged = new HashSet<int>(TaggedLines);

            DataList.Clear();

            int rawFlag;
            //fields by index: the header is read as a record and skipped (comma separated: the preamble would confuse a
            //detected separator); bad quoting is recovered from
            using var csv = CsvReader.FromFile(filename, CsvLoader.Options.With((nameof(CsvReaderOptions.HasHeader), false)));

            //the header line
            csv.Read();

            if (csv.Current.ToString().StartsWith("wisp - "))
            {
                //dumb ass non-csv, so skip it: the header is the fifth record
                csv.Read();
                csv.Read();
                csv.Read();
                csv.Read();
            }
                

            var ln11 = 1;
            //(RawRecord was built for every row even with debug messages off)
            var debug = Log.IsEnabled(LogEventLevel.Debug);

            //one reset at the end instead of a ListChanged event for every row
            using var quiet = CsvLoader.QuietList<WispOutData>(DataList);

            while (csv.Read())
            {
                if (debug)
                {
                    Log.Debug("Line # {Line}, Record: {RawRecord}", ln11, csv.Current.ToString());
                }
                
                var mftEntry = csv.GetString(0);
                var mftSeq = csv.GetString(1);
                var parentMftEntry = csv.GetString(2);
                var parentMftSeq = csv.GetString(3);
                var entryType = csv.GetString(4);
                var modified = csv.GetString(5);
                var modTime = csv.GetString(6);
                var accessd = csv.GetString(7);
                var accessedTime = csv.GetString(8);
                var recordChanged = csv.GetString(9);
                var recordChangedTime = csv.GetString(10);
                var created = csv.GetString(11);
                var createTime = csv.GetString(12);

                var fileType = csv.GetString(13);
                var sizeRes = csv.GetString(14);
                var sizeAct = csv.GetString(15);
                var flags = csv.GetString(16);
                var name = csv.GetString(17);
                var comment = csv.GetString(18);


                var wi = new WispOutData
                {
                    Line = ln11
                };

                wi.Tag = tagged.Contains(ln11);

                if (created.IsNullOrEmpty() == false)
                {
                    wi.CreatedTime = DateTime.Parse($"{created} {createTime}", CultureInfo.InvariantCulture,
                            DateTimeStyles.AssumeUniversal)
                        .ToUniversalTime();
                }

                if (recordChanged.IsNullOrEmpty() == false)
                {
                    wi.RecordChangedTime = DateTime.Parse($"{recordChanged} {recordChangedTime}",
                            CultureInfo.InvariantCulture,
                            DateTimeStyles.AssumeUniversal)
                        .ToUniversalTime();
                }

                if (accessd.IsNullOrEmpty() == false)
                {
                    wi.AccessedTime = DateTime.Parse($"{accessd} {accessedTime}", CultureInfo.InvariantCulture,
                            DateTimeStyles.AssumeUniversal)
                        .ToUniversalTime();
                }

                if (modified.IsNullOrEmpty() == false)
                {
                    wi.ModifiedTime = DateTime
                        .Parse($"{modified} {modTime}", null, DateTimeStyles.AssumeUniversal)
                        .ToUniversalTime();
                }

                if (mftEntry.Length > 0)
                {
                    wi.MftEntry = ulong.Parse(mftEntry);
                }

                if (mftSeq.Length > 0)
                {
                    wi.MftSequence = ulong.Parse(mftSeq);
                }

                if (parentMftEntry.Length > 0)
                {
                    wi.ParentEntry = ulong.Parse(parentMftEntry);
                }

                if (parentMftSeq.Length > 0)
                {
                    wi.ParentSequence = ulong.Parse(parentMftSeq);
                }

                wi.EntryType = entryType;
                if (fileType == "dir")
                {
                    wi.FileType = "Directory";
                }
                else
                {
                    wi.FileType = "File";
                }


                if (sizeAct.Length > 0)
                {
                    wi.SizeUsed = ulong.Parse(sizeAct);
                }

                if (sizeRes.Length > 0)
                {
                    wi.SizeReserved = ulong.Parse(sizeRes);
                }

                if (flags.Length > 0)
                {
                    var rawNum = flags.Replace("0x", "");


                    var valOk = int.TryParse(rawNum, NumberStyles.HexNumber, CultureInfo.InvariantCulture,
                        out rawFlag);

                    if (valOk)
                    {
                        wi.Flags = ((FileFlag) rawFlag).ToString().Replace(", ", " | ");
                    }
                }

                wi.Name = name;
                wi.Comment = comment;


                DataList.Add(wi);
                ln11 += 1;
            }
        }
    }

    [Flags]
    public enum FileFlag
    {
        ReadOnly = 0x01,
        Hidden = 0x02,
        System = 0x04,
        VolumeLabel = 0x08,
        Directory = 0x010,
        Archive = 0x020,
        Device = 0x040,
        Normal = 0x080,
        Temporary = 0x0100,
        SparseFile = 0x0200,
        ReparsePoint = 0x0400,
        Compressed = 0x0800,
        Offline = 0x01000,
        NotContentIndexed = 0x02000,
        Encrypted = 0x04000,
        IntegrityStream = 0x08000,
        Virtual = 0x010000,
        NoScrubData = 0x020000,
        HasEa = 0x040000,
        IsDirectory = 0x10000000,
        IsIndexView = 0x20000000
    }
}