using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using ITLEFileSpec;
using Serilog;

namespace TLEFileGenericCsv
{
    public class GenericCsv : IFileSpec
    {
        public GenericCsv()
        {
            TaggedLines = new List<int>();

            DataList = new BindingList<dynamic>();

            ExpectedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }

        public string Author => "Eric Zimmerman";
        public string FileDescription => "Generic CSV/TSV plugin. Adds Tag and Line number columns when loading the file";
        public HashSet<string> ExpectedHeaders { get; }

        public IBindingList DataList { get; private set; }
        public List<int> TaggedLines { get; set; }
        public string InternalGuid => "40dd7405-16cf-4612-a480-9a050d0a9952";

        public void ProcessFile(string filename)
        {
            

            var tempList = new List<dynamic>();

            using var fileReader = File.Open(filename, FileMode.Open, FileAccess.Read);
            using var ff = new StreamReader(fileReader);
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                //  BadDataFound = null,
                    
                BadDataFound = context =>
                {
                    Log.Warning("Bad data found in {Field}! Skipping. Raw data: {RawRecord}",context.Field, context.RawRecord);
                },
            };

            if (filename.ToUpperInvariant().EndsWith(".TSV"))
            {
                config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    BadDataFound = null,
                    Delimiter = "\t"
                };
            }

            var csv = new CsvReader(ff, config);
            
            

            var ln = 1;
            while (csv.Read())
            {
                if (csv.Parser.RawRecord.StartsWith("#TYPE"))
                {
                    csv.Read();
                }
                
                
                Log.Debug("Line # {Line}, Record: {RawRecord}",ln,csv.Context.Parser.RawRecord);

                var f = csv.GetRecord<dynamic>();
                f.Line = ln;
                f.Tag = TaggedLines.Contains(ln);

                tempList.Add(f);

                ln += 1;
            }

            // var records = csv.GetRecords<dynamic>().ToList();

            DataList = new BindingList<dynamic>(tempList);
        }
    }
}