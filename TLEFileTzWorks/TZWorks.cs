using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.TypeConversion;
using ITLEFileSpec;
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
            DataList.Clear();

            using (var fileReader = File.OpenText(filename))
            {
                var csv = new CsvReader(fileReader, CultureInfo.InvariantCulture);
                csv.Configuration.HasHeaderRecord = true;

                var o = new TypeConverterOptions
                {
                    DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal
                };
                csv.Configuration.TypeConverterOptionsCache.AddOptions<PeScanOutData>(o);


                csv.Read();

                if (csv.Context.RawRecord.StartsWith("pescan - "))
                {
                    //dumb ass non-csv, so skip it
                    csv.Read();
                    csv.Read();
                    csv.Read();
                    csv.Read();
                }


                csv.ReadHeader();

                csv.Configuration.BadDataFound = null;

                var ln11 = 1;
                while (csv.Read())
                {
                    var compiled = csv.GetField(0);
                    var compTime = csv.GetField(1);
                    var created = csv.GetField(2);
                    var createdTime = csv.GetField(3);
                    var accessd = csv.GetField(4);
                    var accessedTime = csv.GetField(5);
                    var modified = csv.GetField(6);
                    var modTime = csv.GetField(7);
                    var size = csv.GetField(8);
                    var fileType = csv.GetField(9);
                    var cpu = csv.GetField(10);
                    var linker = csv.GetField(11);
                    var codeSize = csv.GetField(12);
                    var initData = csv.GetField(13);
                    var uinitData = csv.GetField(14);
                    var imageVer = csv.GetField(15);
                    var subsysVer = csv.GetField(16);
                    var minOs = csv.GetField(17);
                    var entryRva = csv.GetField(18);
                    var entryfAddr = csv.GetField(19);
                    var imagebase = csv.GetField(20);
                    var cert = csv.GetField(21);
                    var checksum = csv.GetField(22);
                    var md5 = csv.GetField(23);
                    var company = csv.GetField(24);
                    var file = csv.GetField(25);
                    var rating = csv.GetField(26);
                    var notes = csv.GetField(27);
                    var peid = csv.GetField(28);
                    var symbolFile = csv.GetField(29);

                    var pe = new PeScanOutData {Line = ln11};
                    pe.Tag = TaggedLines.Contains(ln11);
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
            DataList.Clear();

            int rawFlag;
            using (var fileReader = File.OpenText(filename))
            {
                var csv = new CsvReader(fileReader, CultureInfo.InvariantCulture);
                csv.Configuration.HasHeaderRecord = true;

                var o = new TypeConverterOptions
                {
                    DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal
                };
                csv.Configuration.TypeConverterOptionsCache.AddOptions<WispOutData>(o);

                csv.Read();

                if (csv.Context.RawRecord.StartsWith("wisp - "))
                {
                    //dumb ass non-csv, so skip it
                    csv.Read();
                    csv.Read();
                    csv.Read();
                    csv.Read();
                }

                csv.ReadHeader();

                var ln11 = 1;
                while (csv.Read())
                {
                    var mftEntry = csv.GetField(0);
                    var mftSeq = csv.GetField(1);
                    var parentMftEntry = csv.GetField(2);
                    var parentMftSeq = csv.GetField(3);
                    var entryType = csv.GetField(4);
                    var modified = csv.GetField(5);
                    var modTime = csv.GetField(6);
                    var accessd = csv.GetField(7);
                    var accessedTime = csv.GetField(8);
                    var recordChanged = csv.GetField(9);
                    var recordChangedTime = csv.GetField(10);
                    var created = csv.GetField(11);
                    var createTime = csv.GetField(12);

                    var fileType = csv.GetField(13);
                    var sizeRes = csv.GetField(14);
                    var sizeAct = csv.GetField(15);
                    var flags = csv.GetField(16);
                    var name = csv.GetField(17);
                    var comment = csv.GetField(18);


                    var wi = new WispOutData
                    {
                        Line = ln11
                    };

                    wi.Tag = TaggedLines.Contains(ln11);

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