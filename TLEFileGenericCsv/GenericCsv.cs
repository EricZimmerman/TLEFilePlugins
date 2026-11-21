using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.IO;
using System.Linq;
using CsvHelper;
using ITLEFileSpec;
using TLEFilePlugins.Shared;
using Serilog;
using Serilog.Events;

namespace TLEFileGenericCsv
{
    public class GenericCsv : IFileSpec
    {
        //what is tried, in this order when two are as frequent (PR #18)
        private const string Separators = ",\t|;";

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
        ///     The reader options for <paramref name="filename" />: .tsv is always tab separated (it was always a comma: PR #18);
        ///     anything else has its separator detected (comma, tab, vertical bar or semicolon, the one that appears the same
        ///     number of times on the most lines). A PowerShell #TYPE line above the header is skipped
        /// </summary>
        private static CsvReaderOptions OptionsFor(string filename, bool tsv)
        {
            //(only the first line is checked: a row starting with #TYPE was skipped too)
            var typeLine = File.ReadLines(filename).FirstOrDefault()?.StartsWith("#TYPE", StringComparison.Ordinal) == true;

            return CsvLoader.Options.With(
                (nameof(CsvReaderOptions.Separator), tsv ? '\t' : (char?) null),
                (nameof(CsvReaderOptions.SeparatorCandidates), Separators),
                (nameof(CsvReaderOptions.SkipLines), typeLine ? 1 : 0),
                //bad quoting is recovered from (the record is kept), and reported for a CSV
                (nameof(CsvReaderOptions.OnMalformedRecord), tsv
                    ? null
                    : (Action<CsvMalformedRecord>) (record => Log.Warning("Bad data found ({Reason}) in record {Record}! Raw data: {RawRecord}", record.Reason, record.RecordNumber, record.RawRecord))));
        }

        /// <summary>
        ///     The separator CsvHelper detects in <paramref name="filename" /> (after a PowerShell #TYPE line)
        /// </summary>
        internal static char DetectDelimiter(string filename)
        {
            using var csv = CsvReader.FromFile(filename, OptionsFor(filename, false).With((nameof(CsvReaderOptions.OnMalformedRecord), null)));
            return csv.Separator;
        }

        public void ProcessFile(string filename)
        {
            var tempList = new List<dynamic>();
            var tagged = new HashSet<int>(TaggedLines);

            var tsv = filename.EndsWith(".TSV", StringComparison.OrdinalIgnoreCase);

            using var fileReader = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var csv = CsvReader.Create(fileReader, OptionsFor(filename, tsv));

            Log.Debug("Delimiter: {Delimiter}", csv.Separator == '\t' ? "tab" : csv.Separator.ToString());

            //(RawRecord was built for every row even with debug messages off)
            var debug = Log.IsEnabled(LogEventLevel.Debug);

            //each row goes straight into an ExpandoObject (Timeline Explorer needs one): CsvHelper's dynamic record would be
            //made and then copied for every row. A column without a name is left out, and of two with the same name the
            //last value is kept in the first one's place, as before (PR #18); a field a short row doesn't have is null
            var ln = 1;
            string[] names = null;
            //(CsvHelper 33's CacheFields; PoolStrings alone stops pooling most columns of a large file)
            var pool = new StringPool();
            while (csv.Read())
            {
                names ??= csv.Headers.ToArray();

                if (debug)
                {
                    Log.Debug("Line # {Line}, Record: {RawRecord}", ln, csv.Current.ToString());
                }

                IDictionary<string, object> row = new ExpandoObject();
                for (var i = 0; i < names.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(names[i]))
                    {
                        continue;
                    }

                    row[names[i]] = i < csv.FieldCount ? pool.Get(csv.GetString(i)) : null;
                }

                row["Line"] = ln;
                row["Tag"] = tagged.Contains(ln);

                tempList.Add(row);

                ln += 1;
            }

            DataList = new BindingList<dynamic>(tempList);
        }
    }
}
