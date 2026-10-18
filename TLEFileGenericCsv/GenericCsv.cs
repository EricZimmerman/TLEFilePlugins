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
using Serilog.Events;

namespace TLEFileGenericCsv
{
    public class GenericCsv : IFileSpec
    {
        //what is tried, in this order when two are as frequent (PR #18)
        private static readonly char[] Delimiters = {',', '\t', '|', ';'};

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

        private static ExpandoObject ConvertToDynamic(IDictionary<string, object> fastDynamicObject)
        {
            if (fastDynamicObject == null)
            {
                return null;
            }

            IDictionary<string, object> result = new ExpandoObject();
            foreach (var item in fastDynamicObject)
            {
                //a column without a name, or a second one with the same name, would throw (PR #18)
                if (string.IsNullOrWhiteSpace(item.Key) || result.ContainsKey(item.Key))
                {
                    continue;
                }

                result.Add(item.Key, item.Value);
            }

            return (ExpandoObject) result;
        }

        /// <summary>
        ///     The delimiter that occurs most often in the header line (the first line that isn't a PowerShell #TYPE line)
        /// </summary>
        internal static char DetectDelimiter(string filename)
        {
            var header = File.ReadLines(filename).FirstOrDefault(l => !l.StartsWith("#TYPE")) ?? string.Empty;

            var best = Delimiters[0];
            var bestCount = -1;
            foreach (var d in Delimiters)
            {
                var count = header.Count(c => c == d);
                if (count > bestCount)
                {
                    best = d;
                    bestCount = count;
                }
            }

            return best;
        }

        public void ProcessFile(string filename)
        {
            var tempList = new List<dynamic>();
            var tagged = new HashSet<int>(TaggedLines);

            //.tsv is always tab separated; anything else uses the delimiter its header has most of (it was always a comma: PR #18)
            var tsv = filename.EndsWith(".TSV", StringComparison.OrdinalIgnoreCase);
            var delimiter = tsv ? '\t' : DetectDelimiter(filename);
            Log.Debug("Delimiter: {Delimiter}", delimiter == '\t' ? "tab" : delimiter.ToString());

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = delimiter.ToString(),
                BadDataFound = tsv
                    ? null
                    : context => { Log.Warning("Bad data found in {Field}! Skipping. Raw data: {RawRecord}", context.Field, context.RawRecord); }
            };

            using var fileReader = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var ff = new StreamReader(fileReader);
            using var csv = new CsvReader(ff, config);

            //(RawRecord was built for every row even with debug messages off)
            var debug = Log.IsEnabled(LogEventLevel.Debug);

            var ln = 1;
            while (csv.Read())
            {
                if (csv.Parser.RawRecord.StartsWith("#TYPE"))
                {
                    csv.Read();
                }

                if (debug)
                {
                    Log.Debug("Line # {Line}, Record: {RawRecord}", ln, csv.Context.Parser.RawRecord);
                }

                var f = csv.GetRecord<dynamic>();
                f.Line = ln;
                f.Tag = tagged.Contains(ln);

                tempList.Add(ConvertToDynamic(f));

                ln += 1;
            }

            DataList = new BindingList<dynamic>(tempList);
        }
    }
}
