using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLEFileKAPE
{
    public class KAPE
    {

        /*
        case
        "copiedtimestamp,sourcefile,destinationfile,filesize,sourcefilesha1,deferredcopy,createdonutc,modifiedonutc,lastaccessedonutc,copyduration"
        :
        FileType = ContentType.Kape;
        break;
        case "\"datetimeutc\",\"eventtimesource\",\"hostname\",\"hostipaddress\",\"userid\",\"assessment\",\"eventtype\",\"eventdetails\",\"sdipaddress\",\"examinercomments\",\"md5\",\"eventaddedby\",\"dateadded\",\"rawevent\"":
        FileType = ContentType.KAPETriage;*/

         /*
         case ContentType.KAPETriage:
                    DataList = new BindingList<KAPETriage>();
                    using (var fileReader = File.OpenText(FileName))
                    {
                        var csv = new CsvReader(fileReader,CultureInfo.InvariantCulture);
                        csv.Configuration.HasHeaderRecord = true;
                        csv.Configuration.Delimiter = separator;
                        var o = new TypeConverterOptions
                        {
                            DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal
                        };
                        csv.Configuration.TypeConverterOptionsCache.AddOptions<KAPETriage>(o);


                        var foo = csv.Configuration.AutoMap<KAPETriage>();

                        foo.Map(t => t.Line).Ignore();
                        foo.Map(t => t.Tag).Ignore();

                        csv.Configuration.RegisterClassMap(foo);

                        var records = csv.GetRecords<KAPETriage>();

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
                    
                        case ContentType.Kape:
                    DataList = new BindingList<KapeCopy>();
                    using (var fileReader = File.OpenText(FileName))
                    {
                        var csv = new CsvReader(fileReader,CultureInfo.InvariantCulture);
                        csv.Configuration.HasHeaderRecord = true;
                        csv.Configuration.Delimiter = separator;

                        var foo = csv.Configuration.AutoMap<KapeCopy>();

                        var o = new TypeConverterOptions
                        {
                            DateTimeStyle = DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal
                        };

                        csv.Configuration.TypeConverterOptionsCache.AddOptions<KapeCopy>(o);

                        foo.Map(t => t.Line).Ignore();
                        foo.Map(t => t.Tag).Ignore();

                        foo.Map(m => m.CopiedTimestamp).TypeConverterOption
                            .DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);


                        foo.Map(m => m.CreatedOnUtc).TypeConverterOption
                            .DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);
                        foo.Map(m => m.ModifiedOnUtc).TypeConverterOption
                            .DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);
                        foo.Map(m => m.LastAccessedOnUtc).TypeConverterOption
                            .DateTimeStyles(DateTimeStyles.AssumeUniversal & DateTimeStyles.AdjustToUniversal);

                        csv.Configuration.RegisterClassMap(foo);

                        var records = csv.GetRecords<KapeCopy>();

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
                    */

    }
}
