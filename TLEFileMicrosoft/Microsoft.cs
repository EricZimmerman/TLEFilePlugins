using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLEFileMicrosoft
{
    public class Microsoft
    {
        //sigcheck
        //sigcheckTroy
        //autoruns
        //troymft


        /*

        

                    case
                        "segmentnumber	inuse	referencecount	isdirectory	isstream	filename	filesize	extension	creationtime0x10	lastmodificationtime0x10	lastchangetime0x10	lastaccesstime0x10	lastmodificationtime0x30	creationtime0x30	lastchangetime0x30	lastaccesstime0x30	path	parentfrs	ownerid	securityid	hasea	isreadonly	ishidden	issystem	isarchive	isnormal	istemporary	issparse	isreparsepoint	iscompressed	isoffline	isnotcontentindexed	isencrypted	isintegritystream	isvirtual	isnoscrubdata	isea	loggedutilitystream	sequencenumber	parseerrormsg"
                        :
                        FileType = ContentType.MsftMft;
                        break;


        case
        "path,verified,date,publisher,company,description,product,product version,file version,machine type,md5,sha1,pesha1,pesha256,sha256,imp,vt detection,vt link":
        FileType = ContentType.SigCheck;
        break;
        case
        "path,verified,date,publisher,company,description,product,product version,file version,machine type,binary version,original name,internal name,copyright,comments,entropy,md5,sha1,pesha1,pesha256,sha256,imp":
        FileType = ContentType.SigCheckTroy;
        break;
        case
        "\"time\",\"entry location\",\"entry\",\"enabled\",\"category\",\"profile\",\"description\",\"signer\",\"company\",\"image path\",\"version\",\"launch string\",\"md5\",\"sha-1\",\"pesha-1\",\"pesha-256\",\"sha-256\",\"imp\",\"pscomputername\",\"runspaceid\",\"psshowcomputername\""
        :
        FileType = ContentType.Autoruns;
        break;
        */

         /*
         case ContentType.MsftMft:
                    DataList = new BindingList<MsftMft>();
                    using (var fileReader = File.OpenText(FileName))
                    {
                        var csv = new CsvReader(fileReader,CultureInfo.InvariantCulture);
                        csv.Configuration.HasHeaderRecord = true;
                        csv.Configuration.Delimiter = separator;

                        var foo = csv.Configuration.AutoMap<MsftMft>();

                        var o = new TypeConverterOptions
                        {
                            DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal
                        };

                        csv.Configuration.TypeConverterOptionsCache.AddOptions<MsftMft>(o);

                        foo.Map(t => t.Line).Ignore();
                        foo.Map(t => t.Tag).Ignore();

                        foo.Map(m => m.CreationTime0x10).ConvertUsing(row =>
                            DateTime.Parse(row.GetField<string>("CreationTime0x10").Replace("Z", "")));
                        foo.Map(m => m.CreationTime0x30).ConvertUsing(row =>
                            DateTime.Parse(row.GetField<string>("CreationTime0x30").Replace("Z", "")));
                        foo.Map(m => m.LastModificationTime0x10).ConvertUsing(row =>
                            DateTime.Parse(row.GetField<string>("LastModificationTime0x10").Replace("Z", "")));
                        foo.Map(m => m.LastModificationTime0x30).ConvertUsing(row =>
                            DateTime.Parse(row.GetField<string>("LastModificationTime0x30").Replace("Z", "")));
                        foo.Map(m => m.LastChangeTime0x10).ConvertUsing(row =>
                            DateTime.Parse(row.GetField<string>("LastChangeTime0x10").Replace("Z", "")));
                        foo.Map(m => m.LastChangeTime0x30).ConvertUsing(row =>
                            DateTime.Parse(row.GetField<string>("LastChangeTime0x30").Replace("Z", "")));
                        foo.Map(m => m.LastAccessTime0x10).ConvertUsing(row =>
                            DateTime.Parse(row.GetField<string>("LastAccessTime0x10").Replace("Z", "")));
                        foo.Map(m => m.LastAccessTime0x30).ConvertUsing(row =>
                            DateTime.Parse(row.GetField<string>("LastAccessTime0x30").Replace("Z", "")));


                        csv.Configuration.RegisterClassMap(foo);

                        var records = csv.GetRecords<MsftMft>();

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


          /*case ContentType.SigCheck:

                    DataList = new BindingList<SigCheck>();
                    using (var fileReader = new StreamReader(FileName, Encoding.GetEncoding(1252)))
                    {
                        var csv = new CsvReader(fileReader,CultureInfo.InvariantCulture);
                        csv.Configuration.HasHeaderRecord = true;
                        csv.Configuration.BadDataFound = null;

                        var o = new TypeConverterOptions
                        {
                            DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal
                        };
                        csv.Configuration.TypeConverterOptionsCache.AddOptions<SigCheck>(o);

                        var foo = csv.Configuration.AutoMap<SigCheck>();

                        foo.Map(t => t.Line).Ignore();
                        foo.Map(t => t.Tag).Ignore();

                        foo.Map(t => t.Verified).ConvertUsing(t => t.GetField("Verified") == "Signed");

                        foo.Map(t => t.Timestamp).Name("Date");
                        foo.Map(t => t.ProductVersion).Name("Product Version");
                        foo.Map(t => t.FileVersion).Name("File Version");
                        foo.Map(t => t.MachineType).Name("Machine Type");
                        foo.Map(t => t.VTDetection).Name("VT detection");
                        foo.Map(t => t.VTLink).Name("VT link");

                        csv.Configuration.RegisterClassMap(foo);

                        var records = csv.GetRecords<SigCheck>();

                        var ln = 1;
                        foreach (var sc in records)
                        {
                            sc.Line = ln;
                            sc.Tag = _taggedLines.Contains(ln);
                            DataList.Add(sc);
                            ln += 1;
                        }
                    }

                    return true;

                case ContentType.SigCheckTroy:

                    DataList = new BindingList<SigCheckTroy>();
                    using (var fileReader = new StreamReader(FileName, Encoding.GetEncoding(1252)))
                    {
                        var csv = new CsvReader(fileReader,CultureInfo.InvariantCulture);
                        csv.Configuration.HasHeaderRecord = true;
                        csv.Configuration.BadDataFound = null;

                        var o = new TypeConverterOptions
                        {
                            DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal
                        };
                        csv.Configuration.TypeConverterOptionsCache.AddOptions<SigCheckTroy>(o);

                        var foo = csv.Configuration.AutoMap<SigCheckTroy>();

                        //path,verified,date,publisher,company,description,product,product version,file version,machine type,binary version,original name,internal name,copyright,comments,entropy,md5,sha1,pesha1,pesha256,sha256,imp

                        foo.Map(t => t.Line).Ignore();
                        foo.Map(t => t.Tag).Ignore();

                        foo.Map(t => t.Verified).ConvertUsing(t => t.GetField("Verified") == "Signed");

                        foo.Map(t => t.Timestamp).Name("Date");
                        foo.Map(t => t.ProductVersion).Name("Product Version");
                        foo.Map(t => t.FileVersion).Name("File Version");
                        foo.Map(t => t.MachineType).Name("Machine Type");
                        foo.Map(t => t.BinaryVersion).Name("Binary Version");
                        foo.Map(t => t.OriginalName).Name("Original Name");
                        foo.Map(t => t.InternalName).Name("Internal Name");
                  

                        csv.Configuration.RegisterClassMap(foo);

                        var records = csv.GetRecords<SigCheckTroy>();

                        var ln = 1;
                        foreach (var sc in records)
                        {
                            sc.Line = ln;
                            sc.Tag = _taggedLines.Contains(ln);
                            DataList.Add(sc);
                            ln += 1;
                        }
                    }

                    return true;
                case ContentType.Autoruns:
                    DataList = new BindingList<AutorunsEntry>();
                    using (var fileReader = File.OpenText(FileName))
                    {
                        var csv = new CsvReader(fileReader,CultureInfo.InvariantCulture);
                        csv.Configuration.HasHeaderRecord = true;
                        var foo = csv.Configuration.AutoMap<AutorunsEntry>();

                        //"Time","Entry Location","Entry","Enabled","Category","Profile","Description","Signer","Company",
                        //"Image Path","Version","Launch String","MD5","SHA-1","PESHA-1","PESHA-256","SHA-256","IMP",
                        //"PSComputerName","RunspaceId","PSShowComputerName"

                        foo.Map(t => t.Line).Ignore();
                        foo.Map(t => t.Tag).Ignore();

                        foo.Map(t => t.EntryLocation).Name("Entry Location");
                        foo.Map(t => t.Enabled).ConvertUsing(t => t[3] == "enabled");
                        foo.Map(t => t.ImagePath).Name("Image Path");
                        foo.Map(t => t.LaunchString).Name("Launch String");
                        foo.Map(t => t.SHA1).Name("SHA-1");
                        foo.Map(t => t.PESHA1).Name("PESHA-1");
                        foo.Map(t => t.SHA256).Name("SHA-256");
                        foo.Map(t => t.PESHA256).Name("PESHA-256");
                        foo.Map(t => t.Imp).Name("IMP");

                        csv.Configuration.RegisterClassMap(foo);

                        var records = csv.GetRecords<AutorunsEntry>();

                        var ln = 1;
                        foreach (var autorunsEntry in records)
                        {
                            autorunsEntry.Line = ln;
                            autorunsEntry.Tag = _taggedLines.Contains(ln);
                            DataList.Add(autorunsEntry);
                            ln += 1;
                        }
                    }

                    return true;*/
    }
}
