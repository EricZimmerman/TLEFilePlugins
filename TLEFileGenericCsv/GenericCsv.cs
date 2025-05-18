using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
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



        private ExpandoObject ConvertToDynamic(IDictionary<string, object> fastDynamicObject) {
            if (fastDynamicObject == null) return null;  
            IDictionary<string,object> result = new ExpandoObject();
            foreach (var item in fastDynamicObject) {
                if (string.IsNullOrWhiteSpace(item.Key))
                    continue; // skip invalid keys
                result.Add(item.Key, item.Value);
            }
            return (ExpandoObject)result;
        }
        
        
        public void ProcessFile(string filename)
        {
            var tempList = new List<dynamic>();

            using var fileReader = File.Open(filename, FileMode.Open, FileAccess.Read);
            using var ff = new StreamReader(fileReader);
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                BadDataFound = context =>
                {
                    Log.Warning("Bad data found in {Field}! Skipping. Raw data: {RawRecord}", context.Field, context.RawRecord);
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
            else {
                // Possible delimiters
                var delimiters = new Dictionary<char, int> { { ',', 0 }, { '\t', 0 }, { '|', 0 }, { ';', 0 } };

                var firstLine = ff.ReadLine();
                foreach (var delimiter in delimiters)
                {
                    // Find the delimiter that has the most occurrences
                    var count = firstLine.Count(c => c == delimiter.Key);
                    delimiters[delimiter.Key] = count;
                }

                Log.Debug("Delimiters: {Delimiters}", delimiters);

                // Find the delimiter with the most occurrences
                var maxDelimiter = delimiters.OrderByDescending(d => d.Value).First().Key;
                Log.Debug("Max delimiter: {MaxDelimiter}", maxDelimiter);

                config.Delimiter = maxDelimiter.ToString();
            }

            ff.BaseStream.Seek(0, SeekOrigin.Begin);
            ff.DiscardBufferedData();
            var csv = new CsvReader(ff, config);

            var ln = 1;
            while (csv.Read())
            {
                if (csv.Parser.RawRecord.StartsWith("#TYPE"))
                {
                    csv.Read();
                }

                Log.Debug("Line # {Line}, Record: {RawRecord}", ln, csv.Context.Parser.RawRecord);

                var f = csv.GetRecord<dynamic>();
                f.Line = ln;
                f.Tag = TaggedLines.Contains(ln);

                ExpandoObject f1 = ConvertToDynamic(f);

                tempList.Add(f1);

                ln += 1;
            }

            DataList = new BindingList<dynamic>(tempList);
        }
    }
}
