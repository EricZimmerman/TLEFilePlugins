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



    //EvtxEcmd
    //RBCmd
    //Amcache
    //Appcompatcache
    //sbecmd
    //jlecmdcustom
    //jlecmdauto
    //lecmd
    //pecmd
    //pecmdtimeline
    //WxT
    //


    /*
    case "sourcename,filetype,filename,filesize,deletedon":
    FileType = ContentType.RBCmd;
    break;
    */

    /*case "recordnumber,eventrecordid,timecreated,eventid,level,provider,channel,processid,threadid,computer,userid,mapdescription,chunknumber,username,remotehost,payloaddata1,payloaddata2,payloaddata3,payloaddata4,payloaddata5,payloaddata6,executableinfo,sourcefile,payload":
    case "recordnumber,eventrecordid,timecreated,eventid,level,provider,channel,processid,threadid,computer,userid,mapdescription,chunknumber,username,remotehost,payloaddata1,payloaddata2,payloaddata3,payloaddata4,payloaddata5,payloaddata6,executableinfo,hiddenrecord,sourcefile,payload,extradataoffset":
    case "recordnumber,eventrecordid,timecreated,eventid,level,provider,channel,processid,threadid,computer,userid,mapdescription,chunknumber,username,remotehost,payloaddata1,payloaddata2,payloaddata3,payloaddata4,payloaddata5,payloaddata6,executableinfo,hiddenrecord,sourcefile,payload,keywords,extradataoffset":
    FileType = ContentType.EvtxECmd;
    break;
    case "hivepath,hivetype,description,category,keypath,valuename,valuetype,valuedata,valuedata2,valuedata3,comment,recursive,deleted,lastwritetimestamp,plugindetailfile":
    FileType = ContentType.RecmdBatch;
    break;
    */


    
                    /*
                    case
                        "keyname,keylastwritetimestamp,busreporteddescription,class,classguid,compid,containerid,description,driverid,driverpackagestrongname,drivername,driververdate,driververversion,enumerator,hwid,inf,installstate,manufacturer,matchingid,model,parentid,problemcode,provider,service,stackid"
                        :
                    case
                        "keyname	keylastwritetimestamp	busreporteddescription	class	classguid	compid	containerid	description	driverid	driverpackagestrongname	drivername	driververdate	driververversion	enumerator	hwid	inf	installstate	manufacturer	matchingid	model	parentid	problemcode	provider	service	stackid"
                        :
                        FileType = ContentType.AmcacheParserNewDevicePnP;
                        break;
                    case
                        "keyname,keylastwritetimestamp,categories,discoverymethod,friendlyname,icon,isactive,isconnected,ismachinecontainer,isnetworked,ispaired,manufacturer,modelid,modelname,modelnumber,primarycategory,state"
                        :
                    case
                        "keyname	keylastwritetimestamp	categories	discoverymethod	friendlyname	icon	isactive	isconnected	ismachinecontainer	isnetworked	ispaired	manufacturer	modelid	modelname	modelnumber	primarycategory	state"
                        :
                        FileType = ContentType.AmcacheParserNewDeviceContainer;
                        break;
                    case
                        "keyname,keylastwritetimestamp,date,class,directory,driverinbox,hwids,inf,provider,submissionid,sysfile,version"
                        :
                    case
                        "keyname	keylastwritetimestamp	date	class	directory	driverinbox	hwids	inf	provider	submissionid	sysfile	version"
                        :
                        FileType = ContentType.AmcacheParserNewDriverPackage;
                        break;
                    case
                        "keyname,keylastwritetimestamp,drivertimestamp,driverlastwritetime,drivername,driverinbox,driveriskernelmode,driversigned,driverchecksum,drivercompany,driverid,driverpackagestrongname,drivertype,driverversion,imagesize,inf,product,productversion,service,wdfversion"
                        :
                    case
                        "keyname	keylastwritetimestamp	drivertimestamp	driverlastwritetime	drivername	driverinbox	driveriskernelmode	driversigned	driverchecksum	drivercompany	driverid	driverpackagestrongname	drivertype	driverversion	imagesize	inf	product	productversion	service	wdfversion"
                        :
                        FileType = ContentType.AmcacheParserNewDriveBinary;
                        break;
                    case "keyname,lnkname,keylastwritetimestamp":
                    case "keyname	lnkname	keylastwritetimestamp":
                        FileType = ContentType.AmcacheParserShortcuts;
                        break;
                    case
                        "programid,keylastwritetimestamp,name,version,publisher,installdate,osversionatinstalltime,bundlemanifestpath,hiddenarp,inboxmodernapp,language,manifestpath,msipackagecode,msiproductcode,packagefullname,programinstanceid,registrykeypath,rootdirpath,type,source,storeapptype,uninstallstring"
                        :
                    case
                        "programid	keylastwritetimestamp	name	version	publisher	installdate	osversionatinstalltime	bundlemanifestpath	hiddenarp	inboxmodernapp	language	manifestpath	msipackagecode	msiproductcode	packagefullname	programinstanceid	registrykeypath	rootdirpath	type	source	storeapptype	uninstallstring"
                        :
                        FileType = ContentType.AmcacheParserNewPrograms;
                        break;
                  
                    case "controlset,cacheentryposition,path,lastmodifiedtimeutc,executed,duplicate,sourcefile":
                        FileType = ContentType.AppCompatcache;
                        break;
                    case
                        "bagpath,slot,nodeslot,mruposition,absolutepath,shelltype,value,childbags,createdon,modifiedon,accessedon,lastwritetime,mftentry,mftsequencenumber,extensionblockcount,firstinteracted,lastinteracted,hasexplored,miscellaneous"
                        :
                   
                        FileType = ContentType.SBECmd;
                        break;
                    case
                        "sourcefile,sourcecreated,sourcemodified,sourceaccessed,appid,appiddescription,entryname,targetcreated,targetmodified,targetaccessed,filesize,relativepath,workingdirectory,fileattributes,headerflags,drivetype,driveserialnumber,drivelabel,localpath,commonpath,targetidabsolutepath,targetmftentrynumber,targetmftsequencenumber,machineid,machinemacaddress,trackercreatedon,extrablockspresent,arguments"
                        :
                  
                        FileType = ContentType.JLECmdCustom;
                        break;
                    case
                        "sourcefile,sourcecreated,sourcemodified,sourceaccessed,appid,appiddescription,destlistversion,lastusedentrynumber,mru,entrynumber,creationtime,lastmodified,hostname,macaddress,path,interactioncount,pinstatus,filebirthdroid,filedroid,volumebirthdroid,volumedroid,targetcreated,targetmodified,targetaccessed,filesize,relativepath,workingdirectory,fileattributes,headerflags,drivetype,volumeserialnumber,volumelabel,localpath,commonpath,targetidabsolutepath,targetmftentrynumber,targetmftsequencenumber,machineid,machinemacaddress,trackercreatedon,extrablockspresent,arguments,notes"
                        :
                        FileType = ContentType.JLECmdAuto;
                        break;
                    case
                        "sourcefile,sourcecreated,sourcemodified,sourceaccessed,targetcreated,targetmodified,targetaccessed,filesize,relativepath,workingdirectory,fileattributes,headerflags,drivetype,volumeserialnumber,volumelabel,localpath,networkpath,commonpath,arguments,targetidabsolutepath,targetmftentrynumber,targetmftsequencenumber,machineid,machinemacaddress,macvendor,trackercreatedon,extrablockspresent"
                        :

                        FileType = ContentType.LECmd;
                        break;
                    case "runtime,executablename":
                        FileType = ContentType.PECmdTimeline;
                        break;
                    case "note,sourcefilename,sourcecreated,sourcemodified,sourceaccessed,executablename,hash,size,version,runcount,lastrun,previousrun0,previousrun1,previousrun2,previousrun3,previousrun4,previousrun5,previousrun6,volume0name,volume0serial,volume0created,volume1name,volume1serial,volume1created,directories,filesloaded":
                    case
                        "note,sourcefilename,sourcecreated,sourcemodified,sourceaccessed,executablename,hash,size,version,runcount,lastrun,previousrun0,previousrun1,previousrun2,previousrun3,previousrun4,previousrun5,previousrun6,volume0name,volume0serial,volume0created,volume1name,volume1serial,volume1created,directories,filesloaded,parsingerror"
                        :
                  
                        FileType = ContentType.PECmd;
                        break;
                    case
                        "programid,lastwritetimestamp,programname_0,programversion_1,vendorname_2,installdateepoch_a,installdateepoch_b,languagecode_3,installsource_6,uninstallregistrykey_7,pathslist_d"
                        :
                  
                        FileType = ContentType.AmcacheParserPrograms;
                        break;
                    case
                        "programname,programid,volumeid,volumeidlastwritetimestamp,fileid,fileidlastwritetimestamp,sha1,fullpath,fileextension,mftentrynumber,mftsequencenumber,filesize,fileversionstring,fileversionnumber,filedescription,peheadersize,peheaderhash,peheaderchecksum,created,lastmodified,lastmodified2,compiletime,languageid"
                        :
                   
                        FileType = ContentType.AmcacheParserFiles;
                        break;

                 
                    case
                        "applicationname,programid,filekeylastwritetimestamp,sha1,isoscomponent,fullpath,name,fileextension,linkdate,productname,size,version,productversion,longpathhash,binarytype,ispefile,binfileversion,binproductversion,language"
                        :
                    case
                        "applicationname	programid	filekeylastwritetimestamp	sha1	isoscomponent	fullpath	name	fileextension	linkdate	productname	size	version	productversion	longpathhash	binarytype	ispefile	binfileversion	binproductversion	language"
                        :
                        FileType = ContentType.AmcacheParserNewFile;
                        break;
                    
                    case "id,platform,name,additionalinformation,expires":
                    case "id	platform	name	additionalinformation	expires":
                        FileType = ContentType.WxTActivityPackageId;
                        break;
                    case
                        "id,executable,displaytext,contentinfo,starttime,endtime,lastmodifiedtime,expirationtime,createdincloud,lastmodifiedonclient,originallastmodifiedonclient,activitytype,islocalonly,etag,packageidhash,platformdeviceid,duration"
                        :
                    case
                        "id	executable	displaytext	contentinfo	starttime	endtime	lastmodifiedtime	expirationtime	createdincloud	lastmodifiedonclient	originallastmodifiedonclient	activitytype	islocalonly	etag	packageidhash	platformdeviceid	duration"
                        :
                        FileType = ContentType.WxTActivity;
                        break;*/


      /*
      case ContentType.EvtxECmd:
                    DataList = new BindingList<EvtECmd>();
                    using (var fileReader = File.OpenText(FileName))
                    {
                        var csv = new CsvReader(fileReader,CultureInfo.InvariantCulture);
                        csv.Configuration.HasHeaderRecord = true;
                        csv.Configuration.Delimiter = separator;
                        var o = new TypeConverterOptions
                        {
                            DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal
                        };
                        csv.Configuration.TypeConverterOptionsCache.AddOptions<EvtECmd>(o);

                        var foo = csv.Configuration.AutoMap<EvtECmd>();

                        foo.Map(t => t.Line).Ignore();
                        foo.Map(t => t.Tag).Ignore();
                        foo.Map(t => t.Payload).Optional();
                        foo.Map(t => t.HiddenRecord).Optional();
                        foo.Map(t => t.Keywords).Optional();

                        csv.Configuration.RegisterClassMap(foo);

                        var records = csv.GetRecords<EvtECmd>();

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
                case ContentType.RecmdBatch:
                    DataList = new BindingList<RecmdBatch>();
                    using (var fileReader = File.OpenText(FileName))
                    {
                        var csv = new CsvReader(fileReader,CultureInfo.InvariantCulture);
                        csv.Configuration.HasHeaderRecord = true;
                        csv.Configuration.Delimiter = separator;
                        var o = new TypeConverterOptions
                        {
                            DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal
                        };
                        csv.Configuration.TypeConverterOptionsCache.AddOptions<RecmdBatch>(o);

                        var foo = csv.Configuration.AutoMap<RecmdBatch>();

                        foo.Map(t => t.Line).Ignore();
                        foo.Map(t => t.Tag).Ignore();

                        csv.Configuration.RegisterClassMap(foo);

                        var records = csv.GetRecords<RecmdBatch>();

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

              

             
                case ContentType.RBCmd:
                    DataList = new BindingList<RbCmd>();
                    using (var fileReader = File.OpenText(FileName))
                    {
                        var csv = new CsvReader(fileReader,CultureInfo.InvariantCulture);
                        csv.Configuration.HasHeaderRecord = true;
                        csv.Configuration.Delimiter = separator;
                        var o = new TypeConverterOptions
                        {
                            DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal
                        };
                        csv.Configuration.TypeConverterOptionsCache.AddOptions<RbCmd>(o);

                        var foo = csv.Configuration.AutoMap<RbCmd>();

                        foo.Map(m => m.DeletedOn).TypeConverterOption
                            .DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);

                        foo.Map(t => t.Line).Ignore();
                        foo.Map(t => t.Tag).Ignore();

                        csv.Configuration.RegisterClassMap(foo);

                        var records = csv.GetRecords<RbCmd>();

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

            

              
               


        

              

                case ContentType.AmcacheParserFiles:

                    DataList = new BindingList<AmcacheParserFiles>();
                    using (var fileReader = File.OpenText(FileName))
                    {
                        var csv = new CsvReader(fileReader,CultureInfo.InvariantCulture);
                        csv.Configuration.HasHeaderRecord = true;
                        csv.Configuration.Delimiter = separator;
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
                        csv.Configuration.Delimiter = separator;

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
                        csv.Configuration.Delimiter = separator;
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

                case ContentType.AppCompatcache:
                    DataList = new BindingList<AppCompatcache>();
                    using (var fileReader = File.OpenText(FileName))
                    {
                        var csv = new CsvReader(fileReader,CultureInfo.InvariantCulture);
                        csv.Configuration.HasHeaderRecord = true;
                        csv.Configuration.Delimiter = separator;

                        var o = new TypeConverterOptions
                        {
                            DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal
                        };
                        csv.Configuration.TypeConverterOptionsCache.AddOptions<AppCompatcache>(o);

                        var foo = csv.Configuration.AutoMap<AppCompatcache>();

                        foo.Map(t => t.Line).Ignore();
                        foo.Map(t => t.Tag).Ignore();

                        csv.Configuration.RegisterClassMap(foo);

                        var records = csv.GetRecords<AppCompatcache>();

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

                case ContentType.JLECmdAuto:

                    DataList = new BindingList<JLECmdAuto>();
                    using (var fileReader = File.OpenText(FileName))
                    {
                        var csv = new CsvReader(fileReader,CultureInfo.InvariantCulture);
                        csv.Configuration.HasHeaderRecord = true;
                        csv.Configuration.Delimiter = separator;

                        csv.Configuration.PrepareHeaderForMatch =  (header, index) => header.Replace(" ", "");

                        var o = new TypeConverterOptions
                        {
                            DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal
                        };
                        csv.Configuration.TypeConverterOptionsCache.AddOptions<JLECmdAuto>(o);

                        var foo = csv.Configuration.AutoMap<JLECmdAuto>();

                        foo.Map(t => t.Line).Ignore();
                        foo.Map(t => t.Tag).Ignore();

                        csv.Configuration.RegisterClassMap(foo);

                        var records = csv.GetRecords<JLECmdAuto>();

                        var ln = 1;
                        foreach (var record in records)
                        {
                            if (record.FileAttributes.Equals("0"))
                            {
                                record.FileAttributes = string.Empty;
                            }

                            record.Line = ln;
                            record.Tag = _taggedLines.Contains(ln);
                            DataList.Add(record);

                            ln += 1;
                        }
                    }

                    return true;

                case ContentType.JLECmdCustom:
                    DataList = new BindingList<JLECmdCustom>();
                    using (var fileReader = File.OpenText(FileName))
                    {
                        var csv = new CsvReader(fileReader,CultureInfo.InvariantCulture);
                        csv.Configuration.HasHeaderRecord = true;
                        csv.Configuration.Delimiter = separator;

                        var o = new TypeConverterOptions
                        {
                            DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal
                        };
                        csv.Configuration.TypeConverterOptionsCache.AddOptions<JLECmdCustom>(o);

                        csv.Configuration.PrepareHeaderForMatch =  (header, index) => header.Replace(" ", "");

                        var foo = csv.Configuration.AutoMap<JLECmdCustom>();

                        foo.Map(t => t.Line).Ignore();
                        foo.Map(t => t.Tag).Ignore();

                        csv.Configuration.RegisterClassMap(foo);

                        var records = csv.GetRecords<JLECmdCustom>();

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

                case ContentType.LECmd:
                    DataList = new BindingList<LECmd>();
                    using (var fileReader = File.OpenText(FileName))
                    {
                        var csv = new CsvReader(fileReader,CultureInfo.InvariantCulture);
                        csv.Configuration.HasHeaderRecord = true;
                        csv.Configuration.Delimiter = separator;

                        var o = new TypeConverterOptions
                        {
                            DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal
                        };
                        csv.Configuration.TypeConverterOptionsCache.AddOptions<LECmd>(o);

                        csv.Configuration.PrepareHeaderForMatch =  (header, index) => header.Replace(" ", "");

                        var foo = csv.Configuration.AutoMap<LECmd>();

                        foo.Map(t => t.Line).Ignore();
                        foo.Map(t => t.Tag).Ignore();

                        csv.Configuration.RegisterClassMap(foo);

                        var records = csv.GetRecords<LECmd>();

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

                case ContentType.PECmd:
                    DataList = new BindingList<PECmd>();
                    using (var fileReader = File.OpenText(FileName))
                    {
                        var csv = new CsvReader(fileReader,CultureInfo.InvariantCulture);
                        csv.Configuration.HasHeaderRecord = true;
                        csv.Configuration.Delimiter = separator;
                        var o = new TypeConverterOptions
                        {
                            DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal
                        };
                        csv.Configuration.TypeConverterOptionsCache.AddOptions<PECmd>(o);

                        var foo = csv.Configuration.AutoMap<PECmd>();

                        foo.Map(t => t.Line).Ignore();
                        foo.Map(t => t.Tag).Ignore();

                        csv.Configuration.RegisterClassMap(foo);

                        var records = csv.GetRecords<PECmd>();

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

                case ContentType.PECmdTimeline:
                    DataList = new BindingList<PECmdTimeline>();
                    using (var fileReader = File.OpenText(FileName))
                    {
                        var csv = new CsvReader(fileReader,CultureInfo.InvariantCulture);
                        csv.Configuration.HasHeaderRecord = true;
                        csv.Configuration.Delimiter = separator;
                        var o = new TypeConverterOptions
                        {
                            DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal
                        };
                        csv.Configuration.TypeConverterOptionsCache.AddOptions<PECmdTimeline>(o);

                        var foo = csv.Configuration.AutoMap<PECmdTimeline>();

                        foo.Map(t => t.Line).Ignore();
                        foo.Map(t => t.Tag).Ignore();

                        csv.Configuration.RegisterClassMap(foo);

                        var records = csv.GetRecords<PECmdTimeline>();

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

                case ContentType.SBECmd:
                    DataList = new BindingList<SBECmd>();
                    using (var fileReader = File.OpenText(FileName))
                    {
                        var csv = new CsvReader(fileReader,CultureInfo.InvariantCulture);
                        csv.Configuration.HasHeaderRecord = true;
                        csv.Configuration.Delimiter = separator;

                        var o = new TypeConverterOptions
                        {
                            DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal
                        };
                        csv.Configuration.TypeConverterOptionsCache.AddOptions<SBECmd>(o);

                       csv.Configuration.PrepareHeaderForMatch = (header, index) => header.Replace(" ", "");

                        var foo = csv.Configuration.AutoMap<SBECmd>();

                        foo.Map(t => t.Line).Ignore();
                        foo.Map(t => t.Tag).Ignore();

                        csv.Configuration.RegisterClassMap(foo);

                        var records = csv.GetRecords<SBECmd>();

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

        /*
        case ContentType.WxTActivity:
                    DataList = new BindingList<ActivityEntry>();
                    using (var fileReader = File.OpenText(FileName))
                    {
                        var csv = new CsvReader(fileReader,CultureInfo.InvariantCulture);
                        csv.Configuration.HasHeaderRecord = true;
                        csv.Configuration.Delimiter = separator;
                        var foo = csv.Configuration.AutoMap<ActivityEntry>();

                        var o = new TypeConverterOptions
                        {
                            DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal
                        };
                        csv.Configuration.TypeConverterOptionsCache.AddOptions<ActivityEntry>(o);

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

                        var records = csv.GetRecords<ActivityEntry>();

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


                case ContentType.WxTActivityPackageId:
                    DataList = new BindingList<ActivityPackageEntry>();
                    using (var fileReader = File.OpenText(FileName))
                    {
                        var csv = new CsvReader(fileReader,CultureInfo.InvariantCulture);
                        csv.Configuration.HasHeaderRecord = true;
                        var foo = csv.Configuration.AutoMap<ActivityPackageEntry>();

                        var o = new TypeConverterOptions
                        {
                            DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal
                        };
                        csv.Configuration.TypeConverterOptionsCache.AddOptions<ActivityPackageEntry>(o);

                        foo.Map(t => t.Line).Ignore();
                        foo.Map(t => t.Tag).Ignore();
                        foo.Map(m => m.Expires).TypeConverterOption
                            .DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);

                        csv.Configuration.RegisterClassMap(foo);


                        var records = csv.GetRecords<ActivityPackageEntry>();

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
                        csv.Configuration.Delimiter = separator;
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
                        csv.Configuration.Delimiter = separator;
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
                        csv.Configuration.Delimiter = separator;
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
                        csv.Configuration.Delimiter = separator;
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
                        csv.Configuration.Delimiter = separator;
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
                        csv.Configuration.Delimiter = separator;
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