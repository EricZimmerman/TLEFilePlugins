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

            //the header line, after a PowerShell #TYPE line (only the first line is checked: a row starting with #TYPE was
            //skipped too)
            var hasHeader = csv.Read();
            if (hasHeader && csv.Parser.RawRecord.StartsWith("#TYPE", StringComparison.Ordinal))
            {
                hasHeader = csv.Read();
            }

            if (hasHeader)
            {
                csv.ReadHeader();

                //each row goes straight into an ExpandoObject (Timeline Explorer needs one): CsvHelper's dynamic record
                //was made and then copied for every row. A column without a name is left out, and of two with the same
                //name the last value is kept in the first one's place, as before (PR #18)
                var names = csv.HeaderRecord;

                var ln = 1;
                while (csv.Read())
                {
                    if (debug)
                    {
                        Log.Debug("Line # {Line}, Record: {RawRecord}", ln, csv.Context.Parser.RawRecord);
                    }

                    IDictionary<string, object> row = new ExpandoObject();
                    for (var i = 0; i < names.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(names[i]))
                        {
                            continue;
                        }

                        csv.TryGetField(i, out string field);
                        row[names[i]] = field;
                    }

                    row["Line"] = ln;
                    row["Tag"] = tagged.Contains(ln);

                    tempList.Add(row);

                    ln += 1;
                }
            }

            DataList = new BindingList<dynamic>(tempList);
        }
    }
}
