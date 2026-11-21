using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using ITLEFileSpec;
using TLEFilePlugins.Shared;
using Serilog;
using Serilog.Events;

namespace TLEFileMisc
{

    #region Hayabusa

    
    [CsvRecord]
    public partial class HayabusaStandardData : IFileSpecData
    {
        [CsvColumn(Optional = true), CsvConverter(typeof(UniversalParseTimeConverter))] public DateTime Timestamp { get; set; }
        [CsvColumn(Optional = true)] public string RuleTitle { get; set; }
        [CsvColumn(Optional = true)] public string Level { get; set; }
        [CsvColumn(Optional = true)] public string Computer { get; set; }
        [CsvColumn(Optional = true)] public string Channel { get; set; }
        
        [CsvColumn("EventID", Optional = true)] public int EventId { get; set; }
        [CsvColumn("RecordID", Optional = true)] public ulong RecordId { get; set; }
        [CsvColumn(Optional = true)] public string Details { get; set; }
        [CsvColumn(Optional = true)] public string ExtraFieldInfo { get; set; }
        
        [CsvIgnore] public int Line { get; set; }
        [CsvIgnore] public bool Tag { get; set; }

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
            CsvLoader.Load<HayabusaStandardData>(filename, DataList, TaggedLines);
        }
        
    }
    
    
    [CsvRecord]
    public partial class HayabusaVerboseData : IFileSpecData
    {
        
        [CsvColumn(Optional = true), CsvConverter(typeof(UniversalParseTimeConverter))] public DateTime Timestamp { get; set; }
        [CsvColumn(Optional = true)] public string RuleTitle { get; set; }
        [CsvColumn(Optional = true)] public string Level { get; set; }
        [CsvColumn(Optional = true)] public string Computer { get; set; }
        [CsvColumn(Optional = true)] public string Channel { get; set; }
        
        [CsvColumn("EventID", Optional = true)] public int EventId { get; set; }
        
        [CsvColumn(Optional = true)] public string MitreTactics { get; set; }
        [CsvColumn(Optional = true)] public string MitreTags { get; set; }
        [CsvColumn(Optional = true)] public string OtherTags { get; set; }
        
        [CsvColumn("RecordID", Optional = true)] public ulong RecordId { get; set; }
        [CsvColumn(Optional = true)] public string Details { get; set; }
        [CsvColumn(Optional = true)] public string ExtraFieldInfo { get; set; }
        [CsvColumn(Optional = true)] public string RuleFile { get; set; }
        [CsvColumn(Optional = true)] public string EvtxFile { get; set; }
        
        [CsvIgnore] public int Line { get; set; }
        [CsvIgnore] public bool Tag { get; set; }

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
            CsvLoader.Load<HayabusaVerboseData>(filename, DataList, TaggedLines);
        }
        
    }
    
    
    [CsvRecord]
    public partial class HayabusaSuperVerboseData : IFileSpecData
    {
        //"EventID","RuleAuthor","RuleModifiedDate","Status",
        //"ExtraFieldInfo","MitreTactics","MitreTags","OtherTags","Provider","RuleCreationDate","RuleFile","EvtxFile"
        [CsvColumn(Optional = true), CsvConverter(typeof(UniversalParseTimeConverter))] public DateTime Timestamp { get; set; }
        [CsvColumn(Optional = true)] public string RuleTitle { get; set; }
        [CsvColumn(Optional = true)] public string Level { get; set; }
        [CsvColumn(Optional = true)] public string Computer { get; set; }
        [CsvColumn(Optional = true)] public string Channel { get; set; }
        
        [CsvColumn("EventID", Optional = true)] public int EventId { get; set; }
        
        [CsvColumn(Optional = true)] public string RuleAuthor { get; set; }
        [CsvColumn(Optional = true)] public string RuleModifiedDate { get; set; }
        [CsvColumn(Optional = true)] public string Status { get; set; }
        [CsvColumn("RecordID", Optional = true)] public ulong RecordId { get; set; }
        [CsvColumn(Optional = true)] public string Details { get; set; }
        [CsvColumn(Optional = true)] public string ExtraFieldInfo { get; set; }
        [CsvColumn(Optional = true)] public string MitreTactics { get; set; }
        [CsvColumn(Optional = true)] public string MitreTags { get; set; }
        [CsvColumn(Optional = true)] public string OtherTags { get; set; }
        [CsvColumn(Optional = true)] public string Provider { get; set; }
        [CsvColumn(Optional = true)] public string RuleCreationDate { get; set; }
        [CsvColumn(Optional = true)] public string RuleFile { get; set; }
        [CsvColumn(Optional = true)] public string EvtxFile { get; set; }
        
        //"2020-07-09 21:22:31.163 +00:00","Active Directory User Backdoors","high","rootdc1.offsec.lan","Sec",5136,"@neu5ron",
        //"2024/02/26","test",15779594,"User: lambda-user ¦ SID: S-1-5-21-4230534742-2542757381-3142984815-1158 ¦ ObjDN: CN=honey-pot1,OU=Test-OU,OU=OFFSEC-COMPANY,DC=offsec,DC=lan ¦ AttrLDAPName: servicePrincipalName ¦ OpType: %%14674 ¦ LID: 0x6529663","AppCorrelationID: - ¦ AttributeSyntaxOID: 2.5.5.12 ¦ AttributeValue: HTTP/HACK-ME-PC.offsec.lan ¦ DSName: offsec.lan ¦ DSType: %%14676 ¦ ObjectClass: user ¦ ObjectGUID: 259162F1-58E4-4EE9-9B9C-2BAF2A03D376 ¦ OpCorrelationID: 52BFBF59-4CF4-4D5E-98D1-09D1EBE12FDE ¦ SubjectDomainName: OFFSEC",
        //"Persis","T1098","","Sec","2017/04/13","win_security_alert_ad_user_backdoors.yml","C:\Users\KOHDA\Downloads\tmp\ID4738,5136-SPN set on user account.evtx"
        
        [CsvIgnore] public int Line { get; set; }
        [CsvIgnore] public bool Tag { get; set; }

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
            CsvLoader.Load<HayabusaSuperVerboseData>(filename, DataList, TaggedLines);
        }
        
        /// <summary>(not used) a value with a '-' is null</summary>
        public class StupidDateConverter : ICsvConverter<string>
        {
            public string Read(CsvReader reader, int index)
            {
                var text = reader.GetString(index);
                return text.Contains("-") ? null : text;
            }

            public void Write(CsvWriter writer, string value)
            {
                writer.WriteField(value ?? string.Empty);
            }
        }
        
    }
    
    [CsvRecord]
    public partial class HayabusaMinimalData : IFileSpecData
    {
        [CsvColumn(Optional = true), CsvConverter(typeof(UniversalParseTimeConverter))] public DateTime Timestamp { get; set; }
        [CsvColumn(Optional = true)] public string RuleTitle { get; set; }
        [CsvColumn(Optional = true)] public string Level { get; set; }
        [CsvColumn(Optional = true)] public string Computer { get; set; }
        [CsvColumn(Optional = true)] public string Channel { get; set; }
        
        [CsvColumn("EventID", Optional = true)] public int EventId { get; set; }
        [CsvColumn("RecordID", Optional = true)] public ulong RecordId { get; set; }
        [CsvColumn(Optional = true)] public string Details { get; set; }
        
        [CsvIgnore] public int Line { get; set; }
        [CsvIgnore] public bool Tag { get; set; }

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
            CsvLoader.Load<HayabusaMinimalData>(filename, DataList, TaggedLines);
        }
        
    }

    #endregion


    #region Chainsaw


    [CsvRecord]
    public partial class ChainsawSigmaData : IFileSpecData
    {
        [CsvColumn(Optional = true), CsvConverter(typeof(UniversalParseTimeConverter))] public DateTime Timestamp { get; set; }
        [CsvColumn("detections", Optional = true)] public string Detections { get; set; }
        [CsvColumn("path", Optional = true)] public string Path { get; set; }
        [CsvColumn("count", Optional = true)] public int Count { get; set; }
        [CsvColumn("Event.System.Provider", Optional = true)] public string EventSystemProvider { get; set; }

        [CsvColumn("Event ID", Optional = true)] public int? EventID { get; set; }
        [CsvColumn("Record ID", Optional = true)] public ulong? RecordID { get; set; }
        [CsvColumn(Optional = true)] public string Computer { get; set; }
        [CsvColumn("Event Data", Optional = true)] public string EventData { get; set; }

        [CsvIgnore] public int Line { get; set; }
        [CsvIgnore] public bool Tag { get; set; }

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
            CsvLoader.Load<ChainsawSigmaData>(filename, DataList, TaggedLines);
        }

    }

    [CsvRecord]
    public partial class ChainsawAntivirusData : IFileSpecData
    {
        [CsvColumn(Optional = true), CsvConverter(typeof(UniversalParseTimeConverter))] public DateTime Timestamp { get; set; }
        [CsvColumn("detections", Optional = true)] public string Detections { get; set; }
        [CsvColumn("path", Optional = true)] public string Path { get; set; }

        [CsvColumn("Event ID", Optional = true)] public int EventID { get; set; }
        [CsvColumn("Record ID", Optional = true)] public ulong? RecordID { get; set; }
        [CsvColumn("Computer", Optional = true)] public string Computer { get; set; }
        [CsvColumn("Threat Name", Optional = true)] public string ThreatName { get; set; }
        [CsvColumn("Threat Path", Optional = true)] public string ThreatPath { get; set; }
        [CsvColumn("SHA1", Optional = true)] public string SHA1 { get; set; }
        [CsvColumn("User", Optional = true)] public string User { get; set; }
        [CsvColumn("Threat Type", Optional = true)] public string ThreatType { get; set; }

        [CsvIgnore] public int Line { get; set; }
        [CsvIgnore] public bool Tag { get; set; }

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
            CsvLoader.Load<ChainsawAntivirusData>(filename, DataList, TaggedLines);
        }

    }

    [CsvRecord]
    public partial class ChainsawAccountTamperingData : IFileSpecData
    {
        [CsvColumn(Optional = true), CsvConverter(typeof(UniversalParseTimeConverter))] public DateTime Timestamp { get; set; }
        [CsvColumn("detections", Optional = true)] public string Detections { get; set; }
        [CsvColumn("path", Optional = true)] public string Path { get; set; }

        [CsvColumn("Event ID", Optional = true)] public int EventID { get; set; }
        [CsvColumn("Record ID", Optional = true)] public ulong? RecordID { get; set; }
        [CsvColumn("Computer", Optional = true)] public string Computer { get; set; }
        [CsvColumn("User", Optional = true)] public string User { get; set; }
        [CsvColumn("User SID", Optional = true)] public string UserSID { get; set; }
        [CsvColumn("Member SID", Optional = true)] public string MemberSID { get; set; }

        [CsvIgnore] public int Line { get; set; }
        [CsvIgnore] public bool Tag { get; set; }

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
            CsvLoader.Load<ChainsawAccountTamperingData>(filename, DataList, TaggedLines);
        }

    }

    [CsvRecord]
    public partial class ChainsawCredentialAccessData : IFileSpecData
    {
        [CsvColumn(Optional = true), CsvConverter(typeof(UniversalParseTimeConverter))] public DateTime Timestamp { get; set; }
        [CsvColumn("detections", Optional = true)] public string Detections { get; set; }
        [CsvColumn("path", Optional = true)] public string Path { get; set; }

        [CsvColumn("Event ID", Optional = true)] public int EventID { get; set; }
        [CsvColumn("Computer", Optional = true)] public string Computer { get; set; }
        [CsvColumn("User", Optional = true)] public string User { get; set; }
        [CsvColumn("Service Name", Optional = true)] public string ServiceName { get; set; }
        [CsvColumn("IP Address", Optional = true)] public string IPAddress { get; set; }

        [CsvIgnore] public int Line { get; set; }
        [CsvIgnore] public bool Tag { get; set; }

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
            CsvLoader.Load<ChainsawCredentialAccessData>(filename, DataList, TaggedLines);
        }

    }

    [CsvRecord]
    public partial class ChainsawLateralMovementData : IFileSpecData
    {
        [CsvColumn(Optional = true), CsvConverter(typeof(UniversalParseTimeConverter))] public DateTime Timestamp { get; set; }
        [CsvColumn("detections", Optional = true)] public string Detections { get; set; }
        [CsvColumn("path", Optional = true)] public string Path { get; set; }

        [CsvColumn("Event ID", Optional = true)] public int EventID { get; set; }
        [CsvColumn("Record ID", Optional = true)] public ulong? RecordID { get; set; }
        [CsvColumn("Computer", Optional = true)] public string Computer { get; set; }
        [CsvColumn("User", Optional = true)] public string User { get; set; }
        [CsvColumn("Logon Type", Optional = true)] public string LogonType { get; set; }
        [CsvColumn("IP Address", Optional = true)] public string IPAddress { get; set; }
        [CsvColumn("Process Name", Optional = true)] public string ProcessName { get; set; }
        [CsvColumn("Process ID", Optional = true)] public string ProcessID { get; set; }
        [CsvColumn("Interface Index", Optional = true)] public string InterfaceIndex { get; set; }
        [CsvColumn("Source Address", Optional = true)] public string SourceAddress { get; set; }
        [CsvColumn("Source Port", Optional = true)] public string SourcePort { get; set; }
        [CsvColumn("Dest Address", Optional = true)] public string DestAddress { get; set; }
        [CsvColumn("Dest Port", Optional = true)] public string DestPort { get; set; }

        [CsvIgnore] public int Line { get; set; }
        [CsvIgnore] public bool Tag { get; set; }

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
            CsvLoader.Load<ChainsawLateralMovementData>(filename, DataList, TaggedLines);
        }

    }

    [CsvRecord]
    public partial class ChainsawLateralMovementOldData : IFileSpecData
    {
        [CsvColumn(Optional = true), CsvConverter(typeof(UniversalParseTimeConverter))] public DateTime Timestamp { get; set; }
        [CsvColumn("detections", Optional = true)] public string Detections { get; set; }
        [CsvColumn("path", Optional = true)] public string Path { get; set; }

        [CsvColumn("Event ID", Optional = true)] public int EventID { get; set; }
        [CsvColumn("Record ID", Optional = true)] public ulong? RecordID { get; set; }
        [CsvColumn("Computer", Optional = true)] public string Computer { get; set; }
        [CsvColumn("User", Optional = true)] public string User { get; set; }
        [CsvColumn("Logon Type", Optional = true)] public string LogonType { get; set; }
        [CsvColumn("IP Address", Optional = true)] public string IPAddress { get; set; }

        [CsvIgnore] public int Line { get; set; }
        [CsvIgnore] public bool Tag { get; set; }

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
            CsvLoader.Load<ChainsawLateralMovementOldData>(filename, DataList, TaggedLines);
        }

    }

    [CsvRecord]
    public partial class ChainsawRDSPowerShellScriptData : IFileSpecData
    {
        [CsvColumn(Optional = true), CsvConverter(typeof(UniversalParseTimeConverter))] public DateTime Timestamp { get; set; }
        [CsvColumn("detections", Optional = true)] public string Detections { get; set; }
        [CsvColumn("path", Optional = true)] public string Path { get; set; }

        [CsvColumn("Event ID", Optional = true)] public int EventID { get; set; }
        [CsvColumn("Channel", Optional = true)] public string Channel { get; set; }
        [CsvColumn("Computer", Optional = true)] public string Computer { get; set; }
        [CsvColumn("Information", Optional = true)] public string Information { get; set; }

        [CsvIgnore] public int Line { get; set; }
        [CsvIgnore] public bool Tag { get; set; }

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
            CsvLoader.Load<ChainsawRDSPowerShellScriptData>(filename, DataList, TaggedLines);
        }

    }

    [CsvRecord]
    public partial class ChainsawRDSWebAccessData : IFileSpecData
    {
        [CsvColumn(Optional = true), CsvConverter(typeof(UniversalParseTimeConverter))] public DateTime Timestamp { get; set; }
        [CsvColumn("detections", Optional = true)] public string Detections { get; set; }
        [CsvColumn("path", Optional = true)] public string Path { get; set; }

        [CsvColumn("Event ID", Optional = true)] public int EventID { get; set; }
        [CsvColumn("Channel", Optional = true)] public string Channel { get; set; }
        [CsvColumn("Computer", Optional = true)] public string Computer { get; set; }
        [CsvColumn("IpAddress", Optional = true)] public string IPAddress { get; set; }
        [CsvColumn("LogonType", Optional = true)] public string LogonType { get; set; }
        [CsvColumn("TargetUserName", Optional = true)] public string TargetUserName { get; set; }
        [CsvColumn("WorkstationName", Optional = true)] public string WorkstationName { get; set; }

        [CsvIgnore] public int Line { get; set; }
        [CsvIgnore] public bool Tag { get; set; }

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
            CsvLoader.Load<ChainsawRDSWebAccessData>(filename, DataList, TaggedLines);
        }

    }

    [CsvRecord]
    public partial class ChainsawLogTamperingData : IFileSpecData
    {
        [CsvColumn(Optional = true), CsvConverter(typeof(UniversalParseTimeConverter))] public DateTime Timestamp { get; set; }
        [CsvColumn("detections", Optional = true)] public string Detections { get; set; }
        [CsvColumn("path", Optional = true)] public string Path { get; set; }

        [CsvColumn("Event ID", Optional = true)] public int EventID { get; set; }
        [CsvColumn("Record ID", Optional = true)] public ulong? RecordID { get; set; }
        [CsvColumn("Computer", Optional = true)] public string Computer { get; set; }
        [CsvColumn("User", Optional = true)] public string User { get; set; }

        [CsvIgnore] public int Line { get; set; }
        [CsvIgnore] public bool Tag { get; set; }

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
            CsvLoader.Load<ChainsawLogTamperingData>(filename, DataList, TaggedLines);
        }

    }

    [CsvRecord]
    public partial class ChainsawPowerShellEngineStateData : IFileSpecData
    {
        [CsvColumn(Optional = true), CsvConverter(typeof(UniversalParseTimeConverter))] public DateTime Timestamp { get; set; }
        [CsvColumn("detections", Optional = true)] public string Detections { get; set; }
        [CsvColumn("path", Optional = true)] public string Path { get; set; }

        [CsvColumn("Event ID", Optional = true)] public int EventID { get; set; }
        [CsvColumn("Channel", Optional = true)] public string Channel { get; set; }
        [CsvColumn("Computer", Optional = true)] public string Computer { get; set; }
        [CsvColumn("HostName", Optional = true)] public string HostName { get; set; }
        [CsvColumn("HostVersion", Optional = true)] public string HostVersion { get; set; }
        [CsvColumn("HostApplication", Optional = true)] public string HostApplication { get; set; }
        [CsvColumn("PipelineId", Optional = true)] public string PipelineID { get; set; }
        [CsvColumn("CommandName", Optional = true)] public string CommandName { get; set; }
        [CsvColumn("CommandType", Optional = true)] public string CommandType { get; set; }
        [CsvColumn("ScriptName", Optional = true)] public string ScriptName { get; set; }
        [CsvColumn("CommandPath", Optional = true)] public string CommandPath { get; set; }
        [CsvColumn("CommandLine", Optional = true)] public string CommandLine { get; set; }

        [CsvIgnore] public int Line { get; set; }
        [CsvIgnore] public bool Tag { get; set; }

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
            CsvLoader.Load<ChainsawPowerShellEngineStateData>(filename, DataList, TaggedLines);
        }

    }

    [CsvRecord]
    public partial class ChainsawLoginAttacksData : IFileSpecData
    {
        [CsvColumn(Optional = true), CsvConverter(typeof(UniversalParseTimeConverter))] public DateTime Timestamp { get; set; }
        [CsvColumn("detections", Optional = true)] public string Detections { get; set; }
        [CsvColumn("path", Optional = true)] public string Path { get; set; }

        [CsvColumn("count", Optional = true)] public int Count { get; set; }
        [CsvColumn("Event ID", Optional = true)] public int EventID { get; set; }
        [CsvColumn("User", Optional = true)] public string User { get; set; }

        [CsvIgnore] public int Line { get; set; }
        [CsvIgnore] public bool Tag { get; set; }

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
            CsvLoader.Load<ChainsawLoginAttacksData>(filename, DataList, TaggedLines);
        }

    }

    [CsvRecord]
    public partial class ChainsawIndicatorRemovalData : IFileSpecData
    {
        [CsvColumn(Optional = true), CsvConverter(typeof(UniversalParseTimeConverter))] public DateTime Timestamp { get; set; }
        [CsvColumn("detections", Optional = true)] public string Detections { get; set; }
        [CsvColumn("path", Optional = true)] public string Path { get; set; }

        [CsvColumn("Event ID", Optional = true)] public int EventID { get; set; }
        [CsvColumn("Record ID", Optional = true)] public ulong? RecordID { get; set; }
        [CsvColumn("Computer", Optional = true)] public string Computer { get; set; }
        [CsvColumn("User Name", Optional = true)] public string UserName { get; set; }
        [CsvColumn("Scheduled Task Name", Optional = true)] public string ScheduledTaskName { get; set; }

        [CsvIgnore] public int Line { get; set; }
        [CsvIgnore] public bool Tag { get; set; }

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
            CsvLoader.Load<ChainsawIndicatorRemovalData>(filename, DataList, TaggedLines);
        }

    }

    [CsvRecord]
    public partial class ChainsawPersistenceData : IFileSpecData
    {
        [CsvColumn(Optional = true), CsvConverter(typeof(UniversalParseTimeConverter))] public DateTime Timestamp { get; set; }
        [CsvColumn("detections", Optional = true)] public string Detections { get; set; }
        [CsvColumn("path", Optional = true)] public string Path { get; set; }

        [CsvColumn("Event ID", Optional = true)] public int EventID { get; set; }
        [CsvColumn("Record ID", Optional = true)] public ulong? RecordID { get; set; }
        [CsvColumn("Computer", Optional = true)] public string Computer { get; set; }
        [CsvColumn("User Name", Optional = true)] public string UserName { get; set; }
        [CsvColumn("Scheduled Task Name", Optional = true)] public string ScheduledTaskName { get; set; }
        [CsvColumn("Domain Name", Optional = true)] public string DomainName { get; set; }
        [CsvColumn("Logon ID", Optional = true)] public string LogonID { get; set; }
        [CsvColumn("Parent Process Name", Optional = true)] public string ParentProcessName { get; set; }
        [CsvColumn("Parent Process Command Line", Optional = true)] public string ParentProcessCommandLine { get; set; }
        [CsvColumn("New Process Name", Optional = true)] public string NewProcessName { get; set; }
        [CsvColumn("New Process ID", Optional = true)] public string NewProcessID { get; set; }

        [CsvIgnore] public int Line { get; set; }
        [CsvIgnore] public bool Tag { get; set; }

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
            CsvLoader.Load<ChainsawPersistenceData>(filename, DataList, TaggedLines);
        }

    }

    [CsvRecord]
    public partial class ChainsawRDPAttacksData : IFileSpecData
    {
        [CsvColumn(Optional = true), CsvConverter(typeof(UniversalParseTimeConverter))] public DateTime Timestamp { get; set; }
        [CsvColumn("detections", Optional = true)] public string Detections { get; set; }
        [CsvColumn("path", Optional = true)] public string Path { get; set; }

        [CsvColumn("event id", Optional = true)] public int EventID { get; set; }
        [CsvColumn("computer", Optional = true)] public string Computer { get; set; }
        [CsvColumn("ip address", Optional = true)] public string IPAddress { get; set; }
        [CsvColumn("username", Optional = true)] public string Username { get; set; }
        [CsvColumn("provider", Optional = true)] public string Provider { get; set; }
        [CsvColumn("logon type", Optional = true)] public string LogonType { get; set; }
        [CsvColumn("record id", Optional = true)] public ulong? RecordID { get; set; }
        [CsvColumn("domain", Optional = true)] public string Domain { get; set; }

        [CsvIgnore] public int Line { get; set; }
        [CsvIgnore] public bool Tag { get; set; }

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
            CsvLoader.Load<ChainsawRDPAttacksData>(filename, DataList, TaggedLines);
        }

    }

    [CsvRecord]
    public partial class ChainsawRasVPNData : IFileSpecData
    {
        [CsvColumn(Optional = true), CsvConverter(typeof(UniversalParseTimeConverter))] public DateTime Timestamp { get; set; }
        [CsvColumn("detections", Optional = true)] public string Detections { get; set; }
        [CsvColumn("path", Optional = true)] public string Path { get; set; }

        [CsvColumn("Event ID", Optional = true)] public int EventID { get; set; }
        [CsvColumn("Channel", Optional = true)] public string Channel { get; set; }
        [CsvColumn("Provider", Optional = true)] public string Provider { get; set; }
        [CsvColumn("Computer", Optional = true)] public string Computer { get; set; }
        [CsvColumn("Data", Optional = true)] public string Data { get; set; }

        [CsvIgnore] public int Line { get; set; }
        [CsvIgnore] public bool Tag { get; set; }

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
            CsvLoader.Load<ChainsawRasVPNData>(filename, DataList, TaggedLines);
        }

    }

    [CsvRecord]
    public partial class ChainsawServiceInstallationData : IFileSpecData
    {
        [CsvColumn(Optional = true), CsvConverter(typeof(UniversalParseTimeConverter))] public DateTime Timestamp { get; set; }
        [CsvColumn("detections", Optional = true)] public string Detections { get; set; }
        [CsvColumn("path", Optional = true)] public string Path { get; set; }

        [CsvColumn("Event ID", Optional = true)] public int EventID { get; set; }
        [CsvColumn("Record ID", Optional = true)] public ulong? RecordID { get; set; }
        [CsvColumn("Computer", Optional = true)] public string Computer { get; set; }
        [CsvColumn("Service Name", Optional = true)] public string ServiceName { get; set; }
        [CsvColumn("Service File Name", Optional = true)] public string ServiceFileName { get; set; }
        [CsvColumn("Service Type", Optional = true)] public string ServiceType { get; set; }
        [CsvColumn("Service Start Type", Optional = true)] public string ServiceStartType { get; set; }
        [CsvColumn("Service Account", Optional = true)] public string ServiceAccount { get; set; }

        [CsvIgnore] public int Line { get; set; }
        [CsvIgnore] public bool Tag { get; set; }

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
            CsvLoader.Load<ChainsawServiceInstallationData>(filename, DataList, TaggedLines);
        }

    }

    [CsvRecord]
    public partial class ChainsawServiceTamperingData : IFileSpecData
    {
        [CsvColumn(Optional = true), CsvConverter(typeof(UniversalParseTimeConverter))] public DateTime Timestamp { get; set; }
        [CsvColumn("detections", Optional = true)] public string Detections { get; set; }
        [CsvColumn("path", Optional = true)] public string Path { get; set; }

        [CsvColumn("Event ID", Optional = true)] public int EventID { get; set; }
        [CsvColumn("Username", Optional = true)] public string Username { get; set; }
        [CsvColumn("Computer", Optional = true)] public string Computer { get; set; }
        [CsvColumn("Command", Optional = true)] public string Command { get; set; }
        [CsvColumn("Old Value", Optional = true)] public string OldValue { get; set; }
        [CsvColumn("New Value", Optional = true)] public string NewValue { get; set; }
        [CsvColumn("Service", Optional = true)] public string Service { get; set; }
        [CsvColumn("Old State", Optional = true)] public string OldState { get; set; }
        [CsvColumn("New State", Optional = true)] public string NewState { get; set; }
        [CsvColumn("SID", Optional = true)] public string SID { get; set; }
        [CsvColumn("Record ID", Optional = true)] public ulong? RecordID { get; set; }
        [CsvColumn("Service Name", Optional = true)] public string ServiceName { get; set; }
        [CsvColumn("Action", Optional = true)] public string Action { get; set; }

        [CsvIgnore] public int Line { get; set; }
        [CsvIgnore] public bool Tag { get; set; }

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
            CsvLoader.Load<ChainsawServiceTamperingData>(filename, DataList, TaggedLines);
        }

    }

    [CsvRecord]
    public partial class ChainsawAppLockerData : IFileSpecData
    {
        [CsvColumn(Optional = true), CsvConverter(typeof(UniversalParseTimeConverter))] public DateTime Timestamp { get; set; }
        [CsvColumn("detections", Optional = true)] public string Detections { get; set; }
        [CsvColumn("path", Optional = true)] public string Path { get; set; }

        [CsvColumn("Event ID", Optional = true)] public int EventID { get; set; }
        [CsvColumn("Channel", Optional = true)] public string Channel { get; set; }
        [CsvColumn("Computer", Optional = true)] public string Computer { get; set; }
        [CsvColumn("TargetUser", Optional = true)] public string TargetUser { get; set; }
        [CsvColumn("FullFilePath", Optional = true)] public string FullFilePath { get; set; }

        [CsvIgnore] public int Line { get; set; }
        [CsvIgnore] public bool Tag { get; set; }

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
            CsvLoader.Load<ChainsawAppLockerData>(filename, DataList, TaggedLines);
        }

    }

    [CsvRecord]
    public partial class ChainsawDefenseEvasionData : IFileSpecData
    {
        [CsvColumn(Optional = true), CsvConverter(typeof(UniversalParseTimeConverter))] public DateTime Timestamp { get; set; }
        [CsvColumn("detections", Optional = true)] public string Detections { get; set; }
        [CsvColumn("path", Optional = true)] public string Path { get; set; }

        [CsvColumn("Event ID", Optional = true)] public int EventID { get; set; }
        [CsvColumn("Record ID", Optional = true)] public ulong? RecordID { get; set; }
        [CsvColumn("Computer", Optional = true)] public string Computer { get; set; }
        [CsvColumn("Service Name", Optional = true)] public string ServiceName { get; set; }
        [CsvColumn("Old Startup Type", Optional = true)] public string OldStartupType { get; set; }
        [CsvColumn("Current Startup Type", Optional = true)] public string CurrentStartupType { get; set; }

        [CsvIgnore] public int Line { get; set; }
        [CsvIgnore] public bool Tag { get; set; }

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
            CsvLoader.Load<ChainsawDefenseEvasionData>(filename, DataList, TaggedLines);
        }

    }

    [CsvRecord]
    public partial class ChainsawMFTData : IFileSpecData
    {
        [CsvColumn(Optional = true), CsvConverter(typeof(UniversalParseTimeConverter))] public DateTime Timestamp { get; set; }
        [CsvColumn("detections", Optional = true)] public string Detections { get; set; }
        [CsvColumn("path", Optional = true)] public string Path { get; set; }
        [CsvColumn("FileNamePath", Optional = true)] public string FileNamePath { get; set; }
        [CsvColumn(Optional = true)] public DateTime? StandardInfoLastModified0x10 { get; set; }
        [CsvColumn(Optional = true)] public DateTime? StandardInfoLastAccess0x10 { get; set; }
        [CsvColumn(Optional = true)] public DateTime? FileNameCreated0x30 { get; set; }
        [CsvColumn(Optional = true)] public DateTime? FileNameLastModified0x30 { get; set; }
        [CsvColumn(Optional = true)] public DateTime? FileNameLastAccess0x30 { get; set; }
        [CsvColumn(Optional = true)] public ulong FileSize { get; set; }

        [CsvColumn("IsADirectory", Optional = true)] public bool Directory { get; set; }
        [CsvColumn("IsDeleted", Optional = true)] public bool Deleted { get; set; }
        [CsvColumn(Optional = true)] public bool HasAlternateDataStreams { get; set; }

        [CsvIgnore] public int Line { get; set; }

        [CsvIgnore] public bool Tag { get; set; }

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
            CsvLoader.Load<ChainsawMFTData>(filename, DataList, TaggedLines);
        }
    }

    [CsvRecord]
    public partial class ChainsawMFTDataStreamsData : IFileSpecData
    {
        [CsvColumn(Optional = true), CsvConverter(typeof(UniversalParseTimeConverter))] public DateTime Timestamp { get; set; }
        [CsvColumn("detections", Optional = true)] public string Detections { get; set; }
        [CsvColumn("path", Optional = true)] public string Path { get; set; }
        [CsvColumn("FileNamePath", Optional = true)] public string FileNamePath { get; set; }
        [CsvColumn(Optional = true)] public DateTime? StandardInfoLastModified0x10 { get; set; }
        [CsvColumn(Optional = true)] public DateTime? StandardInfoLastAccess0x10 { get; set; }
        [CsvColumn(Optional = true)] public DateTime? FileNameCreated0x30 { get; set; }
        [CsvColumn(Optional = true)] public DateTime? FileNameLastModified0x30 { get; set; }
        [CsvColumn(Optional = true)] public DateTime? FileNameLastAccess0x30 { get; set; }
        [CsvColumn(Optional = true)] public ulong FileSize { get; set; }

        [CsvColumn("IsADirectory", Optional = true)] public bool Directory { get; set; }
        [CsvColumn("IsDeleted", Optional = true)] public bool Deleted { get; set; }
        [CsvColumn(Optional = true)] public bool HasAlternateDataStreams { get; set; }
        [CsvColumn(Optional = true)] public string DataStreams { get; set; }

        [CsvIgnore] public int Line { get; set; }

        [CsvIgnore] public bool Tag { get; set; }

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
            CsvLoader.Load<ChainsawMFTDataStreamsData>(filename, DataList, TaggedLines);
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
            //(TaggedLines.Contains was a list scan for every row)
            var tagged = new HashSet<int>(TaggedLines);

            DataList.Clear();

            //read by index after the header (comma separated; bad quoting is recovered from)
            using var csv = CsvReader.FromFile(filename, CsvLoader.Options);

            var ln = 1;
            //(RawRecord was built for every row even with debug messages off)
            var debug = Log.IsEnabled(LogEventLevel.Debug);

            //one reset at the end instead of a ListChanged event for every row
            using var quiet = CsvLoader.QuietList<AnalyzeMftData>(DataList);

            while (csv.Read())
            {
                if (debug)
                {
                    Log.Debug("Line # {Line}, Record: {RawRecord}", ln, csv.Current.ToString());
                }

                if (csv.Current.ToString().Contains("BAAD MFT Record"))
                {
                    continue;
                }

                var recNum = int.Parse(csv.GetString(0));
                var isGood = csv.GetString(1).Equals("Good");
                var isActive = csv.GetString(2).Equals("Active");
                var recType = csv.GetString(3);
                var seqNumber = csv.GetString(4);

                var parentEntryNum = csv.GetString(5);
                var parentSeqNum = csv.GetString(6);

                if (parentSeqNum == "NoParent")
                {
                    parentSeqNum = "-1";
                }

                if (parentEntryNum == "NoParent")
                {
                    parentEntryNum = "-1";
                }

                var fileName1 = csv.GetString(7);

                var siCreate1 = csv.GetString(8).Trim('=').Trim('"');

                DateTime? siCreate1D = null;
                DateTime? siModify1d = null;
                DateTime? siAccess1d = null;
                DateTime? siEntry1d = null;

                if (siCreate1.Equals("NoSIRecord") == false && siCreate1.Length > 0)
                {
                    siCreate1D = DateTime.Parse(siCreate1,
                        CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime();
                    siModify1d = DateTime.Parse(csv.GetString(9).Trim('=').Trim('"'),
                        CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime();
                    siAccess1d = DateTime.Parse(csv.GetString(10).Trim('=').Trim('"'),
                        CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime();
                    siEntry1d = DateTime.Parse(csv.GetString(11).Trim('=').Trim('"'),
                        CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime();
                }

                var fnCreate1 = csv.GetString(12).Trim('=').Trim('"');
                var fnModify1 = csv.GetString(13).Trim('=').Trim('"');
                var fnAccess1 = csv.GetString(14).Trim('=').Trim('"');
                var fnEntry1 = csv.GetString(15).Trim('=').Trim('"');

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

                var objectId = csv.GetString(16);
                var birthVolId = csv.GetString(17);
                var birthObjectId = csv.GetString(18);
                var birthDomainId = csv.GetString(19);

                var fileName2 = csv.GetString(20);

                var fnCreate2 = csv.GetString(21).Trim('=').Trim('"');
                DateTime? fnCreate2d = null;
                DateTime? fnModify2d = null;
                DateTime? fnAccess2d = null;
                DateTime? fnEntry2d = null;

                if (fnCreate2.Equals("NoFNRecord") == false && fnCreate2.Length > 0)
                {
                    fnCreate2d = DateTime.Parse(fnCreate2,
                        CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime();
                    fnModify2d = DateTime.Parse(csv.GetString(22).Trim('=').Trim('"'),
                        CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime();
                    fnAccess2d = DateTime.Parse(csv.GetString(23).Trim('=').Trim('"'),
                        CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime();
                    fnEntry2d = DateTime.Parse(csv.GetString(24).Trim('=').Trim('"'),
                        CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime();
                }


                var fileName3 = csv.GetString(25);

                var fnCreate3 = csv.GetString(26).Trim('=').Trim('"');
                DateTime? fnCreate3d = null;
                DateTime? fnModify3d = null;
                DateTime? fnAccess3d = null;
                DateTime? fnEntry3d = null;

                if (fnCreate3.Equals("NoFNRecord") == false && fnCreate3.Length > 0)
                {
                    fnCreate3d = DateTime.Parse(fnCreate3,
                        CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime();
                    fnModify3d = DateTime.Parse(csv.GetString(27).Trim('=').Trim('"'),
                        CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime();
                    fnAccess3d = DateTime.Parse(csv.GetString(29).Trim('=').Trim('"'),
                        CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime();
                    fnEntry3d = DateTime.Parse(csv.GetString(29).Trim('=').Trim('"'),
                        CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime();
                }

                var fileName4 = csv.GetString(30);

                var fnCreate4 = csv.GetString(31).Trim('=').Trim('"');
                DateTime? fnCreate4d = null;
                DateTime? fnModify4d = null;
                DateTime? fnAccess4d = null;
                DateTime? fnEntry4d = null;

                if (fnCreate4.Equals("NoFNRecord") == false && fnCreate4.Length > 0)
                {
                    fnCreate4d = DateTime.Parse(fnCreate4,
                        CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime();
                    fnModify4d = DateTime.Parse(csv.GetString(32).Trim('=').Trim('"'),
                        CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime();
                    fnAccess4d = DateTime.Parse(csv.GetString(33).Trim('=').Trim('"'),
                        CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime();
                    fnEntry4d = DateTime.Parse(csv.GetString(34).Trim('=').Trim('"'),
                        CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime();
                }

                var stdInfo = csv.GetString(35).Equals("True");
                var attrList = csv.GetString(36).Equals("True");
                var fn = csv.GetString(37).Equals("True");
                var objectId2 = csv.GetString(38).Equals("True");
                var volName = csv.GetString(39).Equals("True");
                var volInfo = csv.GetString(40).Equals("True");
                var data = csv.GetString(41).Equals("True");
                var indexRoot = csv.GetString(42).Equals("True");

                var indexAlloc = csv.GetString(43).Equals("True");
                var bitmap = csv.GetString(44).Equals("True");
                var reparsePoint = csv.GetString(45).Equals("True");
                var eaInfo = csv.GetString(46).Equals("True");
                var ea = csv.GetString(47).Equals("True");
                var propSet = csv.GetString(48).Equals("True");
                var loggedUtil = csv.GetString(49).Equals("True");
                var logNotes = csv.GetString(50);
                var fnShift = csv.GetString(51).Equals("Y");
                var uSecZero = csv.GetString(52).Equals("Y");
                var hasAds = csv.GetString(53).Equals("Y");

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
                e.Tag = tagged.Contains(ln);

                DataList.Add(e);
                ln += 1;
            }
        }
    }

    //public string VisitDuration { get; set; }



    [CsvRecord]
    public partial class BrowsingHistoryViewEventData251AndNewer : IFileSpecData
    {
        [CsvColumn("URL", Optional = true)] public string Url { get; set; }
        [CsvColumn(Optional = true)] public string Title { get; set; }
        [CsvColumn("Visit Time", Optional = true), CsvConverter(typeof(ParseTimeConverter))] public DateTime VisitTime { get; set; }
        [CsvColumn("Visit Count", Optional = true)] public int VisitCount { get; set; }
        [CsvColumn("Visited From", Optional = true)] public string VisitedFrom { get; set; }
        [CsvColumn("Visit Type", Optional = true)] public string VisitType { get; set; }
        [CsvColumn("Visit Duration", Optional = true)] public string VisitDuration { get; set; }
        [CsvColumn("Web Browser", Optional = true)] public string WebBrowser { get; set; }
        [CsvColumn("User Profile", Optional = true)] public string UserProfile { get; set; }
        [CsvColumn("Browser Profile", Optional = true)] public string BrowserProfile { get; set; }
        [CsvColumn("URL Length", Optional = true)] public int UrlLength { get; set; }
        [CsvColumn("Typed Count", Optional = true)] public int? TypedCount { get; set; }
        [CsvColumn("History File", Optional = true)] public string HistoryFile { get; set; }
        [CsvColumn("Record ID", Optional = true)] public int RecordId { get; set; }


        [CsvIgnore] public int Line { get; set; }
        [CsvIgnore] public bool Tag { get; set; }

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
        public string FileDescription => "CSV generated from BrowsingHistoryView 2.51 or newer (Visit Time is shown as written: the local time of the machine BrowsingHistoryView ran on, or UTC when it ran with /ShowTimeInGMT 1)";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "cba7c7d7-eefd-431d-898c-07b9ea93c120";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<BrowsingHistoryViewEventData251AndNewer>(filename, DataList, TaggedLines);
        }
    }

    [CsvRecord]
    public partial class BrowsingHistoryViewEventData250AndOlder : IFileSpecData
    {
        [CsvColumn("URL", Optional = true)] public string Url { get; set; }
        [CsvColumn(Optional = true)] public string Title { get; set; }
        [CsvColumn("Visit Time", Optional = true), CsvConverter(typeof(ParseTimeConverter))] public DateTime VisitTime { get; set; }
        [CsvColumn("Visit Count", Optional = true)] public int VisitCount { get; set; }
        [CsvColumn("Visited From", Optional = true)] public string VisitedFrom { get; set; }
        [CsvColumn("Visit Type", Optional = true)] public string VisitType { get; set; }
        [CsvColumn("Web Browser", Optional = true)] public string WebBrowser { get; set; }
        [CsvColumn("User Profile", Optional = true)] public string UserProfile { get; set; }
        [CsvColumn("Browser Profile", Optional = true)] public string BrowserProfile { get; set; }
        [CsvColumn("URL Length", Optional = true)] public int UrlLength { get; set; }
        [CsvColumn("Typed Count", Optional = true)] public int? TypedCount { get; set; }
        [CsvColumn("History File", Optional = true)] public string HistoryFile { get; set; }
        [CsvColumn("Record ID", Optional = true)] public int RecordId { get; set; }


        [CsvIgnore] public int Line { get; set; }
        [CsvIgnore] public bool Tag { get; set; }

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
        public string FileDescription => "CSV generated from BrowsingHistoryView 2.50 or older (Visit Time is shown as written: the local time of the machine BrowsingHistoryView ran on, or UTC when it ran with /ShowTimeInGMT 1)";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; }
        public List<int> TaggedLines { get; set; }

        public string InternalGuid => "30a260e2-0630-4937-9ab7-c3f86bdee49d";

        public void ProcessFile(string filename)
        {
            CsvLoader.Load<BrowsingHistoryViewEventData250AndOlder>(filename, DataList, TaggedLines);
        }
    }


    [CsvRecord]
    public partial class CrowdStrikeEventData : IFileSpecData
    {
        [CsvColumn(Optional = true), CsvConverter(typeof(NoZParseTimeConverter))] public DateTime Timestamp { get; set; }
        [CsvColumn("TreeID", Optional = true)] public string TreeId { get; set; }
        [CsvColumn("PatternID", Optional = true)] public string PatternId { get; set; }
        [CsvColumn("Parent Process ID", Optional = true)] public string ParentProcessId { get; set; }
        [CsvColumn("Process ID", Optional = true)] public string ProcessId { get; set; }
        [CsvColumn(Optional = true)] public string Event { get; set; }
        [CsvColumn("File Loaded/Executed", Optional = true)] public string FileLoadedExecuted { get; set; }
        [CsvColumn(Optional = true)] public string Domain { get; set; }
        [CsvColumn("DestIP", Optional = true)] public string DestIP { get; set; }
        [CsvColumn("File Accessed/Written", Optional = true)] public string FileAccessedWritten { get; set; }
        [CsvColumn("UserSid_readable", Optional = true)] public string UserSid { get; set; }
        [CsvColumn(Optional = true)] public string UserName { get; set; }
        [CsvColumn(Optional = true)] public string Attributes { get; set; }
        [CsvColumn("aid", Optional = true)] public string Aid { get; set; }

        [CsvIgnore] public int Line { get; set; }
        [CsvIgnore] public bool Tag { get; set; }

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
            CsvLoader.Load<CrowdStrikeEventData>(filename, DataList, TaggedLines);
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
            //(TaggedLines.Contains was a list scan for every row)
            var tagged = new HashSet<int>(TaggedLines);

            DataList.Clear();

            //columns by name, matched ignoring case as TLE matches header lines (a lower-case header failed)
            using var csv = CsvReader.FromFile(filename, CsvLoader.Options);

            var ln = 1;
            //(RawRecord was built for every row even with debug messages off)
            var debug = Log.IsEnabled(LogEventLevel.Debug);

            //one reset at the end instead of a ListChanged event for every row
            using var quiet = CsvLoader.QuietList<ShimcacheParserData>(DataList);

            while (csv.Read())
            {
                if (debug)
                {
                    Log.Debug("Line # {Line}, Record: {RawRecord}", ln, csv.Current.ToString());
                }

                var modified = DateTime.Parse(csv.GetString("Last Modified"),
                    CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime();

                var upraw = csv.GetString("Last Update");
                DateTime? update = null;
                if (upraw.Length > 0 && upraw != "N/A")
                {
                    update = DateTime.Parse(upraw,
                        CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime();
                }

                var executed = csv.GetString("Exec Flag") == "True";
                var fileSize = csv.GetString("File Size").Length > 0 && csv.GetString("File Size") != "N/A"
                    ? long.Parse(csv.GetString("File Size"))
                    : 0;
                var filePath = csv.GetString("Path");

                var e = new ShimcacheParserData(ln, modified, update, executed, fileSize, filePath);
                e.Tag = tagged.Contains(ln);
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
            //(TaggedLines.Contains was a list scan for every row)
            var tagged = new HashSet<int>(TaggedLines);

            DataList.Clear();

            //columns by name, matched ignoring case as TLE matches header lines (a lower-case header failed)
            using var csv = CsvReader.FromFile(filename, CsvLoader.Options);

            var ln = 1;
            //(RawRecord was built for every row even with debug messages off)
            var debug = Log.IsEnabled(LogEventLevel.Debug);

            //one reset at the end instead of a ListChanged event for every row
            using var quiet = CsvLoader.QuietList<ShimcacheVolatilityData>(DataList);

            while (csv.Read())
            {
                if (debug)
                {
                    Log.Debug("Line # {Line}, Record: {RawRecord}", ln, csv.Current.ToString());
                }

                var order = int.Parse(csv.GetString("Order"));

                var lastMod = csv.GetString("Last Modified");
                DateTime? lastModTs = null;
                if (lastMod.Length > 0)
                {
                    lastModTs = DateTime.Parse(lastMod,
                        CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime();
                }

                var upraw = csv.GetString("Last Update");
                DateTime? update = null;
                if (upraw.Length > 0)
                {
                    update = DateTime.Parse(upraw,
                        CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime();
                }

                var executed = csv.GetString("Exec Flag") == "True";
                var fileSize = csv.GetString("File Size").Length > 0
                    ? long.Parse(csv.GetString("File Size"))
                    : 0;
                var filePath = csv.GetString("File Path");

                var e = new ShimcacheVolatilityData(ln, order, lastModTs, update, executed, fileSize, filePath);
                e.Tag = tagged.Contains(ln);
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
            //(TaggedLines.Contains was a list scan for every row)
            var tagged = new HashSet<int>(TaggedLines);

            DataList.Clear();

            using var fileReader = File.OpenText(filename);
            var ln = fileReader.ReadLine();
            var ln1 = 1;

            var debug = Log.IsEnabled(LogEventLevel.Debug);

            //one reset at the end instead of a ListChanged event for every row
            using var quiet = CsvLoader.QuietList<DensityScoutData>(DataList);

            while (ln != null)
            {
                if (debug)
                {
                    //(the line was built into the message template, a new template for every row)
                    Log.Debug("Line # {Line}, Record: {Record}", ln1, ln);
                }

                var d = new DensityScoutData();

                var dsegs = ln.Split('|');

                var rawNum = dsegs.First().Substring(1, dsegs.First().Length - 3);

                d.Score = float.Parse(rawNum);
                d.Path = dsegs.Last();
                d.Line = ln1;
                d.Tag = tagged.Contains(ln1);
                DataList.Add(d);
                ln1 += 1;
                ln = fileReader.ReadLine();
            }
        }
    }

    [CsvRecord]
    public partial class VanillaWindowsReferenceData : IFileSpecData
    {
        [CsvColumn(Optional = true)] public string DirectoryName { get; set; }
        [CsvColumn(Optional = true)] public string Name { get; set; }
        [CsvColumn(Optional = true)] public string FullName { get; set; }
        [CsvColumn(Optional = true)] public int Length { get; set; }
        [CsvColumn(Optional = true)] public DateTime CreationTimeUtc { get; set; }
        [CsvColumn(Optional = true)] public DateTime LastAccessTimeUtc { get; set; }
        [CsvColumn(Optional = true)] public DateTime LastWriteTimeUtc { get; set; }
        [CsvColumn(Optional = true)] public string Attributes { get; set; }
        [CsvColumn("MD5", Optional = true)] public string Md5 { get; set; }
        [CsvColumn("SHA256", Optional = true)] public string Sha256 { get; set; }
        [CsvColumn(Optional = true)] public string Sddl { get; set; }

        [CsvIgnore] public int Line { get; set; }
        [CsvIgnore] public bool Tag { get; set; }

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
            CsvLoader.Load<VanillaWindowsReferenceData>(filename, DataList, TaggedLines);
        }
    }

    [CsvRecord]
    public partial class EVTXETWResourcesData : IFileSpecData
    {
        [CsvColumn("Event ID", Optional = true)] public int EventID { get; set; }
        [CsvColumn("Event Version", Optional = true)] public int EventVersion { get; set; }
        [CsvColumn("Level", Optional = true)] public string Level { get; set; }
        [CsvColumn("Channel", Optional = true)] public string Channel { get; set; }
        [CsvColumn("Task", Optional = true)] public string Task { get; set; }
        [CsvColumn("Opcode", Optional = true)] public string Opcode { get; set; }
        [CsvColumn("Keyword", Optional = true)] public string Keyword { get; set; }
        [CsvColumn("Windows", Optional = true)] public string Windows { get; set; }
        [CsvColumn("Version", Optional = true)] public string Version { get; set; }
        [CsvColumn("Edition", Optional = true)] public string Edition { get; set; }
        [CsvColumn(Optional = true), CsvConverter(typeof(UniversalParseTimeConverter))] public DateTime Date { get; set; }
        [CsvColumn("Build", Optional = true)] public float Build { get; set; }
        [CsvColumn("Event Message", Optional = true)] public string EventMessage { get; set; }
        [CsvColumn("Event Fields", Optional = true)] public string EventFields { get; set; }


        [CsvIgnore] public int Line { get; set; }
        [CsvIgnore] public bool Tag { get; set; }

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
            CsvLoader.Load<EVTXETWResourcesData>(filename, DataList, TaggedLines);
        }
    }
    
        [CsvRecord]
        public partial class ConvertPSHistoryToCSVData : IFileSpecData
    {
        // https://gist.github.com/Qazeer/a0c1c14bb1eae233c1147d1d9dfb3e93
        public string User { get; set; }
        public int CommandIndex { get; set; }
        public string Command { get; set; }
        [CsvConverter(typeof(PsHistoryTimestampConverter))] public DateTime ExecutionTimestamp { get; set; }
        public string File { get; set; }

        [CsvIgnore] public int Line { get; set; }
        [CsvIgnore] public bool Tag { get; set; }

        public override string ToString()
        {
            return
                $"{User} {CommandIndex} {Command} {ExecutionTimestamp} {File}";
        }
    }

    /// <summary>ConvertPSHistoryToCSV's ExecutionTimestamp (yyyy-MM-ddTHH:mm:ssZ, UTC); empty or another format is MinValue</summary>
    internal sealed class PsHistoryTimestampConverter : ICsvConverter<DateTime>
    {
        public DateTime Read(CsvReader reader, int index)
        {
            var executionTimestamp = reader.GetString(index);

            if (string.IsNullOrEmpty(executionTimestamp))
            {
                return default;
            }

            return DateTime.TryParseExact(executionTimestamp, "yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var result)
                ? result
                : default;
        }

        public void Write(CsvWriter writer, DateTime value)
        {
            writer.WriteField(value.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture));
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
            //(TaggedLines.Contains was a list scan for every row)
            var tagged = new HashSet<int>(TaggedLines);

            DataList.Clear();

            //the columns are matched ignoring case, as TLE matches header lines (a lower-case header failed)
            using var csv = CsvReader.FromFile(filename, CsvLoader.Options);

            var ln = 1;
            try
            {
                //(RawRecord was built for every row even with debug messages off)
                var debug = Log.IsEnabled(LogEventLevel.Debug);

                //one reset at the end instead of a ListChanged event for every row
                using var quiet = CsvLoader.QuietList<ConvertPSHistoryToCSVData>(DataList);

                foreach (var record in csv.GetRecords<ConvertPSHistoryToCSVData>())
                {
                    if (debug)
                    {
                        Log.Debug("Line # {Line}, Record: {RawRecord}", ln, csv.Current.ToString());
                    }

                    record.Line = ln;

                    record.Tag = tagged.Contains(ln);

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
