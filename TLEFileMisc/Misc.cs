using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using ITLEFileSpec;
using Serilog;

namespace TLEFileMisc
{

    #region Hayabusa

    
    public class HayabusaStandardData : IFileSpecData
    {
        public DateTime Timestamp { get; set; }
        public string RuleTitle { get; set; }
        public string Level { get; set; }
        public string Computer { get; set; }
        public string Channel { get; set; }
        
        public int EventId { get; set; }
        public int RecordId { get; set; }
        public string Details { get; set; }
        public string ExtraFieldInfo { get; set; }
        
        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{Timestamp} {RuleTitle} {Level} {Computer} {Channel} {EventId} {RecordId} {Details} {ExtraFieldInfo}";
        }
    }

    public class HayabusaStandard: IFileSpec
    {
        public HayabusaStandard()
        {
            //Initialize collections here, one for TaggedLines TLE can add values to, and the collection that TLE will display
            TaggedLines = new List<int>();

            DataList = new BindingList<HayabusaStandardData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "\"Timestamp\",\"RuleTitle\",\"Level\",\"Computer\",\"Channel\",\"EventID\",\"RecordID\",\"Details\",\"ExtraFieldInfo\""
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from Hayabusa Standard";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "33de9217-f9b5-4d91-9324-1a17b03d44df";

        public void ProcessFile(string filename)
        {
            DataList.Clear();

            using var fileReader = File.OpenText(filename);
            var csv = new CsvReader(fileReader, CultureInfo.InvariantCulture);

            var foo = csv.Context.AutoMap<HayabusaStandardData>();

            var o = new TypeConverterOptions
            {
                DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal
            };
            csv.Context.TypeConverterOptionsCache.AddOptions<HayabusaStandardData>(o);

            foo.Map(m=>m.Timestamp).Convert(row => DateTime.Parse(row.Row.GetField<string>("Timestamp")).ToUniversalTime());
            foo.Map(t => t.EventId).Name("EventID");
            foo.Map(t => t.RecordId).Name("RecordID");
            
            foo.Map(t => t.Line).Ignore();
            foo.Map(t => t.Tag).Ignore();

            csv.Context.RegisterClassMap(foo);

            var records = csv.GetRecords<HayabusaStandardData>();

            var ln = 1;
            foreach (var record in records)
            {
                Log.Debug("Line # {Line}, Record: {RawRecord}", ln, csv.Context.Parser.RawRecord);

                record.Line = ln;

                record.Tag = TaggedLines.Contains(ln);

                DataList.Add(record);

                ln += 1;
            }
        }
        
    }
    
    
    public class HayabusaVerboseData : IFileSpecData
    {
        
        public DateTime Timestamp { get; set; }
        public string RuleTitle { get; set; }
        public string Level { get; set; }
        public string Computer { get; set; }
        public string Channel { get; set; }
        
        public int EventId { get; set; }
        
        public string MitreTactics { get; set; }
        public string MitreTags { get; set; }
        public string OtherTags { get; set; }
        
        public int RecordId { get; set; }
        public string Details { get; set; }
        public string ExtraFieldInfo { get; set; }
        public string RuleFile { get; set; }
        public string EvtxFile { get; set; }
        
        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{Timestamp} {RuleTitle} {Level} {Computer} {Channel} {EventId} {MitreTactics} {MitreTags} {OtherTags} {RecordId} {Details} {ExtraFieldInfo} {RuleFile} {EvtxFile}";
        }
    }

    public class HayabusaVerbose : IFileSpec
    {
        public HayabusaVerbose()
        {
            //Initialize collections here, one for TaggedLines TLE can add values to, and the collection that TLE will display
            TaggedLines = new List<int>();

            DataList = new BindingList<HayabusaVerboseData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "\"Timestamp\",\"RuleTitle\",\"Level\",\"Computer\",\"Channel\",\"EventID\",\"MitreTactics\",\"MitreTags\",\"OtherTags\",\"RecordID\",\"Details\",\"ExtraFieldInfo\",\"RuleFile\",\"EvtxFile\""
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from Hayabusa Verbose";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "72de9223-f9b5-4d91-9324-0a17b03d22df";

        public void ProcessFile(string filename)
        {
            DataList.Clear();

            using var fileReader = File.OpenText(filename);
            var csv = new CsvReader(fileReader, CultureInfo.InvariantCulture);

            var foo = csv.Context.AutoMap<HayabusaVerboseData>();

            var o = new TypeConverterOptions
            {
                DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal
            };
            csv.Context.TypeConverterOptionsCache.AddOptions<HayabusaVerboseData>(o);

            foo.Map(t => t.EventId).Name("EventID");
            foo.Map(t => t.RecordId).Name("RecordID");

            foo.Map(m=>m.Timestamp).Convert(row => DateTime.Parse(row.Row.GetField<string>("Timestamp")).ToUniversalTime());
            foo.Map(t => t.Line).Ignore();
            foo.Map(t => t.Tag).Ignore();

            csv.Context.RegisterClassMap(foo);

            var records = csv.GetRecords<HayabusaVerboseData>();

            var ln = 1;
            foreach (var record in records)
            {
                Log.Debug("Line # {Line}, Record: {RawRecord}", ln, csv.Context.Parser.RawRecord);

                record.Line = ln;

                record.Tag = TaggedLines.Contains(ln);

                DataList.Add(record);

                ln += 1;
            }
        }
        
    }
    
    
    public class HayabusaSuperVerboseData : IFileSpecData
    {
        //"EventID","RuleAuthor","RuleModifiedDate","Status",
        //"ExtraFieldInfo","MitreTactics","MitreTags","OtherTags","Provider","RuleCreationDate","RuleFile","EvtxFile"
        public DateTime Timestamp { get; set; }
        public string RuleTitle { get; set; }
        public string Level { get; set; }
        public string Computer { get; set; }
        public string Channel { get; set; }
        
        public int EventId { get; set; }
        
        public string RuleAuthor { get; set; }
        public string RuleModifiedDate { get; set; }
        public string Status { get; set; }
        public int RecordId { get; set; }
        public string Details { get; set; }
        public string ExtraFieldInfo { get; set; }
        public string MitreTactics { get; set; }
        public string MitreTags { get; set; }
        public string OtherTags { get; set; }
        public string Provider { get; set; }
        public string RuleCreationDate { get; set; }
        public string RuleFile { get; set; }
        public string EvtxFile { get; set; }
        
        //"2020-07-09 21:22:31.163 +00:00","Active Directory User Backdoors","high","rootdc1.offsec.lan","Sec",5136,"@neu5ron",
        //"2024/02/26","test",15779594,"User: lambda-user ¦ SID: S-1-5-21-4230534742-2542757381-3142984815-1158 ¦ ObjDN: CN=honey-pot1,OU=Test-OU,OU=OFFSEC-COMPANY,DC=offsec,DC=lan ¦ AttrLDAPName: servicePrincipalName ¦ OpType: %%14674 ¦ LID: 0x6529663","AppCorrelationID: - ¦ AttributeSyntaxOID: 2.5.5.12 ¦ AttributeValue: HTTP/HACK-ME-PC.offsec.lan ¦ DSName: offsec.lan ¦ DSType: %%14676 ¦ ObjectClass: user ¦ ObjectGUID: 259162F1-58E4-4EE9-9B9C-2BAF2A03D376 ¦ OpCorrelationID: 52BFBF59-4CF4-4D5E-98D1-09D1EBE12FDE ¦ SubjectDomainName: OFFSEC",
        //"Persis","T1098","","Sec","2017/04/13","win_security_alert_ad_user_backdoors.yml","C:\Users\KOHDA\Downloads\tmp\ID4738,5136-SPN set on user account.evtx"
        
        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{Timestamp} {RuleTitle} {Level} {Computer} {Channel} {EventId} {RecordId} {Details}";
        }
    }

    public class HayabusaSuperVerbose : IFileSpec
    {
        public HayabusaSuperVerbose()
        {
            //Initialize collections here, one for TaggedLines TLE can add values to, and the collection that TLE will display
            TaggedLines = new List<int>();

            DataList = new BindingList<HayabusaSuperVerboseData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "\"Timestamp\",\"RuleTitle\",\"Level\",\"Computer\",\"Channel\",\"EventID\",\"RuleAuthor\",\"RuleModifiedDate\",\"Status\",\"RecordID\",\"Details\",\"ExtraFieldInfo\",\"MitreTactics\",\"MitreTags\",\"OtherTags\",\"Provider\",\"RuleCreationDate\",\"RuleFile\",\"EvtxFile\""
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from Hayabusa Super Verbose";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "72de8888-f9b5-4d91-9324-0b17b03d44df";

        public void ProcessFile(string filename)
        {
            DataList.Clear();

            using var fileReader = File.OpenText(filename);
            var csv = new CsvReader(fileReader, CultureInfo.InvariantCulture);

            var foo = csv.Context.AutoMap<HayabusaSuperVerboseData>();

            var o = new TypeConverterOptions
            {
                DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal
            };
            csv.Context.TypeConverterOptionsCache.AddOptions<HayabusaSuperVerboseData>(o);

            foo.Map(t => t.EventId).Name("EventID");
            foo.Map(t => t.RecordId).Name("RecordID");
            
          //  foo.Map(m => m.RuleModifiedDate).TypeConverter<StupidDateConverter>();
            foo.Map(m=>m.Timestamp).Convert(row => DateTime.Parse(row.Row.GetField<string>("Timestamp")).ToUniversalTime());
            foo.Map(t => t.Line).Ignore();
            foo.Map(t => t.Tag).Ignore();

            csv.Context.RegisterClassMap(foo);

            var records = csv.GetRecords<HayabusaSuperVerboseData>();

            var ln = 1;
            foreach (var record in records)
            {
                Log.Debug("Line # {Line}, Record: {RawRecord}", ln, csv.Context.Parser.RawRecord);

                record.Line = ln;

                record.Tag = TaggedLines.Contains(ln);
               
                DataList.Add(record);

                ln += 1;
            }
        }
        
        public class StupidDateConverter : DefaultTypeConverter
        {
            public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
            {
                if (text.Contains("-"))
                {
                    return null;
                }

                return text;
            }
        }
        
    }
    
    public class HayabusaMinimalData : IFileSpecData
    {
        public DateTime Timestamp { get; set; }
        public string RuleTitle { get; set; }
        public string Level { get; set; }
        public string Computer { get; set; }
        public string Channel { get; set; }
        
        public int EventId { get; set; }
        public int RecordId { get; set; }
        public string Details { get; set; }
        
        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{Timestamp} {RuleTitle} {Level} {Computer} {Channel} {EventId} {RecordId} {Details}";
        }
    }

    public class HayabusaMinimal : IFileSpec
    {
        public HayabusaMinimal()
        {
            //Initialize collections here, one for TaggedLines TLE can add values to, and the collection that TLE will display
            TaggedLines = new List<int>();

            DataList = new BindingList<HayabusaMinimalData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "\"Timestamp\",\"RuleTitle\",\"Level\",\"Computer\",\"Channel\",\"EventID\",\"RecordID\",\"Details\""
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from Hayabusa Minimal";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "72de9217-f9b5-4d91-9324-0a17b02d44df";

        public void ProcessFile(string filename)
        {
            DataList.Clear();

            using var fileReader = File.OpenText(filename);
            var csv = new CsvReader(fileReader, CultureInfo.InvariantCulture);

            var foo = csv.Context.AutoMap<HayabusaMinimalData>();

            var o = new TypeConverterOptions
            {
                DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal
            };
            csv.Context.TypeConverterOptionsCache.AddOptions<HayabusaMinimalData>(o);

            foo.Map(m=>m.Timestamp).Convert(row => DateTime.Parse(row.Row.GetField<string>("Timestamp")).ToUniversalTime());
            foo.Map(t => t.EventId).Name("EventID");
            foo.Map(t => t.RecordId).Name("RecordID");
            
            foo.Map(t => t.Line).Ignore();
            foo.Map(t => t.Tag).Ignore();

            csv.Context.RegisterClassMap(foo);

            var records = csv.GetRecords<HayabusaMinimalData>();

            var ln = 1;
            foreach (var record in records)
            {
                Log.Debug("Line # {Line}, Record: {RawRecord}", ln, csv.Context.Parser.RawRecord);

                record.Line = ln;

                record.Tag = TaggedLines.Contains(ln);

                DataList.Add(record);

                ln += 1;
            }
        }
        
    }

    #endregion


    #region Chainsaw


    public class ChainsawSigmaData : IFileSpecData
    {
        public DateTime Timestamp { get; set; }
        public string Detections { get; set; }
        public string Path { get; set; }
        public int Count { get; set; }
        public string EventSystemProvider { get; set; }

        public int? EventID { get; set; }
        public int? RecordID { get; set; }
        public string Computer { get; set; }
        public string EventData { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{Timestamp} {Detections} {Path} {Count} {EventSystemProvider} {EventID} {RecordID} {Computer} {EventData}";
        }
    }

    public class ChainsawSigma : IFileSpec
    {
        public ChainsawSigma()
        {
            //Initialize collections here, one for TaggedLines TLE can add values to, and the collection that TLE will display
            TaggedLines = new List<int>();

            DataList = new BindingList<ChainsawSigmaData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "timestamp,detections,path,count,Event.System.Provider,Event ID,Record ID,Computer,Event Data"
            };
        }

        public string Author => "Reece394";
        public string FileDescription => "CSV generated from Chainsaw - sigma.csv";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "16edcafd-2bd9-4f89-8493-7dc72244aff6";

        public void ProcessFile(string filename)
        {
            DataList.Clear();

            using var fileReader = File.OpenText(filename);
            var csv = new CsvReader(fileReader, CultureInfo.InvariantCulture);

            var foo = csv.Context.AutoMap<ChainsawSigmaData>();

            var o = new TypeConverterOptions
            {
                DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal,
                NullValues = {"=\"\""}
            };
            csv.Context.TypeConverterOptionsCache.AddOptions<ChainsawSigmaData>(o);

            foo.Map(m => m.Timestamp).Convert(row => DateTime.Parse(row.Row.GetField<string>("timestamp")).ToUniversalTime());
            foo.Map(t => t.Detections).Name("detections");
            foo.Map(t => t.Path).Name("path");
            foo.Map(t => t.Count).Name("count");
            foo.Map(t => t.EventSystemProvider).Name("Event.System.Provider");
            foo.Map(t => t.EventID).Name("Event ID");
            foo.Map(t => t.RecordID).Name("Record ID");
            foo.Map(t => t.EventData).Name("Event Data");

            foo.Map(t => t.Line).Ignore();
            foo.Map(t => t.Tag).Ignore();

            csv.Context.RegisterClassMap(foo);

            var records = csv.GetRecords<ChainsawSigmaData>();

            var ln = 1;
            foreach (var record in records)
            {
                Log.Debug("Line # {Line}, Record: {RawRecord}", ln, csv.Context.Parser.RawRecord);

                record.Line = ln;

                record.Tag = TaggedLines.Contains(ln);

                DataList.Add(record);

                ln += 1;
            }
        }

    }

    public class ChainsawAntivirusData : IFileSpecData
    {
        public DateTime Timestamp { get; set; }
        public string Detections { get; set; }
        public string Path { get; set; }

        public int EventID { get; set; }
        public int? RecordID { get; set; }
        public string Computer { get; set; }
        public string ThreatName { get; set; }
        public string ThreatPath { get; set; }
        public string SHA1 { get; set; }
        public string User { get; set; }
        public string ThreatType { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{Timestamp} {Detections} {Path} {EventID} {RecordID} {Computer} {ThreatName} {ThreatPath} {SHA1} {User} {ThreatType}";
        }
    }

    public class ChainsawAntivirus : IFileSpec
    {
        public ChainsawAntivirus()
        {
            //Initialize collections here, one for TaggedLines TLE can add values to, and the collection that TLE will display
            TaggedLines = new List<int>();

            DataList = new BindingList<ChainsawAntivirusData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "timestamp,detections,path,Event ID,Record ID,Computer,Threat Name,Threat Path,SHA1,User,Threat Type"
            };
        }

        public string Author => "Reece394";
        public string FileDescription => "CSV generated from Chainsaw - antivirus.csv";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "c9f944e2-31ae-449b-9500-9c60633e2535";

        public void ProcessFile(string filename)
        {
            DataList.Clear();

            using var fileReader = File.OpenText(filename);
            var csv = new CsvReader(fileReader, CultureInfo.InvariantCulture);

            var foo = csv.Context.AutoMap<ChainsawAntivirusData>();

            var o = new TypeConverterOptions
            {
                DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal,
                NullValues = {"=\"\""}
            };
            csv.Context.TypeConverterOptionsCache.AddOptions<ChainsawAntivirusData>(o);

            foo.Map(m => m.Timestamp).Convert(row => DateTime.Parse(row.Row.GetField<string>("timestamp")).ToUniversalTime());
            foo.Map(t => t.Detections).Name("detections");
            foo.Map(t => t.Path).Name("path");
            foo.Map(t => t.EventID).Name("Event ID");
            foo.Map(t => t.RecordID).Name("Record ID");
            foo.Map(t => t.Computer).Name("Computer");
            foo.Map(t => t.ThreatName).Name("Threat Name");
            foo.Map(t => t.ThreatPath).Name("Threat Path");
            foo.Map(t => t.SHA1).Name("SHA1");
            foo.Map(t => t.User).Name("User");
            foo.Map(t => t.ThreatType).Name("Threat Type");

            foo.Map(t => t.Line).Ignore();
            foo.Map(t => t.Tag).Ignore();

            csv.Context.RegisterClassMap(foo);

            var records = csv.GetRecords<ChainsawAntivirusData>();

            var ln = 1;
            foreach (var record in records)
            {
                Log.Debug("Line # {Line}, Record: {RawRecord}", ln, csv.Context.Parser.RawRecord);

                record.Line = ln;

                record.Tag = TaggedLines.Contains(ln);

                DataList.Add(record);

                ln += 1;
            }
        }

    }

    public class ChainsawAccountTamperingData : IFileSpecData
    {
        public DateTime Timestamp { get; set; }
        public string Detections { get; set; }
        public string Path { get; set; }

        public int EventID { get; set; }
        public int? RecordID { get; set; }
        public string Computer { get; set; }
        public string User { get; set; }
        public string UserSID { get; set; }
        public string MemberSID { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{Timestamp} {Detections} {Path} {EventID} {RecordID} {Computer} {User} {UserSID} {MemberSID}";
        }
    }

    public class ChainsawAccountTampering : IFileSpec
    {
        public ChainsawAccountTampering()
        {
            //Initialize collections here, one for TaggedLines TLE can add values to, and the collection that TLE will display
            TaggedLines = new List<int>();

            DataList = new BindingList<ChainsawAccountTamperingData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "timestamp,detections,path,Event ID,Record ID,Computer,User,User SID,Member SID"
            };
        }

        public string Author => "Reece394";
        public string FileDescription => "CSV generated from Chainsaw - account_tampering.csv";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "e1afe1bd-c8c3-4406-9059-3d111a41aa06";

        public void ProcessFile(string filename)
        {
            DataList.Clear();

            using var fileReader = File.OpenText(filename);
            var csv = new CsvReader(fileReader, CultureInfo.InvariantCulture);

            var foo = csv.Context.AutoMap<ChainsawAccountTamperingData>();

            var o = new TypeConverterOptions
            {
                DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal,
                NullValues = {"=\"\""}
            };
            csv.Context.TypeConverterOptionsCache.AddOptions<ChainsawAccountTamperingData>(o);

            foo.Map(m => m.Timestamp).Convert(row => DateTime.Parse(row.Row.GetField<string>("timestamp")).ToUniversalTime());
            foo.Map(t => t.Detections).Name("detections");
            foo.Map(t => t.Path).Name("path");
            foo.Map(t => t.EventID).Name("Event ID");
            foo.Map(t => t.RecordID).Name("Record ID");
            foo.Map(t => t.Computer).Name("Computer");
            foo.Map(t => t.User).Name("User");
            foo.Map(t => t.UserSID).Name("User SID");
            foo.Map(t => t.MemberSID).Name("Member SID");

            foo.Map(t => t.Line).Ignore();
            foo.Map(t => t.Tag).Ignore();

            csv.Context.RegisterClassMap(foo);

            var records = csv.GetRecords<ChainsawAccountTamperingData>();

            var ln = 1;
            foreach (var record in records)
            {
                Log.Debug("Line # {Line}, Record: {RawRecord}", ln, csv.Context.Parser.RawRecord);

                record.Line = ln;

                record.Tag = TaggedLines.Contains(ln);

                DataList.Add(record);

                ln += 1;
            }
        }

    }

    public class ChainsawCredentialAccessData : IFileSpecData
    {
        public DateTime Timestamp { get; set; }
        public string Detections { get; set; }
        public string Path { get; set; }

        public int EventID { get; set; }
        public string Computer { get; set; }
        public string User { get; set; }
        public string ServiceName { get; set; }
        public string IPAddress { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{Timestamp} {Detections} {Path} {EventID} {Computer} {User} {ServiceName} {IPAddress}";
        }
    }

    public class ChainsawCredentialAccess : IFileSpec
    {
        public ChainsawCredentialAccess()
        {
            //Initialize collections here, one for TaggedLines TLE can add values to, and the collection that TLE will display
            TaggedLines = new List<int>();

            DataList = new BindingList<ChainsawCredentialAccessData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "timestamp,detections,path,Event ID,Computer,User,Service Name,IP Address"
            };
        }

        public string Author => "Reece394";
        public string FileDescription => "CSV generated from Chainsaw - credential_access.csv";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "80ffe0b8-0abe-4cf2-8742-e737e72a9cb5";

        public void ProcessFile(string filename)
        {
            DataList.Clear();

            using var fileReader = File.OpenText(filename);
            var csv = new CsvReader(fileReader, CultureInfo.InvariantCulture);

            var foo = csv.Context.AutoMap<ChainsawCredentialAccessData>();

            var o = new TypeConverterOptions
            {
                DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal
            };
            csv.Context.TypeConverterOptionsCache.AddOptions<ChainsawCredentialAccessData>(o);

            foo.Map(m => m.Timestamp).Convert(row => DateTime.Parse(row.Row.GetField<string>("timestamp")).ToUniversalTime());
            foo.Map(t => t.Detections).Name("detections");
            foo.Map(t => t.Path).Name("path");
            foo.Map(t => t.EventID).Name("Event ID");
            foo.Map(t => t.Computer).Name("Computer");
            foo.Map(t => t.User).Name("User");
            foo.Map(t => t.ServiceName).Name("Service Name");
            foo.Map(t => t.IPAddress).Name("IP Address");

            foo.Map(t => t.Line).Ignore();
            foo.Map(t => t.Tag).Ignore();

            csv.Context.RegisterClassMap(foo);

            var records = csv.GetRecords<ChainsawCredentialAccessData>();

            var ln = 1;
            foreach (var record in records)
            {
                Log.Debug("Line # {Line}, Record: {RawRecord}", ln, csv.Context.Parser.RawRecord);

                record.Line = ln;

                record.Tag = TaggedLines.Contains(ln);

                DataList.Add(record);

                ln += 1;
            }
        }

    }

    public class ChainsawLateralMovementData : IFileSpecData
    {
        public DateTime Timestamp { get; set; }
        public string Detections { get; set; }
        public string Path { get; set; }

        public int EventID { get; set; }
        public int? RecordID { get; set; }
        public string Computer { get; set; }
        public string User { get; set; }
        public string LogonType { get; set; }
        public string IPAddress { get; set; }
        public string ProcessName { get; set; }
        public string ProcessID { get; set; }
        public string InterfaceIndex { get; set; }
        public string SourceAddress { get; set; }
        public string SourcePort { get; set; }
        public string DestAddress { get; set; }
        public string DestPort { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{Timestamp} {Detections} {Path} {EventID} {RecordID} {Computer} {User} {LogonType} {IPAddress} {ProcessName} {ProcessID} {InterfaceIndex} {SourceAddress} {SourcePort} {DestAddress} {DestPort}";
        }
    }

    public class ChainsawLateralMovement : IFileSpec
    {
        public ChainsawLateralMovement()
        {
            //Initialize collections here, one for TaggedLines TLE can add values to, and the collection that TLE will display
            TaggedLines = new List<int>();

            DataList = new BindingList<ChainsawLateralMovementData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "timestamp,detections,path,Event ID,Record ID,Computer,User,Logon Type,IP Address,Process Name,Process ID,Interface Index,Source Address,Source Port,Dest Address,Dest Port"
            };
        }

        public string Author => "Reece394";
        public string FileDescription => "CSV generated from Chainsaw - lateral_movement.csv";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "7fce9e8e-7d40-437b-9545-9e3c446285c2";

        public void ProcessFile(string filename)
        {
            DataList.Clear();

            using var fileReader = File.OpenText(filename);
            var csv = new CsvReader(fileReader, CultureInfo.InvariantCulture);

            var foo = csv.Context.AutoMap<ChainsawLateralMovementData>();

            var o = new TypeConverterOptions
            {
                DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal,
                NullValues = {"=\"\""}
            };
            csv.Context.TypeConverterOptionsCache.AddOptions<ChainsawLateralMovementData>(o);

            foo.Map(m => m.Timestamp).Convert(row => DateTime.Parse(row.Row.GetField<string>("timestamp")).ToUniversalTime());
            foo.Map(t => t.Detections).Name("detections");
            foo.Map(t => t.Path).Name("path");
            foo.Map(t => t.EventID).Name("Event ID");
            foo.Map(t => t.RecordID).Name("Record ID");
            foo.Map(t => t.Computer).Name("Computer");
            foo.Map(t => t.User).Name("User");
            foo.Map(t => t.LogonType).Name("Logon Type");
            foo.Map(t => t.IPAddress).Name("IP Address");
            foo.Map(t => t.ProcessName).Name("Process Name");
            foo.Map(t => t.ProcessID).Name("Process ID");
            foo.Map(t => t.InterfaceIndex).Name("Interface Index");
            foo.Map(t => t.SourceAddress).Name("Source Address");
            foo.Map(t => t.SourcePort).Name("Source Port");
            foo.Map(t => t.DestAddress).Name("Dest Address");
            foo.Map(t => t.DestPort).Name("Dest Port");

            foo.Map(t => t.Line).Ignore();
            foo.Map(t => t.Tag).Ignore();

            csv.Context.RegisterClassMap(foo);

            var records = csv.GetRecords<ChainsawLateralMovementData>();

            var ln = 1;
            foreach (var record in records)
            {
                Log.Debug("Line # {Line}, Record: {RawRecord}", ln, csv.Context.Parser.RawRecord);

                record.Line = ln;

                record.Tag = TaggedLines.Contains(ln);

                DataList.Add(record);

                ln += 1;
            }
        }

    }

    public class ChainsawLateralMovementOldData : IFileSpecData
    {
        public DateTime Timestamp { get; set; }
        public string Detections { get; set; }
        public string Path { get; set; }

        public int EventID { get; set; }
        public int? RecordID { get; set; }
        public string Computer { get; set; }
        public string User { get; set; }
        public string LogonType { get; set; }
        public string IPAddress { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{Timestamp} {Detections} {Path} {EventID} {RecordID} {Computer} {User} {LogonType} {IPAddress}";
        }
    }

    public class ChainsawLateralMovementOld : IFileSpec
    {
        public ChainsawLateralMovementOld()
        {
            //Initialize collections here, one for TaggedLines TLE can add values to, and the collection that TLE will display
            TaggedLines = new List<int>();

            DataList = new BindingList<ChainsawLateralMovementOldData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "timestamp,detections,path,Event ID,Record ID,Computer,User,Logon Type,IP Address"
            };
        }

        public string Author => "Reece394";
        public string FileDescription => "CSV generated from Chainsaw Older Version - lateral_movement.csv";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "fb499ec4-ac70-43d4-8607-fcff501cc58c";

        public void ProcessFile(string filename)
        {
            DataList.Clear();

            using var fileReader = File.OpenText(filename);
            var csv = new CsvReader(fileReader, CultureInfo.InvariantCulture);

            var foo = csv.Context.AutoMap<ChainsawLateralMovementOldData>();

            var o = new TypeConverterOptions
            {
                DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal,
                NullValues = {"=\"\""}
            };
            csv.Context.TypeConverterOptionsCache.AddOptions<ChainsawLateralMovementOldData>(o);

            foo.Map(m => m.Timestamp).Convert(row => DateTime.Parse(row.Row.GetField<string>("timestamp")).ToUniversalTime());
            foo.Map(t => t.Detections).Name("detections");
            foo.Map(t => t.Path).Name("path");
            foo.Map(t => t.EventID).Name("Event ID");
            foo.Map(t => t.RecordID).Name("Record ID");
            foo.Map(t => t.Computer).Name("Computer");
            foo.Map(t => t.User).Name("User");
            foo.Map(t => t.LogonType).Name("Logon Type");
            foo.Map(t => t.IPAddress).Name("IP Address");

            foo.Map(t => t.Line).Ignore();
            foo.Map(t => t.Tag).Ignore();

            csv.Context.RegisterClassMap(foo);

            var records = csv.GetRecords<ChainsawLateralMovementOldData>();

            var ln = 1;
            foreach (var record in records)
            {
                Log.Debug("Line # {Line}, Record: {RawRecord}", ln, csv.Context.Parser.RawRecord);

                record.Line = ln;

                record.Tag = TaggedLines.Contains(ln);

                DataList.Add(record);

                ln += 1;
            }
        }

    }

    public class ChainsawRDSPowerShellScriptData : IFileSpecData
    {
        public DateTime Timestamp { get; set; }
        public string Detections { get; set; }
        public string Path { get; set; }

        public int EventID { get; set; }
        public string Channel { get; set; }
        public string Computer { get; set; }
        public string Information { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{Timestamp} {Detections} {Path} {EventID} {Channel} {Computer} {Information}";
        }
    }

    public class ChainsawRDSPowerShellScript : IFileSpec
    {
        public ChainsawRDSPowerShellScript()
        {
            //Initialize collections here, one for TaggedLines TLE can add values to, and the collection that TLE will display
            TaggedLines = new List<int>();

            DataList = new BindingList<ChainsawRDSPowerShellScriptData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "timestamp,detections,path,Event ID,Channel,Computer,Information"
            };
        }

        public string Author => "Reece394";
        public string FileDescription => "CSV generated from Chainsaw - powershell_script.csv and RDS csvs";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "50d882ec-4c60-46bf-892e-3f8045372185";

        public void ProcessFile(string filename)
        {
            DataList.Clear();

            using var fileReader = File.OpenText(filename);
            var csv = new CsvReader(fileReader, CultureInfo.InvariantCulture);

            var foo = csv.Context.AutoMap<ChainsawRDSPowerShellScriptData>();

            var o = new TypeConverterOptions
            {
                DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal
            };
            csv.Context.TypeConverterOptionsCache.AddOptions<ChainsawRDSPowerShellScriptData>(o);

            foo.Map(m => m.Timestamp).Convert(row => DateTime.Parse(row.Row.GetField<string>("timestamp")).ToUniversalTime());
            foo.Map(t => t.Detections).Name("detections");
            foo.Map(t => t.Path).Name("path");
            foo.Map(t => t.EventID).Name("Event ID");
            foo.Map(t => t.Channel).Name("Channel");
            foo.Map(t => t.Computer).Name("Computer");
            foo.Map(t => t.Information).Name("Information");

            foo.Map(t => t.Line).Ignore();
            foo.Map(t => t.Tag).Ignore();

            csv.Context.RegisterClassMap(foo);

            var records = csv.GetRecords<ChainsawRDSPowerShellScriptData>();

            var ln = 1;
            foreach (var record in records)
            {
                Log.Debug("Line # {Line}, Record: {RawRecord}", ln, csv.Context.Parser.RawRecord);

                record.Line = ln;

                record.Tag = TaggedLines.Contains(ln);

                DataList.Add(record);

                ln += 1;
            }
        }

    }

    public class ChainsawRDSWebAccessData : IFileSpecData
    {
        public DateTime Timestamp { get; set; }
        public string Detections { get; set; }
        public string Path { get; set; }

        public int EventID { get; set; }
        public string Channel { get; set; }
        public string Computer { get; set; }
        public string IPAddress { get; set; }
        public string LogonType { get; set; }
        public string TargetUserName { get; set; }
        public string WorkstationName { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{Timestamp} {Detections} {Path} {EventID} {Channel} {Computer} {IPAddress} {LogonType} {TargetUserName} {WorkstationName}";
        }
    }

    public class ChainsawRDSWebAccess : IFileSpec
    {
        public ChainsawRDSWebAccess()
        {
            //Initialize collections here, one for TaggedLines TLE can add values to, and the collection that TLE will display
            TaggedLines = new List<int>();

            DataList = new BindingList<ChainsawRDSWebAccessData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "timestamp,detections,path,Event ID,Channel,Computer,IpAddress,LogonType,TargetUserName,WorkstationName"
            };
        }

        public string Author => "Reece394";
        public string FileDescription => "CSV generated from Chainsaw - microsoft_rds_events_-_rd_web_access_successful_logon.csv";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "92f5a8df-bd8e-4996-bbe9-2561f12932e2";

        public void ProcessFile(string filename)
        {
            DataList.Clear();

            using var fileReader = File.OpenText(filename);
            var csv = new CsvReader(fileReader, CultureInfo.InvariantCulture);

            var foo = csv.Context.AutoMap<ChainsawRDSWebAccessData>();

            var o = new TypeConverterOptions
            {
                DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal
            };
            csv.Context.TypeConverterOptionsCache.AddOptions<ChainsawRDSWebAccessData>(o);

            foo.Map(m => m.Timestamp).Convert(row => DateTime.Parse(row.Row.GetField<string>("timestamp")).ToUniversalTime());
            foo.Map(t => t.Detections).Name("detections");
            foo.Map(t => t.Path).Name("path");
            foo.Map(t => t.EventID).Name("Event ID");
            foo.Map(t => t.Channel).Name("Channel");
            foo.Map(t => t.Computer).Name("Computer");
            foo.Map(t => t.IPAddress).Name("IpAddress");
            foo.Map(t => t.LogonType).Name("LogonType");
            foo.Map(t => t.TargetUserName).Name("TargetUserName");
            foo.Map(t => t.WorkstationName).Name("WorkstationName");

            foo.Map(t => t.Line).Ignore();
            foo.Map(t => t.Tag).Ignore();

            csv.Context.RegisterClassMap(foo);

            var records = csv.GetRecords<ChainsawRDSWebAccessData>();

            var ln = 1;
            foreach (var record in records)
            {
                Log.Debug("Line # {Line}, Record: {RawRecord}", ln, csv.Context.Parser.RawRecord);

                record.Line = ln;

                record.Tag = TaggedLines.Contains(ln);

                DataList.Add(record);

                ln += 1;
            }
        }

    }

    public class ChainsawLogTamperingData : IFileSpecData
    {
        public DateTime Timestamp { get; set; }
        public string Detections { get; set; }
        public string Path { get; set; }

        public int EventID { get; set; }
        public int? RecordID { get; set; }
        public string Computer { get; set; }
        public string User { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{Timestamp} {Detections} {Path} {EventID} {RecordID} {Computer} {User}";
        }
    }

    public class ChainsawLogTampering : IFileSpec
    {
        public ChainsawLogTampering()
        {
            //Initialize collections here, one for TaggedLines TLE can add values to, and the collection that TLE will display
            TaggedLines = new List<int>();

            DataList = new BindingList<ChainsawLogTamperingData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "timestamp,detections,path,Event ID,Record ID,Computer,User"
            };
        }

        public string Author => "Reece394";
        public string FileDescription => "CSV generated from Chainsaw - log_tampering.csv";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "c98793ac-0c36-4874-8fad-35fad0789da4";

        public void ProcessFile(string filename)
        {
            DataList.Clear();

            using var fileReader = File.OpenText(filename);
            var csv = new CsvReader(fileReader, CultureInfo.InvariantCulture);

            var foo = csv.Context.AutoMap<ChainsawLogTamperingData>();

            var o = new TypeConverterOptions
            {
                DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal,
                NullValues = {"=\"\""}
            };
            csv.Context.TypeConverterOptionsCache.AddOptions<ChainsawLogTamperingData>(o);

            foo.Map(m => m.Timestamp).Convert(row => DateTime.Parse(row.Row.GetField<string>("timestamp")).ToUniversalTime());
            foo.Map(t => t.Detections).Name("detections");
            foo.Map(t => t.Path).Name("path");
            foo.Map(t => t.EventID).Name("Event ID");
            foo.Map(t => t.RecordID).Name("Record ID");
            foo.Map(t => t.Computer).Name("Computer");
            foo.Map(t => t.User).Name("User");

            foo.Map(t => t.Line).Ignore();
            foo.Map(t => t.Tag).Ignore();

            csv.Context.RegisterClassMap(foo);

            var records = csv.GetRecords<ChainsawLogTamperingData>();

            var ln = 1;
            foreach (var record in records)
            {
                Log.Debug("Line # {Line}, Record: {RawRecord}", ln, csv.Context.Parser.RawRecord);

                record.Line = ln;

                record.Tag = TaggedLines.Contains(ln);

                DataList.Add(record);

                ln += 1;
            }
        }

    }

    public class ChainsawPowerShellEngineStateData : IFileSpecData
    {
        public DateTime Timestamp { get; set; }
        public string Detections { get; set; }
        public string Path { get; set; }

        public int EventID { get; set; }
        public string Channel { get; set; }
        public string Computer { get; set; }
        public string HostName { get; set; }
        public string HostVersion { get; set; }
        public string HostApplication { get; set; }
        public string PipelineID { get; set; }
        public string CommandName { get; set; }
        public string CommandType { get; set; }
        public string ScriptName { get; set; }
        public string CommandPath { get; set; }
        public string CommandLine { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{Timestamp} {Detections} {Path} {EventID} {Channel} {Computer} {HostName} {HostVersion} {HostApplication} {PipelineID} {CommandName} {CommandType} {ScriptName} {CommandPath} {CommandLine}";
        }
    }

    public class ChainsawPowerShellEngineState : IFileSpec
    {
        public ChainsawPowerShellEngineState()
        {
            //Initialize collections here, one for TaggedLines TLE can add values to, and the collection that TLE will display
            TaggedLines = new List<int>();

            DataList = new BindingList<ChainsawPowerShellEngineStateData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "timestamp,detections,path,Event ID,Channel,Computer,HostName,HostVersion,HostApplication,PipelineId,CommandName,CommandType,ScriptName,CommandPath,CommandLine"
            };
        }

        public string Author => "Reece394";
        public string FileDescription => "CSV generated from Chainsaw - powershell_engine_state.csv";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "9b464d8e-7b62-44d1-9319-5d4998797f3f";

        public void ProcessFile(string filename)
        {
            DataList.Clear();

            using var fileReader = File.OpenText(filename);
            var csv = new CsvReader(fileReader, CultureInfo.InvariantCulture);

            var foo = csv.Context.AutoMap<ChainsawPowerShellEngineStateData>();

            var o = new TypeConverterOptions
            {
                DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal
            };
            csv.Context.TypeConverterOptionsCache.AddOptions<ChainsawPowerShellEngineStateData>(o);

            foo.Map(m => m.Timestamp).Convert(row => DateTime.Parse(row.Row.GetField<string>("timestamp")).ToUniversalTime());
            foo.Map(t => t.Detections).Name("detections");
            foo.Map(t => t.Path).Name("path");
            foo.Map(t => t.EventID).Name("Event ID");
            foo.Map(t => t.Channel).Name("Channel");
            foo.Map(t => t.Computer).Name("Computer");
            foo.Map(t => t.HostName).Name("HostName");
            foo.Map(t => t.HostVersion).Name("HostVersion");
            foo.Map(t => t.HostApplication).Name("HostApplication");
            foo.Map(t => t.PipelineID).Name("PipelineId");
            foo.Map(t => t.CommandName).Name("CommandName");
            foo.Map(t => t.CommandType).Name("CommandType");
            foo.Map(t => t.ScriptName).Name("ScriptName");
            foo.Map(t => t.CommandPath).Name("CommandPath");
            foo.Map(t => t.CommandLine).Name("CommandLine");

            foo.Map(t => t.Line).Ignore();
            foo.Map(t => t.Tag).Ignore();

            csv.Context.RegisterClassMap(foo);

            var records = csv.GetRecords<ChainsawPowerShellEngineStateData>();

            var ln = 1;
            foreach (var record in records)
            {
                Log.Debug("Line # {Line}, Record: {RawRecord}", ln, csv.Context.Parser.RawRecord);

                record.Line = ln;

                record.Tag = TaggedLines.Contains(ln);

                DataList.Add(record);

                ln += 1;
            }
        }

    }

    public class ChainsawLoginAttacksData : IFileSpecData
    {
        public DateTime Timestamp { get; set; }
        public string Detections { get; set; }
        public string Path { get; set; }

        public int Count { get; set; }
        public int EventID { get; set; }
        public string User { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{Timestamp} {Detections} {Path} {Count} {EventID} {User}";
        }
    }

    public class ChainsawLoginAttacks : IFileSpec
    {
        public ChainsawLoginAttacks()
        {
            //Initialize collections here, one for TaggedLines TLE can add values to, and the collection that TLE will display
            TaggedLines = new List<int>();

            DataList = new BindingList<ChainsawLoginAttacksData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "timestamp,detections,path,count,Event ID,User"
            };
        }

        public string Author => "Reece394";
        public string FileDescription => "CSV generated from Chainsaw - login_attacks.csv";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "01cf0187-3706-447d-bec7-956abdd798cc";

        public void ProcessFile(string filename)
        {
            DataList.Clear();

            using var fileReader = File.OpenText(filename);
            var csv = new CsvReader(fileReader, CultureInfo.InvariantCulture);

            var foo = csv.Context.AutoMap<ChainsawLoginAttacksData>();

            var o = new TypeConverterOptions
            {
                DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal
            };
            csv.Context.TypeConverterOptionsCache.AddOptions<ChainsawLoginAttacksData>(o);

            foo.Map(m => m.Timestamp).Convert(row => DateTime.Parse(row.Row.GetField<string>("timestamp")).ToUniversalTime());
            foo.Map(t => t.Detections).Name("detections");
            foo.Map(t => t.Path).Name("path");
            foo.Map(t => t.Count).Name("count");
            foo.Map(t => t.EventID).Name("Event ID");
            foo.Map(t => t.User).Name("User");

            foo.Map(t => t.Line).Ignore();
            foo.Map(t => t.Tag).Ignore();

            csv.Context.RegisterClassMap(foo);

            var records = csv.GetRecords<ChainsawLoginAttacksData>();

            var ln = 1;
            foreach (var record in records)
            {
                Log.Debug("Line # {Line}, Record: {RawRecord}", ln, csv.Context.Parser.RawRecord);

                record.Line = ln;

                record.Tag = TaggedLines.Contains(ln);

                DataList.Add(record);

                ln += 1;
            }
        }

    }

    public class ChainsawIndicatorRemovalData : IFileSpecData
    {
        public DateTime Timestamp { get; set; }
        public string Detections { get; set; }
        public string Path { get; set; }

        public int EventID { get; set; }
        public int? RecordID { get; set; }
        public string Computer { get; set; }
        public string UserName { get; set; }
        public string ScheduledTaskName { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{Timestamp} {Detections} {Path} {EventID} {RecordID} {Computer} {UserName} {ScheduledTaskName}";
        }
    }

    public class ChainsawIndicatorRemoval : IFileSpec
    {
        public ChainsawIndicatorRemoval()
        {
            //Initialize collections here, one for TaggedLines TLE can add values to, and the collection that TLE will display
            TaggedLines = new List<int>();

            DataList = new BindingList<ChainsawIndicatorRemovalData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "timestamp,detections,path,Event ID,Record ID,Computer,User Name,Scheduled Task Name"
            };
        }

        public string Author => "Reece394";
        public string FileDescription => "CSV generated from Chainsaw - indicator_removal.csv";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "30e3b579-3f45-4e19-a4ac-c302a7551c14";

        public void ProcessFile(string filename)
        {
            DataList.Clear();

            using var fileReader = File.OpenText(filename);
            var csv = new CsvReader(fileReader, CultureInfo.InvariantCulture);

            var foo = csv.Context.AutoMap<ChainsawIndicatorRemovalData>();

            var o = new TypeConverterOptions
            {
                DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal,
                NullValues = {"=\"\""}
            };
            csv.Context.TypeConverterOptionsCache.AddOptions<ChainsawIndicatorRemovalData>(o);

            foo.Map(m => m.Timestamp).Convert(row => DateTime.Parse(row.Row.GetField<string>("timestamp")).ToUniversalTime());
            foo.Map(t => t.Detections).Name("detections");
            foo.Map(t => t.Path).Name("path");
            foo.Map(t => t.EventID).Name("Event ID");
            foo.Map(t => t.RecordID).Name("Record ID");
            foo.Map(t => t.Computer).Name("Computer");
            foo.Map(t => t.UserName).Name("User Name");
            foo.Map(t => t.ScheduledTaskName).Name("Scheduled Task Name");

            foo.Map(t => t.Line).Ignore();
            foo.Map(t => t.Tag).Ignore();

            csv.Context.RegisterClassMap(foo);

            var records = csv.GetRecords<ChainsawIndicatorRemovalData>();

            var ln = 1;
            foreach (var record in records)
            {
                Log.Debug("Line # {Line}, Record: {RawRecord}", ln, csv.Context.Parser.RawRecord);

                record.Line = ln;

                record.Tag = TaggedLines.Contains(ln);

                DataList.Add(record);

                ln += 1;
            }
        }

    }

    public class ChainsawPersistenceData : IFileSpecData
    {
        public DateTime Timestamp { get; set; }
        public string Detections { get; set; }
        public string Path { get; set; }

        public int EventID { get; set; }
        public int? RecordID { get; set; }
        public string Computer { get; set; }
        public string UserName { get; set; }
        public string ScheduledTaskName { get; set; }
        public string DomainName { get; set; }
        public string LogonID { get; set; }
        public string ParentProcessName { get; set; }
        public string ParentProcessCommandLine { get; set; }
        public string NewProcessName { get; set; }
        public string NewProcessID { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{Timestamp} {Detections} {Path} {EventID} {RecordID} {Computer} {UserName} {ScheduledTaskName} {DomainName} {LogonID} {ParentProcessName} {ParentProcessCommandLine} {NewProcessName} {NewProcessID}";
        }
    }

    public class ChainsawPersistence : IFileSpec
    {
        public ChainsawPersistence()
        {
            //Initialize collections here, one for TaggedLines TLE can add values to, and the collection that TLE will display
            TaggedLines = new List<int>();

            DataList = new BindingList<ChainsawPersistenceData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "timestamp,detections,path,Event ID,Record ID,Computer,User Name,Scheduled Task Name,Domain Name,Logon ID,Parent Process Name,Parent Process Command Line,New Process Name,New Process ID"
            };
        }

        public string Author => "Reece394";
        public string FileDescription => "CSV generated from Chainsaw - persistence.csv";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "411f33d8-aece-4b58-b7f2-26d24e5e9a9d";

        public void ProcessFile(string filename)
        {
            DataList.Clear();

            using var fileReader = File.OpenText(filename);
            var csv = new CsvReader(fileReader, CultureInfo.InvariantCulture);

            var foo = csv.Context.AutoMap<ChainsawPersistenceData>();

            var o = new TypeConverterOptions
            {
                DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal,
                NullValues = {"=\"\""}
            };
            csv.Context.TypeConverterOptionsCache.AddOptions<ChainsawPersistenceData>(o);

            foo.Map(m => m.Timestamp).Convert(row => DateTime.Parse(row.Row.GetField<string>("timestamp")).ToUniversalTime());
            foo.Map(t => t.Detections).Name("detections");
            foo.Map(t => t.Path).Name("path");
            foo.Map(t => t.EventID).Name("Event ID");
            foo.Map(t => t.RecordID).Name("Record ID");
            foo.Map(t => t.Computer).Name("Computer");
            foo.Map(t => t.UserName).Name("User Name");
            foo.Map(t => t.ScheduledTaskName).Name("Scheduled Task Name");
            foo.Map(t => t.DomainName).Name("Domain Name");
            foo.Map(t => t.LogonID).Name("Logon ID");
            foo.Map(t => t.ParentProcessName).Name("Parent Process Name");
            foo.Map(t => t.ParentProcessCommandLine).Name("Parent Process Command Line");
            foo.Map(t => t.NewProcessName).Name("New Process Name");
            foo.Map(t => t.NewProcessID).Name("New Process ID");

            foo.Map(t => t.Line).Ignore();
            foo.Map(t => t.Tag).Ignore();

            csv.Context.RegisterClassMap(foo);

            var records = csv.GetRecords<ChainsawPersistenceData>();

            var ln = 1;
            foreach (var record in records)
            {
                Log.Debug("Line # {Line}, Record: {RawRecord}", ln, csv.Context.Parser.RawRecord);

                record.Line = ln;

                record.Tag = TaggedLines.Contains(ln);

                DataList.Add(record);

                ln += 1;
            }
        }

    }

    public class ChainsawRDPAttacksData : IFileSpecData
    {
        public DateTime Timestamp { get; set; }
        public string Detections { get; set; }
        public string Path { get; set; }

        public int EventID { get; set; }
        public string Computer { get; set; }
        public string IPAddress { get; set; }
        public string Username { get; set; }
        public string Provider { get; set; }
        public string LogonType { get; set; }
        public int? RecordID { get; set; }
        public string Domain { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{Timestamp} {Detections} {Path} {EventID} {Computer} {IPAddress} {Username} {Provider} {LogonType} {RecordID} {Domain}";
        }
    }

    public class ChainsawRDPAttacks : IFileSpec
    {
        public ChainsawRDPAttacks()
        {
            //Initialize collections here, one for TaggedLines TLE can add values to, and the collection that TLE will display
            TaggedLines = new List<int>();

            DataList = new BindingList<ChainsawRDPAttacksData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "timestamp,detections,path,event id,computer,ip address,username,provider,logon type,record id,domain"
            };
        }

        public string Author => "Reece394";
        public string FileDescription => "CSV generated from Chainsaw - rdp_attacks.csv";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "861eb62e-8bcd-4410-9afe-b2ceddc9a82e";

        public void ProcessFile(string filename)
        {
            DataList.Clear();

            using var fileReader = File.OpenText(filename);
            var csv = new CsvReader(fileReader, CultureInfo.InvariantCulture);

            var foo = csv.Context.AutoMap<ChainsawRDPAttacksData>();

            var o = new TypeConverterOptions
            {
                DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal,
                NullValues = {"=\"\""}
            };
            csv.Context.TypeConverterOptionsCache.AddOptions<ChainsawRDPAttacksData>(o);

            foo.Map(m => m.Timestamp).Convert(row => DateTime.Parse(row.Row.GetField<string>("timestamp")).ToUniversalTime());
            foo.Map(t => t.Detections).Name("detections");
            foo.Map(t => t.Path).Name("path");
            foo.Map(t => t.EventID).Name("event id");

            foo.Map(t => t.Computer).Name("computer");
            foo.Map(t => t.IPAddress).Name("ip address");
            foo.Map(t => t.Username).Name("username");
            foo.Map(t => t.Provider).Name("provider");
            foo.Map(t => t.LogonType).Name("logon type");
            foo.Map(t => t.RecordID).Name("record id");
            foo.Map(t => t.Domain).Name("domain");

            foo.Map(t => t.Line).Ignore();
            foo.Map(t => t.Tag).Ignore();

            csv.Context.RegisterClassMap(foo);

            var records = csv.GetRecords<ChainsawRDPAttacksData>();

            var ln = 1;
            foreach (var record in records)
            {
                Log.Debug("Line # {Line}, Record: {RawRecord}", ln, csv.Context.Parser.RawRecord);

                record.Line = ln;

                record.Tag = TaggedLines.Contains(ln);

                DataList.Add(record);

                ln += 1;
            }
        }

    }

    public class ChainsawRasVPNData : IFileSpecData
    {
        public DateTime Timestamp { get; set; }
        public string Detections { get; set; }
        public string Path { get; set; }

        public int EventID { get; set; }
        public string Channel { get; set; }
        public string Provider { get; set; }
        public string Computer { get; set; }
        public string Data { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{Timestamp} {Detections} {Path} {EventID} {Channel} {Provider} {Computer} {Data}";
        }
    }

    public class ChainsawRasVPN : IFileSpec
    {
        public ChainsawRasVPN()
        {
            //Initialize collections here, one for TaggedLines TLE can add values to, and the collection that TLE will display
            TaggedLines = new List<int>();

            DataList = new BindingList<ChainsawRasVPNData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "timestamp,detections,path,Event ID,Channel,Provider,Computer,Data"
            };
        }

        public string Author => "Reece394";
        public string FileDescription => "CSV generated from Chainsaw - microsoft_rasvpn_events";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "11000a42-a916-4116-a35c-2bfeaa02b40d";

        public void ProcessFile(string filename)
        {
            DataList.Clear();

            using var fileReader = File.OpenText(filename);
            var csv = new CsvReader(fileReader, CultureInfo.InvariantCulture);

            var foo = csv.Context.AutoMap<ChainsawRasVPNData>();

            var o = new TypeConverterOptions
            {
                DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal
            };
            csv.Context.TypeConverterOptionsCache.AddOptions<ChainsawRasVPNData>(o);

            foo.Map(m => m.Timestamp).Convert(row => DateTime.Parse(row.Row.GetField<string>("timestamp")).ToUniversalTime());
            foo.Map(t => t.Detections).Name("detections");
            foo.Map(t => t.Path).Name("path");
            foo.Map(t => t.EventID).Name("Event ID");
            foo.Map(t => t.Channel).Name("Channel");
            foo.Map(t => t.Provider).Name("Provider");
            foo.Map(t => t.Computer).Name("Computer");
            foo.Map(t => t.Data).Name("Data");

            foo.Map(t => t.Line).Ignore();
            foo.Map(t => t.Tag).Ignore();

            csv.Context.RegisterClassMap(foo);

            var records = csv.GetRecords<ChainsawRasVPNData>();

            var ln = 1;
            foreach (var record in records)
            {
                Log.Debug("Line # {Line}, Record: {RawRecord}", ln, csv.Context.Parser.RawRecord);

                record.Line = ln;

                record.Tag = TaggedLines.Contains(ln);

                DataList.Add(record);

                ln += 1;
            }
        }

    }

    public class ChainsawServiceInstallationData : IFileSpecData
    {
        public DateTime Timestamp { get; set; }
        public string Detections { get; set; }
        public string Path { get; set; }

        public int EventID { get; set; }
        public int? RecordID { get; set; }
        public string Computer { get; set; }
        public string ServiceName { get; set; }
        public string ServiceFileName { get; set; }
        public string ServiceType { get; set; }
        public string ServiceStartType { get; set; }
        public string ServiceAccount { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{Timestamp} {Detections} {Path} {EventID} {RecordID} {Computer} {ServiceName} {ServiceFileName} {ServiceType} {ServiceStartType} {ServiceAccount}";
        }
    }

    public class ChainsawServiceInstallation : IFileSpec
    {
        public ChainsawServiceInstallation()
        {
            //Initialize collections here, one for TaggedLines TLE can add values to, and the collection that TLE will display
            TaggedLines = new List<int>();

            DataList = new BindingList<ChainsawServiceInstallationData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "timestamp,detections,path,Event ID,Record ID,Computer,Service Name,Service File Name,Service Type,Service Start Type,Service Account"
            };
        }

        public string Author => "Reece394";
        public string FileDescription => "CSV generated from Chainsaw - service_installation.csv";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "995c4f43-cca4-4993-9f27-18b1a58ec124";

        public void ProcessFile(string filename)
        {
            DataList.Clear();

            using var fileReader = File.OpenText(filename);
            var csv = new CsvReader(fileReader, CultureInfo.InvariantCulture);

            var foo = csv.Context.AutoMap<ChainsawServiceInstallationData>();

            var o = new TypeConverterOptions
            {
                DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal,
                NullValues = {"=\"\""}
            };
            csv.Context.TypeConverterOptionsCache.AddOptions<ChainsawServiceInstallationData>(o);

            foo.Map(m => m.Timestamp).Convert(row => DateTime.Parse(row.Row.GetField<string>("timestamp")).ToUniversalTime());
            foo.Map(t => t.Detections).Name("detections");
            foo.Map(t => t.Path).Name("path");
            foo.Map(t => t.EventID).Name("Event ID");
            foo.Map(t => t.RecordID).Name("Record ID");
            foo.Map(t => t.Computer).Name("Computer");
            foo.Map(t => t.ServiceName).Name("Service Name");
            foo.Map(t => t.ServiceFileName).Name("Service File Name");
            foo.Map(t => t.ServiceType).Name("Service Type");
            foo.Map(t => t.ServiceStartType).Name("Service Start Type");
            foo.Map(t => t.ServiceAccount).Name("Service Account");

            foo.Map(t => t.Line).Ignore();
            foo.Map(t => t.Tag).Ignore();

            csv.Context.RegisterClassMap(foo);

            var records = csv.GetRecords<ChainsawServiceInstallationData>();

            var ln = 1;
            foreach (var record in records)
            {
                Log.Debug("Line # {Line}, Record: {RawRecord}", ln, csv.Context.Parser.RawRecord);

                record.Line = ln;

                record.Tag = TaggedLines.Contains(ln);

                DataList.Add(record);

                ln += 1;
            }
        }

    }

    public class ChainsawServiceTamperingData : IFileSpecData
    {
        public DateTime Timestamp { get; set; }
        public string Detections { get; set; }
        public string Path { get; set; }

        public int EventID { get; set; }
        public string Username { get; set; }
        public string Computer { get; set; }
        public string Command { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string Service { get; set; }
        public string OldState { get; set; }
        public string NewState { get; set; }
        public string SID { get; set; }
        public int? RecordID { get; set; }
        public string ServiceName { get; set; }
        public string Action { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{Timestamp} {Detections} {Path} {EventID} {Username} {Computer} {Command} {OldValue} {NewValue} {Service} {OldState} {NewState} {SID} {RecordID} {ServiceName} {Action}";
        }
    }

    public class ChainsawServiceTampering : IFileSpec
    {
        public ChainsawServiceTampering()
        {
            //Initialize collections here, one for TaggedLines TLE can add values to, and the collection that TLE will display
            TaggedLines = new List<int>();

            DataList = new BindingList<ChainsawServiceTamperingData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "timestamp,detections,path,Event ID,Username,Computer,Command,Old Value,New Value,Service,Old State,New State,SID,Record ID,Service Name,Action"
            };
        }

        public string Author => "Reece394";
        public string FileDescription => "CSV generated from Chainsaw - service_tampering.csv";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "2f9108dc-aa19-47fb-8875-af31531cb913";

        public void ProcessFile(string filename)
        {
            DataList.Clear();

            using var fileReader = File.OpenText(filename);
            var csv = new CsvReader(fileReader, CultureInfo.InvariantCulture);

            var foo = csv.Context.AutoMap<ChainsawServiceTamperingData>();

            var o = new TypeConverterOptions
            {
                DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal,
                NullValues = {"=\"\""}
            };
            csv.Context.TypeConverterOptionsCache.AddOptions<ChainsawServiceTamperingData>(o);

            foo.Map(m => m.Timestamp).Convert(row => DateTime.Parse(row.Row.GetField<string>("timestamp")).ToUniversalTime());
            foo.Map(t => t.Detections).Name("detections");
            foo.Map(t => t.Path).Name("path");
            foo.Map(t => t.EventID).Name("Event ID");
            foo.Map(t => t.Username).Name("Username");
            foo.Map(t => t.Computer).Name("Computer");
            foo.Map(t => t.Command).Name("Command");
            foo.Map(t => t.OldValue).Name("Old Value");
            foo.Map(t => t.NewValue).Name("New Value");
            foo.Map(t => t.Service).Name("Service");
            foo.Map(t => t.OldState).Name("Old State");
            foo.Map(t => t.NewState).Name("New State");
            foo.Map(t => t.SID).Name("SID");
            foo.Map(t => t.RecordID).Name("Record ID");
            foo.Map(t => t.ServiceName).Name("Service Name");
            foo.Map(t => t.Action).Name("Action");

            foo.Map(t => t.Line).Ignore();
            foo.Map(t => t.Tag).Ignore();

            csv.Context.RegisterClassMap(foo);

            var records = csv.GetRecords<ChainsawServiceTamperingData>();

            var ln = 1;
            foreach (var record in records)
            {
                Log.Debug("Line # {Line}, Record: {RawRecord}", ln, csv.Context.Parser.RawRecord);

                record.Line = ln;

                record.Tag = TaggedLines.Contains(ln);

                DataList.Add(record);

                ln += 1;
            }
        }

    }

    public class ChainsawAppLockerData : IFileSpecData
    {
        public DateTime Timestamp { get; set; }
        public string Detections { get; set; }
        public string Path { get; set; }

        public int EventID { get; set; }
        public string Channel { get; set; }
        public string Computer { get; set; }
        public string TargetUser { get; set; }
        public string FullFilePath { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{Timestamp} {Detections} {Path} {EventID} {Channel} {Computer} {TargetUser} {FullFilePath}";
        }
    }

    public class ChainsawAppLocker : IFileSpec
    {
        public ChainsawAppLocker()
        {
            //Initialize collections here, one for TaggedLines TLE can add values to, and the collection that TLE will display
            TaggedLines = new List<int>();

            DataList = new BindingList<ChainsawAppLockerData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "timestamp,detections,path,Event ID,Channel,Computer,TargetUser,FullFilePath"
            };
        }

        public string Author => "Reece394";
        public string FileDescription => "CSV generated from Chainsaw - applocker.csv";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "c62ddff9-9cb7-4d86-8c0e-640d1dcda014";

        public void ProcessFile(string filename)
        {
            DataList.Clear();

            using var fileReader = File.OpenText(filename);
            var csv = new CsvReader(fileReader, CultureInfo.InvariantCulture);

            var foo = csv.Context.AutoMap<ChainsawAppLockerData>();

            var o = new TypeConverterOptions
            {
                DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal
            };
            csv.Context.TypeConverterOptionsCache.AddOptions<ChainsawAppLockerData>(o);

            foo.Map(m => m.Timestamp).Convert(row => DateTime.Parse(row.Row.GetField<string>("timestamp")).ToUniversalTime());
            foo.Map(t => t.Detections).Name("detections");
            foo.Map(t => t.Path).Name("path");
            foo.Map(t => t.EventID).Name("Event ID");
            foo.Map(t => t.Channel).Name("Channel");
            foo.Map(t => t.Computer).Name("Computer");
            foo.Map(t => t.TargetUser).Name("TargetUser");
            foo.Map(t => t.FullFilePath).Name("FullFilePath");

            foo.Map(t => t.Line).Ignore();
            foo.Map(t => t.Tag).Ignore();

            csv.Context.RegisterClassMap(foo);

            var records = csv.GetRecords<ChainsawAppLockerData>();

            var ln = 1;
            foreach (var record in records)
            {
                Log.Debug("Line # {Line}, Record: {RawRecord}", ln, csv.Context.Parser.RawRecord);

                record.Line = ln;

                record.Tag = TaggedLines.Contains(ln);

                DataList.Add(record);

                ln += 1;
            }
        }

    }

    public class ChainsawDefenseEvasionData : IFileSpecData
    {
        public DateTime Timestamp { get; set; }
        public string Detections { get; set; }
        public string Path { get; set; }

        public int EventID { get; set; }
        public int? RecordID { get; set; }
        public string Computer { get; set; }
        public string ServiceName { get; set; }
        public string OldStartupType { get; set; }
        public string CurrentStartupType { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return $"{Timestamp} {Detections} {Path} {EventID} {RecordID} {Computer} {ServiceName} {OldStartupType} {CurrentStartupType}";
        }
    }

    public class ChainsawDefenseEvasion : IFileSpec
    {
        public ChainsawDefenseEvasion()
        {
            //Initialize collections here, one for TaggedLines TLE can add values to, and the collection that TLE will display
            TaggedLines = new List<int>();

            DataList = new BindingList<ChainsawDefenseEvasionData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "timestamp,detections,path,Event ID,Record ID,Computer,Service Name,Old Startup Type,Current Startup Type"
            };
        }

        public string Author => "Reece394";
        public string FileDescription => "CSV generated from Chainsaw - defense_evasion.csv";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "287772d7-4aec-4638-b108-46e7419686d6";

        public void ProcessFile(string filename)
        {
            DataList.Clear();

            using var fileReader = File.OpenText(filename);
            var csv = new CsvReader(fileReader, CultureInfo.InvariantCulture);

            var foo = csv.Context.AutoMap<ChainsawDefenseEvasionData>();

            var o = new TypeConverterOptions
            {
                DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal,
                NullValues = {"=\"\""}
            };
            csv.Context.TypeConverterOptionsCache.AddOptions<ChainsawDefenseEvasionData>(o);

            foo.Map(m => m.Timestamp).Convert(row => DateTime.Parse(row.Row.GetField<string>("timestamp")).ToUniversalTime());
            foo.Map(t => t.Detections).Name("detections");
            foo.Map(t => t.Path).Name("path");
            foo.Map(t => t.EventID).Name("Event ID");
            foo.Map(t => t.RecordID).Name("Record ID");
            foo.Map(t => t.Computer).Name("Computer");
            foo.Map(t => t.ServiceName).Name("Service Name");
            foo.Map(t => t.OldStartupType).Name("Old Startup Type");
            foo.Map(t => t.CurrentStartupType).Name("Current Startup Type");

            foo.Map(t => t.Line).Ignore();
            foo.Map(t => t.Tag).Ignore();

            csv.Context.RegisterClassMap(foo);

            var records = csv.GetRecords<ChainsawDefenseEvasionData>();

            var ln = 1;
            foreach (var record in records)
            {
                Log.Debug("Line # {Line}, Record: {RawRecord}", ln, csv.Context.Parser.RawRecord);

                record.Line = ln;

                record.Tag = TaggedLines.Contains(ln);

                DataList.Add(record);

                ln += 1;
            }
        }

    }

    public class ChainsawMFTData : IFileSpecData
    {
        public DateTime Timestamp { get; set; }
        public string Detections { get; set; }
        public string Path { get; set; }
        public string FileNamePath { get; set; }
        public DateTime? StandardInfoLastModified0x10 { get; set; }
        public DateTime? StandardInfoLastAccess0x10 { get; set; }
        public DateTime? FileNameCreated0x30 { get; set; }
        public DateTime? FileNameLastModified0x30 { get; set; }
        public DateTime? FileNameLastAccess0x30 { get; set; }
        public ulong FileSize { get; set; }

        public bool Directory { get; set; }
        public bool Deleted { get; set; }
        public bool HasAlternateDataStreams { get; set; }

        public int Line { get; set; }

        public bool Tag { get; set; }

        public override string ToString()
        {
            return
                $"{Timestamp} {Detections} {Path} {FileNamePath} {StandardInfoLastModified0x10} {StandardInfoLastAccess0x10} {FileNameCreated0x30} {FileNameLastModified0x30} {FileNameLastAccess0x30} {FileSize} {Directory} {Deleted} {HasAlternateDataStreams}";
        }
    }

    public class ChainsawMFT : IFileSpec
    {
        public ChainsawMFT()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<ChainsawMFTData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "timestamp,detections,path,FileNamePath,StandardInfoLastModified0x10,StandardInfoLastAccess0x10,FileNameCreated0x30,FileNameLastModified0x30,FileNameLastAccess0x30,FileSize,IsADirectory,IsDeleted,HasAlternateDataStreams"
            };
        }

        public string Author => "Reece394";
        public string FileDescription => "CSV generated from Chainsaw - mft.csv";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "e9d1c745-972f-40ad-b7f5-5d00d88d9c18";

        public void ProcessFile(string filename)
        {
            DataList.Clear();

            using var fileReader = File.OpenText(filename);
            var csv = new CsvReader(fileReader, CultureInfo.InvariantCulture);

            var foo = csv.Context.AutoMap<ChainsawMFTData>();

            var o = new TypeConverterOptions
            {
                DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal,
                NullValues = {"=\"\""}
            };
            csv.Context.TypeConverterOptionsCache.AddOptions<ChainsawMFTData>(o);

            foo.Map(m => m.Timestamp).Convert(row => DateTime.Parse(row.Row.GetField<string>("timestamp")).ToUniversalTime());
            foo.Map(t => t.Detections).Name("detections");
            foo.Map(t => t.Path).Name("path");
            foo.Map(t => t.FileNamePath).Name("FileNamePath");
            foo.Map(m => m.StandardInfoLastModified0x10).TypeConverterOption
                .DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);
            foo.Map(m => m.StandardInfoLastAccess0x10).TypeConverterOption
                .DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);
            foo.Map(m => m.FileNameCreated0x30).TypeConverterOption
                .DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);
            foo.Map(m => m.FileNameLastModified0x30).TypeConverterOption
                .DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);
            foo.Map(m => m.FileNameLastAccess0x30).TypeConverterOption
                .DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);

            foo.Map(t => t.Directory).Name("IsADirectory");
            foo.Map(t => t.Deleted).Name("IsDeleted");

            foo.Map(t => t.Line).Ignore();
            foo.Map(t => t.Tag).Ignore();

            csv.Context.RegisterClassMap(foo);

            var records = csv.GetRecords<ChainsawMFTData>();

            


            var ln = 1;
            foreach (var record in records)
            {
                Log.Debug("Line # {Line}, Record: {RawRecord}",ln,csv.Context.Parser.RawRecord);
                record.Line = ln;
                record.Tag = TaggedLines.Contains(ln);
                DataList.Add(record);

                ln += 1;
            }
        }
    }

    public class ChainsawMFTDataStreamsData : IFileSpecData
    {
        public DateTime Timestamp { get; set; }
        public string Detections { get; set; }
        public string Path { get; set; }
        public string FileNamePath { get; set; }
        public DateTime? StandardInfoLastModified0x10 { get; set; }
        public DateTime? StandardInfoLastAccess0x10 { get; set; }
        public DateTime? FileNameCreated0x30 { get; set; }
        public DateTime? FileNameLastModified0x30 { get; set; }
        public DateTime? FileNameLastAccess0x30 { get; set; }
        public ulong FileSize { get; set; }

        public bool Directory { get; set; }
        public bool Deleted { get; set; }
        public bool HasAlternateDataStreams { get; set; }
        public string DataStreams { get; set; }

        public int Line { get; set; }

        public bool Tag { get; set; }

        public override string ToString()
        {
            return
                $"{Timestamp} {Detections} {Path} {FileNamePath} {StandardInfoLastModified0x10} {StandardInfoLastAccess0x10} {FileNameCreated0x30} {FileNameLastModified0x30} {FileNameLastAccess0x30} {FileSize} {Directory} {Deleted} {HasAlternateDataStreams} {DataStreams}";
        }
    }

    public class ChainsawMFTDataStreams : IFileSpec
    {
        public ChainsawMFTDataStreams()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<ChainsawMFTDataStreamsData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "timestamp,detections,path,FileNamePath,StandardInfoLastModified0x10,StandardInfoLastAccess0x10,FileNameCreated0x30,FileNameLastModified0x30,FileNameLastAccess0x30,FileSize,IsADirectory,IsDeleted,HasAlternateDataStreams,DataStreams"
            };
        }

        public string Author => "Reece394";
        public string FileDescription => "CSV generated from Chainsaw - mft.csv with DataStreams";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "e959e33c-01c0-4200-aeb9-91e2da42f127";

        public void ProcessFile(string filename)
        {
            DataList.Clear();

            using var fileReader = File.OpenText(filename);
            var csv = new CsvReader(fileReader, CultureInfo.InvariantCulture);

            var foo = csv.Context.AutoMap<ChainsawMFTDataStreamsData>();

            var o = new TypeConverterOptions
            {
                DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal,
                NullValues = {"=\"\""}
            };
            csv.Context.TypeConverterOptionsCache.AddOptions<ChainsawMFTDataStreamsData>(o);

            foo.Map(m => m.Timestamp).Convert(row => DateTime.Parse(row.Row.GetField<string>("timestamp")).ToUniversalTime());
            foo.Map(t => t.Detections).Name("detections");
            foo.Map(t => t.Path).Name("path");
            foo.Map(t => t.FileNamePath).Name("FileNamePath");
            foo.Map(m => m.StandardInfoLastModified0x10).TypeConverterOption
                .DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);
            foo.Map(m => m.StandardInfoLastAccess0x10).TypeConverterOption
                .DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);
            foo.Map(m => m.FileNameCreated0x30).TypeConverterOption
                .DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);
            foo.Map(m => m.FileNameLastModified0x30).TypeConverterOption
                .DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);
            foo.Map(m => m.FileNameLastAccess0x30).TypeConverterOption
                .DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);

            foo.Map(t => t.Directory).Name("IsADirectory");
            foo.Map(t => t.Deleted).Name("IsDeleted");

            foo.Map(t => t.Line).Ignore();
            foo.Map(t => t.Tag).Ignore();

            csv.Context.RegisterClassMap(foo);

            var records = csv.GetRecords<ChainsawMFTDataStreamsData>();

            


            var ln = 1;
            foreach (var record in records)
            {
                Log.Debug("Line # {Line}, Record: {RawRecord}",ln,csv.Context.Parser.RawRecord);
                record.Line = ln;
                record.Tag = TaggedLines.Contains(ln);
                DataList.Add(record);

                ln += 1;
            }
        }
    }

    #endregion

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

            using var fileReader = File.OpenText(filename);
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


            csv.Read();
            csv.ReadHeader();

            var ln = 1;
            while (csv.Read())
            {
                Log.Debug("Line # {Line}, Record: {RawRecord}", ln, csv.Context.Parser.RawRecord);

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

    //public string VisitDuration { get; set; }



    public class BrowsingHistoryViewEventData251AndNewer : IFileSpecData
    {
        public string Url { get; set; }
        public string Title { get; set; }
        public DateTime VisitTime { get; set; }
        public int VisitCount { get; set; }
        public string VisitedFrom { get; set; }
        public string VisitType { get; set; }
        public string VisitDuration { get; set; }
        public string WebBrowser { get; set; }
        public string UserProfile { get; set; }
        public string BrowserProfile { get; set; }
        public int UrlLength { get; set; }
        public int? TypedCount { get; set; }
        public string HistoryFile { get; set; }
        public int RecordId { get; set; }


        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return
                $"{Url} {Title} {VisitTime} {VisitedFrom} {VisitCount} {VisitType} {VisitDuration} {WebBrowser} {UserProfile} {BrowserProfile} {UrlLength} {TypedCount} {HistoryFile} {RecordId}";
        }
    }


    public class BrowsingHistoryView251AndNewer : IFileSpec
    {
        public BrowsingHistoryView251AndNewer()
        {
            //Initialize collections here, one for TaggedLines TLE can add values to, and the collection that TLE will display
            TaggedLines = new List<int>();

            DataList = new BindingList<BrowsingHistoryViewEventData251AndNewer>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "URL,Title,Visit Time,Visit Count,Visited From,Visit Type,Visit Duration,Web Browser,User Profile,Browser Profile,URL Length,Typed Count,History File,Record ID"
            };
        }

        //Version 2.51:
        //Added 'Visit Duration' column. This column is available only for Chrome and Chromium-based Web browsers.
        //Improved the 'Visited From' column in new versions of Chrome.

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from BrowsingHistoryView 2.51 or newer";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "cba7c7d7-eefd-431d-898c-07b9ea93c120";

        public void ProcessFile(string filename)
        {
            DataList.Clear();

            using var fileReader = File.OpenText(filename);
            var csv = new CsvReader(fileReader, CultureInfo.InvariantCulture);


            var o = new TypeConverterOptions
            {
                DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal
            };
            csv.Context.TypeConverterOptionsCache.AddOptions<BrowsingHistoryViewEventData251AndNewer>(o);


            var foo = csv.Context.AutoMap<BrowsingHistoryViewEventData251AndNewer>();

            //URL,Title,Visit Time,Visit Count,Visited From,Visit Type,Web Browser,User Profile,Browser Profile,URL Length,Typed Count,History File,Record ID
            //file:///C:/Program%20Files/Commvault/ContentStore/Reports/BackupJobSummaryReport_15460_670556_7872_1618863877.html,,4/19/2021 8:24:43 PM,4,,,Internet Explorer 10/11 / Edge,jasonb,,114,,H:\C\Users\jasonb\AppData\Local\Microsoft\Windows\WebCache\WebCacheV01.dat,212

            foo.Map(t => t.Url).Name("URL");

            foo.Map(m => m.VisitTime).Convert(row =>
                DateTime.Parse(row.Row.GetField<string>("Visit Time")).ToUniversalTime());

            foo.Map(t => t.VisitCount).Name("Visit Count");
            foo.Map(t => t.VisitedFrom).Name("Visited From");
            foo.Map(t => t.VisitType).Name("Visit Type");
            foo.Map(t => t.VisitDuration).Name("Visit Duration");
            foo.Map(t => t.WebBrowser).Name("Web Browser");
            foo.Map(t => t.UserProfile).Name("User Profile");
            foo.Map(t => t.BrowserProfile).Name("Browser Profile");
            foo.Map(t => t.UrlLength).Name("URL Length");
            foo.Map(t => t.TypedCount).Name("Typed Count");
            foo.Map(t => t.HistoryFile).Name("History File");
            foo.Map(t => t.RecordId).Name("Record ID");


            foo.Map(t => t.Line).Ignore();
            foo.Map(t => t.Tag).Ignore();

            csv.Context.RegisterClassMap(foo);


            var records = csv.GetRecords<BrowsingHistoryViewEventData251AndNewer>();

            var ln = 1;
            foreach (var record in records)
            {
                Log.Debug("Line # {Line}, Record: {RawRecord}", ln, csv.Context.Parser.RawRecord);

                record.Line = ln;

                record.Tag = TaggedLines.Contains(ln);

                DataList.Add(record);

                ln += 1;
            }
        }
    }

    public class BrowsingHistoryViewEventData250AndOlder : IFileSpecData
    {
        public string Url { get; set; }
        public string Title { get; set; }
        public DateTime VisitTime { get; set; }
        public int VisitCount { get; set; }
        public string VisitedFrom { get; set; }
        public string VisitType { get; set; }
        public string WebBrowser { get; set; }
        public string UserProfile { get; set; }
        public string BrowserProfile { get; set; }
        public int UrlLength { get; set; }
        public int? TypedCount { get; set; }
        public string HistoryFile { get; set; }
        public int RecordId { get; set; }


        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return
                $"{Url} {Title} {VisitTime} {VisitedFrom} {VisitCount} {VisitType} {WebBrowser} {UserProfile} {BrowserProfile} {UrlLength} {TypedCount} {HistoryFile} {RecordId}";
        }
    }


    public class BrowsingHistoryView250AndOlder : IFileSpec
    {
        public BrowsingHistoryView250AndOlder()
        {
            //Initialize collections here, one for TaggedLines TLE can add values to, and the collection that TLE will display
            TaggedLines = new List<int>();

            DataList = new BindingList<BrowsingHistoryViewEventData250AndOlder>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "url,title,visit time,visit count,visited from,visit type,web browser,user profile,browser profile,url length,typed count,history file,record id"
            };
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "CSV generated from BrowsingHistoryView 2.50 or older";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "30a260e2-0630-4937-9ab7-c3f86bdee49d";

        public void ProcessFile(string filename)
        {
            DataList.Clear();

            using var fileReader = File.OpenText(filename);
            var csv = new CsvReader(fileReader, CultureInfo.InvariantCulture);


            var o = new TypeConverterOptions
            {
                DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal
            };
            csv.Context.TypeConverterOptionsCache.AddOptions<BrowsingHistoryViewEventData250AndOlder>(o);


            var foo = csv.Context.AutoMap<BrowsingHistoryViewEventData250AndOlder>();

            //URL,Title,Visit Time,Visit Count,Visited From,Visit Type,Web Browser,User Profile,Browser Profile,URL Length,Typed Count,History File,Record ID
            //file:///C:/Program%20Files/Commvault/ContentStore/Reports/BackupJobSummaryReport_15460_670556_7872_1618863877.html,,4/19/2021 8:24:43 PM,4,,,Internet Explorer 10/11 / Edge,jasonb,,114,,H:\C\Users\jasonb\AppData\Local\Microsoft\Windows\WebCache\WebCacheV01.dat,212

            foo.Map(t => t.Url).Name("URL");

            foo.Map(m => m.VisitTime).Convert(row =>
                DateTime.Parse(row.Row.GetField<string>("Visit Time")).ToUniversalTime());

            foo.Map(t => t.VisitCount).Name("Visit Count");
            foo.Map(t => t.VisitedFrom).Name("Visited From");
            foo.Map(t => t.VisitType).Name("Visit Type");
            foo.Map(t => t.WebBrowser).Name("Web Browser");
            foo.Map(t => t.UserProfile).Name("User Profile");
            foo.Map(t => t.BrowserProfile).Name("Browser Profile");
            foo.Map(t => t.UrlLength).Name("URL Length");
            foo.Map(t => t.TypedCount).Name("Typed Count");
            foo.Map(t => t.HistoryFile).Name("History File");
            foo.Map(t => t.RecordId).Name("Record ID");


            foo.Map(t => t.Line).Ignore();
            foo.Map(t => t.Tag).Ignore();

            csv.Context.RegisterClassMap(foo);


            var records = csv.GetRecords<BrowsingHistoryViewEventData250AndOlder>();

            var ln = 1;
            foreach (var record in records)
            {
                Log.Debug("Line # {Line}, Record: {RawRecord}", ln, csv.Context.Parser.RawRecord);

                record.Line = ln;

                record.Tag = TaggedLines.Contains(ln);

                DataList.Add(record);

                ln += 1;
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

            using var fileReader = File.OpenText(filename);
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


            var records = csv.GetRecords<CrowdStrikeEventData>();

            var ln = 1;
            foreach (var record in records)
            {
                Log.Debug("Line # {Line}, Record: {RawRecord}", ln, csv.Context.Parser.RawRecord);

                record.Line = ln;

                record.Tag = TaggedLines.Contains(ln);

                DataList.Add(record);

                ln += 1;
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

            using var fileReader = File.OpenText(filename);
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


            csv.Read();
            csv.ReadHeader();

            var ln = 1;
            while (csv.Read())
            {
                Log.Debug("Line # {Line}, Record: {RawRecord}", ln, csv.Context.Parser.RawRecord);

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

            using var fileReader = File.OpenText(filename);
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


            csv.Read();
            csv.ReadHeader();

            var ln = 1;
            while (csv.Read())
            {
                Log.Debug("Line # {Line}, Record: {RawRecord}", ln, csv.Context.Parser.RawRecord);

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

            using var fileReader = File.OpenText(filename);
            var ln = fileReader.ReadLine();
            var ln1 = 1;
            while (ln != null)
            {
                Log.Debug($"Line # {ln1}, Record: {ln}", ln1, ln);

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

    public class VanillaWindowsReferenceData : IFileSpecData
    {
        public string DirectoryName { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public int Length { get; set; }
        public DateTime CreationTimeUtc { get; set; }
        public DateTime LastAccessTimeUtc { get; set; }
        public DateTime LastWriteTimeUtc { get; set; }
        public string Attributes { get; set; }
        public string Md5 { get; set; }
        public string Sha256 { get; set; }
        public string Sddl { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return
                $"{DirectoryName} {Name} {FullName} {Length} {CreationTimeUtc} {LastAccessTimeUtc} {LastWriteTimeUtc} {Attributes} {Md5} {Sha256} {Sddl}";
        }
    }

    public class VanillaWindowsReference : IFileSpec
    {
        public VanillaWindowsReference()
        {
            //Initialize collections here, one for TaggedLines TLE can add values to, and the collection that TLE will display
            TaggedLines = new List<int>();

            DataList = new BindingList<VanillaWindowsReferenceData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "\"directoryname\",\"name\",\"fullname\",\"length\",\"creationtimeutc\",\"lastaccesstimeutc\",\"lastwritetimeutc\",\"attributes\",\"md5\",\"sha256\",\"sddl\""
            };
        }

        public string Author => "Andrew Rathbun";

        public string FileDescription =>
            "CSV generated from AndrewRathbun/VanillaWindowsReference"; //https://github.com/AndrewRathbun/VanillaWindowsReference

        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "e8c703e2-a34c-45df-a444-b133a679ed7a";

        public void ProcessFile(string filename)
        {
            DataList.Clear();

            using var fileReader = File.OpenText(filename);
            var csv = new CsvReader(fileReader, CultureInfo.InvariantCulture);


            var o = new TypeConverterOptions
            {
                DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal
            };
            csv.Context.TypeConverterOptionsCache.AddOptions<VanillaWindowsReferenceData>(o);

            var foo = csv.Context.AutoMap<VanillaWindowsReferenceData>();

            //"DirectoryName","Name","FullName","Length","CreationTimeUtc","LastAccessTimeUtc","LastWriteTimeUtc","Attributes","MD5","SHA256","Sddl"
            //"C:\","PsExec_IgnoreThisFile_ResearchTool.exe","C:\PsExec_IgnoreThisFile_ResearchTool.exe","834936","2021-11-21 20:34:17","2021-11-21 20:34:17","2021-05-25 21:40:08","Archive","C590A84B8C72CF18F35AE166F815C9DF","57492D33B7C0755BB411B22D2DFDFDF088CBBFCD010E30DD8D425D5FE66ADFF4","O:BAG:S-1-5-21-3499336306-2590357158-289705316-513D:AI(A;ID;FA;;;BA)(A;ID;FA;;;SY)(A;ID;0x1200a9;;;BU)(A;ID;0x1301bf;;;AU)"

            //foo.Map(t => t.DirectoryName).Name("DirectoryName");

            //foo.Map(t => t.Name).Name("Filename");

            //foo.Map(t => t.FullName).Name("File Path");

            //foo.Map(t => t.Length).Name("Size (Bytes)");

            //foo.Map(t => t.DirectoryName).Name("Directory Name");

            //foo.Map(t => t.CreationTimeUtc).Convert(row =>
            //       DateTime.Parse(row.Row.GetField<string>("Creation Time UTC")));
            //foo.Map(t => t.LastAccessTimeUtc).Convert(row =>
            //       DateTime.Parse(row.Row.GetField<string>("Last Access Time UTC")));
            // foo.Map(t => t.LastWriteTimeUtc).Convert(row =>
            //       DateTime.Parse(row.Row.GetField<string>("Last Write Time UTC")));
            //foo.Map(t => t.Attributes).Name("Attributes");
            foo.Map(t => t.Md5).Name("MD5");
            foo.Map(t => t.Sha256).Name("SHA256");
            //foo.Map(t => t.Sddl).Name("Sddl");

            foo.Map(t => t.Line).Ignore();
            foo.Map(t => t.Tag).Ignore();

            csv.Context.RegisterClassMap(foo);


            var records = csv.GetRecords<VanillaWindowsReferenceData>();

            var ln = 1;
            foreach (var record in records)
            {
                Log.Debug("Line # {Line}, Record: {RawRecord}", ln, csv.Context.Parser.RawRecord);

                record.Line = ln;

                record.Tag = TaggedLines.Contains(ln);

                DataList.Add(record);

                ln += 1;
            }
        }
    }

    public class EVTXETWResourcesData : IFileSpecData
    {
        public int EventID { get; set; }
        public int EventVersion { get; set; }
        public string Level { get; set; }
        public string Channel { get; set; }
        public string Task { get; set; }
        public string Opcode { get; set; }
        public string Keyword { get; set; }
        public string Windows { get; set; }
        public string Version { get; set; }
        public string Edition { get; set; }
        public DateTime Date { get; set; }
        public float Build { get; set; }
        public string EventMessage { get; set; }
        public string EventFields { get; set; }


        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return
                $"{EventID} {EventVersion} {Level} {Channel} {Task} {Opcode} {Keyword} {Windows} {Version} {Edition} {Date} {Build} {EventMessage} {EventFields}";
        }
    }

    public class EVTXETWResources : IFileSpec
    {
        public EVTXETWResources()
        {
            //Initialize collections here, one for TaggedLines TLE can add values to, and the collection that TLE will display
            TaggedLines = new List<int>();

            DataList = new BindingList<EVTXETWResourcesData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Event ID,Event Version,Level,Channel,Task,Opcode,Keyword,Windows,Version,Edition,Date,Build,Event Message,Event Fields"
            };
        }

        public string Author => "Andrew Rathbun";

        public string FileDescription =>
            "CSV generated from nasbench/EVTX-ETW-Resources/ETWProvidersCSVs"; //https://github.com/nasbench/EVTX-ETW-Resources/tree/main/ETWProvidersCSVs

        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "8e99eb91-8056-41bc-a65f-6db43b1a692a";

        public void ProcessFile(string filename)
        {
            DataList.Clear();

            using var fileReader = File.OpenText(filename);
            var csv = new CsvReader(fileReader, CultureInfo.InvariantCulture);


            var o = new TypeConverterOptions
            {
                DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal
            };
            csv.Context.TypeConverterOptionsCache.AddOptions<EVTXETWResourcesData>(o);

            var foo = csv.Context.AutoMap<EVTXETWResourcesData>();

            //foo.Map(t => t.Url).Name("URL");

            foo.Map(m => m.Date).Convert(row =>
                DateTime.Parse(row.Row.GetField<string>("Date")).ToUniversalTime());

            //"Event ID,Event Version,Level,Channel,Task,Opcode,Keyword,Windows,Version,Edition,Date,Build,Event Message,Event Fields"

            foo.Map(t => t.EventID).Name("Event ID");
            foo.Map(t => t.EventVersion).Name("Event Version");
            foo.Map(t => t.Level).Name("Level");
            foo.Map(t => t.Channel).Name("Channel");
            foo.Map(t => t.Task).Name("Task");
            foo.Map(t => t.Opcode).Name("Opcode");
            foo.Map(t => t.Keyword).Name("Keyword");
            foo.Map(t => t.Windows).Name("Windows");
            foo.Map(t => t.Version).Name("Version");
            foo.Map(t => t.Edition).Name("Edition");
            //foo.Map(t => t.Date).Name("Date");
            foo.Map(t => t.Build).Name("Build");
            foo.Map(t => t.Version).Name("Version");
            foo.Map(t => t.EventMessage).Name("Event Message");
            foo.Map(t => t.EventFields).Name("Event Fields");

            foo.Map(t => t.Line).Ignore();
            foo.Map(t => t.Tag).Ignore();

            csv.Context.RegisterClassMap(foo);

            var records = csv.GetRecords<EVTXETWResourcesData>();

            var ln = 1;
            foreach (var record in records)
            {
                Log.Debug("Line # {Line}, Record: {RawRecord}", ln, csv.Context.Parser.RawRecord);

                record.Line = ln;

                record.Tag = TaggedLines.Contains(ln);

                DataList.Add(record);

                ln += 1;
            }
        }
    }
    
        public class ConvertPSHistoryToCSVData : IFileSpecData
    {
        // https://gist.github.com/Qazeer/a0c1c14bb1eae233c1147d1d9dfb3e93
        public string User { get; set; }
        public int CommandIndex { get; set; }
        public string Command { get; set; }
        public DateTime ExecutionTimestamp { get; set; }
        public string File { get; set; }

        public int Line { get; set; }
        public bool Tag { get; set; }

        public override string ToString()
        {
            return
                $"{User} {CommandIndex} {Command} {ExecutionTimestamp} {File}";
        }
    }

    public class ConvertPSHistoryToCSV : IFileSpec
    {
        // https://gist.github.com/Qazeer/a0c1c14bb1eae233c1147d1d9dfb3e93
        public ConvertPSHistoryToCSV()
        {
            //Initialize collections here, one for TaggedLines TLE can add values to, and the collection that TLE will display
            TaggedLines = new List<int>();

            DataList = new BindingList<ConvertPSHistoryToCSVData>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "\"User\",\"CommandIndex\",\"Command\",\"ExecutionTimestamp\",\"File\""
            };
        }

        public string Author => "Andrew Rathbun";

        public string FileDescription =>
            "CSV generated from Qazeer/ConvertPSHistoryTo-CSV.ps1"; //https://gist.github.com/Qazeer/a0c1c14bb1eae233c1147d1d9dfb3e93

        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "63B74A10-2ECE-4314-82F0-85E83146FEE1";

        public void ProcessFile(string filename)
        {
            DataList.Clear();

            using var fileReader = File.OpenText(filename);
            var csv = new CsvReader(fileReader, CultureInfo.InvariantCulture);

            var o = new TypeConverterOptions
            {
                DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal
            };
            csv.Context.TypeConverterOptionsCache.AddOptions<ConvertPSHistoryToCSVData>(o);

            var foo = csv.Context.AutoMap<ConvertPSHistoryToCSVData>();

            //foo.Map(t => t.Url).Name("URL");

            //"User","CommandIndex","Command","ExecutionTimestamp","File"

            foo.Map(t => t.User).Name("User");
            foo.Map(t => t.CommandIndex).Name("CommandIndex");
            foo.Map(t => t.Command).Name("Command");
            foo.Map(m => m.ExecutionTimestamp).Convert(args =>
            {
                var executionTimestamp = args.Row.GetField<string>("ExecutionTimestamp");

                if (string.IsNullOrEmpty(executionTimestamp))
                {
                    return default(DateTime);
                }

                DateTime result;
                return DateTime.TryParseExact(executionTimestamp, "yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out result)
                    ? result
                    : default(DateTime);
            });

            foo.Map(t => t.File).Name("File");

            foo.Map(t => t.Line).Ignore();
            foo.Map(t => t.Tag).Ignore();

            csv.Context.RegisterClassMap(foo);

            var records = csv.GetRecords<ConvertPSHistoryToCSVData>();

            var ln = 1;
            try
            {
                foreach (var record in records)
                {
                    Console.WriteLine("Line # {0}, Record: {1}", ln, csv.Context.Parser.RawRecord);
                    Log.Debug("Line # {Line}, Record: {RawRecord}",ln,csv.Context.Parser.RawRecord);

                    record.Line = ln;

                    record.Tag = TaggedLines.Contains(ln);

                    DataList.Add(record);

                    ln += 1;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error processing line #{0}: {1}", ln, e.Message);
            }
        }
    }
}
